using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using YourConfigIsInAnotherCastle.Configuration;
using System.Reflection;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using YourConfigIsInAnotherCastle.Models;
using YourConfigIsInAnotherCastle.Caching;
using YourConfigIsInAnotherCastle.Exceptions;
using YourConfigIsInAnotherCastle.Parser;

namespace YourConfigIsInAnotherCastle
{

    public class InAnotherCastleHandler : IConfigurationSectionHandler
    {

        public IInAnotherCastleParser Parser { get; set; }
        private static IInAnotherCastleConfiguration _configuration;
        public static IInAnotherCastleConfiguration Configuration
        {
            get { return _configuration; }
            set
            {
                if (_configuration != null && _configuration.ServiceProvider != null && _wasProviderCreatedHere)
                {
                    _configuration.ServiceProvider.Dispose();
                }
                _configuration = value;
                CreateServiceProviderIfNotPassed();
            }
        }
        private static bool _wasProviderCreatedHere = false;
        private static void CreateServiceProviderIfNotPassed()
        {
            _wasProviderCreatedHere = false;
            if (Configuration != null)
            {
                if (Configuration.ServiceProvider == null)
                {
                    if (string.IsNullOrWhiteSpace(Configuration.ServiceProviderTypeName))
                    {
                        throw new ArgumentException("Either the ServiceProviderType must be sent and be a valid type name, or ServiceProvider must be assigned.");
                    }
                    else
                    {
                        try
                        {
                            Configuration.ServiceProvider = (IInAnotherCastleServiceProvider)Activator.CreateInstance(Type.GetType(Configuration.ServiceProviderTypeName, true));
                            _wasProviderCreatedHere = true;
                        }
                        catch (TypeLoadException exc)
                        {
                            throw new InAnotherCastleInvalidTypeException("Service Provider could not be loaded, see inner exception for details", exc);
                        }
                    }
                }
                else
                {
                    Configuration.ServiceProviderTypeName = Configuration.ServiceProvider.GetType().AssemblyQualifiedName;
                }
            }
        }

        public const string DefaultConfigurationSectionName = "InAnotherCastleConfigSection";
        public static void ClearConfig()
        {
            Configuration = null;
        }

        public InAnotherCastleHandler()
        {
            //TODO move to config
            Parser = new DefaultInAnotherCastleParser();
        }
        public static void RefreshConfiguration()
        {
            Configuration = ConfigurationManager.GetSection(DefaultConfigurationSectionName) as InAnotherCastleConfigSection;
            if (Configuration == null)
            {
                Configuration = new DefaultInAnotherCastleConfiguration();
            }
        }


        public const int CacheDurationUnDefinedUseMinutesInADay = 1440;
        public object Create(object parent, object configContext, XmlNode xmlInConfig)
        {

            if (_configuration == null) RefreshConfiguration();

            var cache = Configuration.ServiceProvider.GetConfigCache();

            //Cache and return by section name
            return cache.GetCache(xmlInConfig.Name, () =>
                {
                    //Parse the section for details about where it is?
                    //TODO should the service provider be specified per redirect?? and then  you could have config in more than one place?
                    var redirectInformation = this.Parser.ParseRedirectDetails(xmlInConfig.OuterXml);

                    Type expectedType = Type.GetType(redirectInformation.Type, true);

                    //Results for cache result storage and information
                    CacheInformation cacheResults = new CacheInformation()
                    {
                        Duration = new TimeSpan(0, redirectInformation.CacheDurationInMinutes == 0 ? CacheDurationUnDefinedUseMinutesInADay : redirectInformation.CacheDurationInMinutes, 0),
                    };

                    //TODO ignore name and system name  when in config section and has ID - needs to be implemented
                    using (var storage = Configuration.ServiceProvider.GetStorage())
                    {
                        //The whole purpose is to handle standard config sections designed in System.Configuration
                        if (redirectInformation.Mode == Mode.Standard)
                        {
                            var configurationValue = storage.GetConfigurationSection(redirectInformation.Name, redirectInformation.SystemName);
                            cacheResults.Value = this.Parser.StandardConfigSectionParse(configurationValue.XML, expectedType);
                        }
                        //Handle regular POCO calls
                        else if (redirectInformation.Mode == Mode.Poco)
                        {
                            if (redirectInformation.PocoBody != null && !string.IsNullOrWhiteSpace(redirectInformation.PocoBody.Value))
                            {
                                cacheResults.Value = this.Parser.POCOConfigSectionParse(redirectInformation.PocoBody.Value, expectedType);
                            }
                            else
                            {
                                var configurationValue = storage.GetConfigurationSection(redirectInformation.Name, redirectInformation.SystemName);
                                cacheResults.Value = this.Parser.POCOConfigSectionParse(configurationValue.XML, expectedType);
                            }
                        }
                        else
                        {
                            throw new InAnotherCastleInvalidConfigurationException(string.Format("Invalid Mode type, must be within the Enum ({0})", typeof(Mode).FullName));
                        }
                    }
                    return cacheResults;
                },
                //Important, if this is not called the section will remain cached by ASP.Net... in a way we do not need are own cache, but we do for fall back purposes
                (name, value) =>
                {
                    ConfigurationManager.RefreshSection(name);
                }
            );
        }

    }



}
