using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourConfigIsInAnotherCastle.Configuration;
using YourConfigIsInAnotherCastle.Caching;
using YourConfigIsInAnotherCastle.Logging;
using YourConfigIsInAnotherCastle.Storage;
using YourConfigIsInAnotherCastle.Exceptions;
using YourConfigIsInAnotherCastle.Parser;
using YourConfigIsInAnotherCastle.Models;
using System.Xml;
using YourConfigIsInAnotherCastle.Test.Mocks;

namespace YourConfigIsInAnotherCastle.Test.Configuration

{
    /// <summary>
    /// Theses tests focus on the flow and behaviour of the handler not that the results are correct based on XML
    /// The parser would be responsible for the parsing tests
    /// </summary>
    [TestClass]
    public class InAnotherCastleHandlerTests
    {
        #region TestImplementations
        public class TestConfiguration : IInAnotherCastleConfiguration
        {
            public string ServiceProviderTypeName { get; set; }
            public IInAnotherCastleServiceProvider ServiceProvider { get; set; }
        }
        public class TestParser : IInAnotherCastleParser
        {
            public Func<string, IRedirectIdentifier> ParseRedirectDetailsValue { get; set; }
            public Func<string, Type, object> POCOConfigSectionParseValue { get; set; }
            public Func<string, Type, object> StandardConfigSectionParseValue { get; set; }
            public IRedirectIdentifier ParseRedirectDetails(string xml)
            {
                return ParseRedirectDetailsValue(xml);
            }

            public object POCOConfigSectionParse(string xml, Type expectedType)
            {
                return POCOConfigSectionParseValue(xml, expectedType);
            }

            public object StandardConfigSectionParse(string xml, Type expectedType)
            {
                return StandardConfigSectionParseValue(xml, expectedType);
            }
        }

       

        public class TestResultObject
        {
            public string SomeValueField { get; set; }
        }

        #endregion TestImplementations


        private void SetConfigurationSection(string typeName)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.Sections.Clear();
            config.Sections.Add(InAnotherCastleHandler.DefaultConfigurationSectionName, new InAnotherCastleConfigSection()
            {
                ServiceProviderTypeName = typeName
            });
            //Ensure that connection is present
            config.ConnectionStrings.ConnectionStrings.Add(new ConnectionStringSettings(DefaultServiceProvider.ConnectionStringKey, "Server=LocalHost;Database=MyTests;Trusted_Connection=True;MultipleActiveResultSets=true;", "System.Data.SqlClient"));
            config.Save();
            ConfigurationManager.RefreshSection(InAnotherCastleHandler.DefaultConfigurationSectionName);
        }
        private void ClearConfiguration()
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.Sections.Clear();
            config.Save();
            ConfigurationManager.RefreshSection(InAnotherCastleHandler.DefaultConfigurationSectionName);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ConfigHandler_ConfigProvider_NoType_ExceptionThrown()
        {
            SetConfigurationSection(null);//Null for typed
            InAnotherCastleHandler.RefreshConfiguration();
        }


        [TestMethod]
        public void ConfigHandler_ConfigProvider_ProperType_InstanceCreated()
        {
            var type = typeof(MockProvider);
            SetConfigurationSection(type.AssemblyQualifiedName);
            InAnotherCastleHandler.RefreshConfiguration();
            Assert.AreEqual(InAnotherCastleHandler.Configuration.ServiceProviderTypeName, type.AssemblyQualifiedName, "Name should be the same as what was given from config");
            Assert.IsInstanceOfType(InAnotherCastleHandler.Configuration.ServiceProvider, type, "Should be of the type passed in");
        }

        [TestMethod]
        [ExpectedException(typeof(InAnotherCastleInvalidTypeException))]
        public void ConfigHandler_ConfigProvider_InvalidTypeName_ExceptionThrown()
        {
            SetConfigurationSection("Invalid Type Name here");
            InAnotherCastleHandler.RefreshConfiguration();

        }


        [TestMethod]
        public void ConfigHandler_ConfigProviderMissing_DefaultConfigurationUsed()
        {
            ClearConfiguration();
            InAnotherCastleHandler.RefreshConfiguration();
            Assert.IsInstanceOfType(InAnotherCastleHandler.Configuration.ServiceProvider, typeof(DefaultServiceProvider), "Should be of the type default packaged with this library");
        }
        [TestMethod]
        public void ConfigHandler_SetConfigWithoutInstance_TypeName_InstanceCreated()
        {
            var type = typeof(MockProvider);
            InAnotherCastleHandler.Configuration = new TestConfiguration()
            {
                ServiceProviderTypeName = type.AssemblyQualifiedName
            };
            Assert.AreEqual(InAnotherCastleHandler.Configuration.ServiceProviderTypeName, type.AssemblyQualifiedName, "Name should be the same as what was given from config");
            Assert.IsInstanceOfType(InAnotherCastleHandler.Configuration.ServiceProvider, type, "Should be of the type passed in");
        }
        [TestMethod]
        [ExpectedException(typeof(InAnotherCastleInvalidTypeException))]
        public void ConfigHandler_SetConfigWithoutInstance_InvalidTypeName_ExceptionThrown()
        {
            InAnotherCastleHandler.Configuration = new TestConfiguration()
            {
                ServiceProviderTypeName = "Invalid Type Name here"
            };
        }

        [TestMethod]
        public void ConfigHandler_SetConfigWithInstanceNoType_TypeSetNoErrorInstanceSame()
        {
            var type = typeof(MockProvider);
            InAnotherCastleHandler.Configuration = new TestConfiguration()
            {
                ServiceProvider = new MockProvider()
            };
            Assert.AreEqual(InAnotherCastleHandler.Configuration.ServiceProviderTypeName, type.AssemblyQualifiedName, "Name should be the same as what was given from the instance");
            Assert.IsInstanceOfType(InAnotherCastleHandler.Configuration.ServiceProvider, type, "Should be assigned the type that was passed in");

        }

        [TestMethod]
        public void ConfigHandler_ResetConfigWithInstanceExternal_PreviousInstanceNotDisposed()
        {
            var first = new MockProvider();
            var second = new MockProvider();
            InAnotherCastleHandler.Configuration = new TestConfiguration()
            {
                ServiceProvider = first
            };
            InAnotherCastleHandler.Configuration = new TestConfiguration()
            {
                ServiceProvider = second
            };

            Assert.IsFalse(first.WasDisposed, "Replaced providers should not be be disposed if they were created outside");
        }
        [TestMethod]
        public void ConfigHandler_ResetConfigWithInstanceFromConfig_PreviousInstanceDisposed()
        {
            var type = typeof(MockProvider);
            SetConfigurationSection(type.AssemblyQualifiedName);
            InAnotherCastleHandler.RefreshConfiguration();

            var first = (MockProvider)InAnotherCastleHandler.Configuration.ServiceProvider;
            var second = new MockProvider();
            InAnotherCastleHandler.Configuration = new TestConfiguration()
            {
                ServiceProvider = second
            };

            Assert.IsTrue(first.WasDisposed, "Replaced providers should be disposed");
        }



        [TestMethod]
        public void ConfigHandler_SetConfigWhenNull_ConfigIsSet()
        {
            var type = typeof(MockProvider);
            SetConfigurationSection(type.AssemblyQualifiedName);
            InAnotherCastleHandler.ClearConfig();
            InAnotherCastleHandler handler = new InAnotherCastleHandler();

            var parser = new TestParser();
            handler.Parser = parser;

            XmlDocument document = new XmlDocument();
            document.LoadXml("<Test></Test>");

            var resultingObject = handler.Create(null, null, document.FirstChild);
            Assert.AreEqual(InAnotherCastleHandler.Configuration.ServiceProviderTypeName, type.AssemblyQualifiedName, "Name should be the same as what was given from config");
            Assert.IsInstanceOfType(InAnotherCastleHandler.Configuration.ServiceProvider, type, "Should be of the type passed in");

            Assert.IsNull(resultingObject, "The default test cache return is empty");
        }

        [TestMethod]
        public void ConfigHandler_Standard_ProperNamesPassedToStorageMockObjectsReturned()
        {
            InAnotherCastleHandler handler = new InAnotherCastleHandler();

            object expectedObject = new TestResultObject();
            string expectedXml = "<Test></Test>";
            string expectedName = "TestName";
            string expectedSystemName = "SystemName";
            int expectedCachDuration = 919191;


            //Setup mock components used by handler
            var type = typeof(MockProvider);
            SetConfigurationSection(type.AssemblyQualifiedName);
            InAnotherCastleHandler.RefreshConfiguration();
            //Allow the cache function to execute the get section functionality within the handler
            var mockProvider = (MockProvider)InAnotherCastleHandler.Configuration.ServiceProvider;
            var testCach = (MockProvider.MockCache)(mockProvider).GetConfigCacheValue;
            testCach.ExecuteCache = true;
            var mockStorage = (MockProvider.MockStorage)mockProvider.GetStorageValue;
            string passedName = null, passedSystemName = null, passedXml = null;

            mockStorage.GetConfigurationSectionFunc = (n, s) =>
            {
                passedName = n;
                passedSystemName = s;
                return new ConfigurationValue() { XML = expectedXml };
            };
            handler.Parser = new TestParser()
            {
                ParseRedirectDetailsValue = (xml) => new MockRedirectIdentifier()
                {
                    Type = typeof(TestResultObject).AssemblyQualifiedName,
                    Name = expectedName,
                    SystemName = expectedSystemName,
                    Mode = Mode.Standard,
                    CacheDurationInMinutes = expectedCachDuration
                },
                StandardConfigSectionParseValue = (config, t) =>
                {
                    passedXml = config;
                    return expectedObject;
                }
            };

            XmlDocument document = new XmlDocument();
            document.LoadXml("<Test></Test>");

            var resultingObject = handler.Create(null, null, document.FirstChild);

            Assert.AreEqual(resultingObject, expectedObject, "Object returned should be the expected as it has been mocked in the pipeline");
            Assert.AreEqual(passedName, expectedName, "Name is expected based on mock");
            Assert.AreEqual(passedSystemName, expectedSystemName, "System Name is expected based on mock");
            Assert.AreEqual(expectedXml, passedXml, "Config is expected to be based on the mock");
        }
        [TestMethod]
        [ExpectedException(typeof(InAnotherCastleInvalidConfigurationException))]
        public void ConfigHandler_IvalidModeType_ExceptionExpected()
        {
            InAnotherCastleHandler handler = new InAnotherCastleHandler();

            var type = typeof(MockProvider);
            SetConfigurationSection(type.AssemblyQualifiedName);
            InAnotherCastleHandler.RefreshConfiguration();
            var mockProvider = (MockProvider)InAnotherCastleHandler.Configuration.ServiceProvider;
            var testCach = (MockProvider.MockCache)(mockProvider).GetConfigCacheValue;
            testCach.ExecuteCache = true;

            //Allow the cache function to execute the get section functionality within the handler

            handler.Parser = new TestParser()
            {
                ParseRedirectDetailsValue = (xml) => new MockRedirectIdentifier()
                {
                    Type = typeof(object).AssemblyQualifiedName,
                    Mode = (Mode)11111,// Invalid mode, in the file this may be some invalid text
                },
            };

            XmlDocument document = new XmlDocument();
            document.LoadXml("<Test></Test>");

            var resultingObject = handler.Create(null, null, document.FirstChild);

        }
        [TestMethod]
        public void ConfigHandler_Poco_FromDataStore_ShouldCallIntoParserWithXmlValue()
        {
            InAnotherCastleHandler handler = new InAnotherCastleHandler();

            object expectedObject = new TestResultObject();

            string expectedName = "TestName";
            string expectedSystemName = "SystemName";
            string expectedXml = "<TestPassedIn></TestPassedIn>";
            int expectedCachDuration = 919191;
            ConfigurationValue expectedConfigurationValue = new ConfigurationValue()
            {
                XML = expectedXml
            };

            //Setup mock components used by handler
            var type = typeof(MockProvider);
            SetConfigurationSection(type.AssemblyQualifiedName);
            InAnotherCastleHandler.RefreshConfiguration();
            //Allow the cache function to execute the get section functionality within the handler
            var mockProvider = (MockProvider)InAnotherCastleHandler.Configuration.ServiceProvider;
            var testCach = (MockProvider.MockCache)(mockProvider).GetConfigCacheValue;
            testCach.ExecuteCache = true;
            var mockStorage = (MockProvider.MockStorage)mockProvider.GetStorageValue;
            string resultingName = null, resultingSystemName = null, resultingXml = null;

            mockStorage.GetConfigurationSectionFunc = (n, s) =>
            {
                resultingName = n;
                resultingSystemName = s;
                return expectedConfigurationValue;
            };
            handler.Parser = new TestParser()
            {
                ParseRedirectDetailsValue = (xml) => new MockRedirectIdentifier()
                {
                    Type = typeof(TestResultObject).AssemblyQualifiedName,
                    Name = expectedName,
                    SystemName = expectedSystemName,
                    Mode = Mode.Poco,
                    CacheDurationInMinutes = expectedCachDuration
                },
                POCOConfigSectionParseValue = (xml, t) =>
                {
                    resultingXml = xml;
                    return expectedObject;
                }
            };

            XmlDocument document = new XmlDocument();
            document.LoadXml("<Test></Test>");
            var resultingObject = handler.Create(null, null, document.FirstChild);

            Assert.AreEqual(resultingObject, expectedObject, "Object returned should be the expected as it has been mocked in the pipeline");
            Assert.AreEqual(resultingName, expectedName, "Name is expected based on mock");
            Assert.AreEqual(resultingSystemName, expectedSystemName, "System Name is expected based on mock");
            Assert.AreEqual(resultingXml, expectedXml, "XML is expected to be based on the mock");
        }
        [TestMethod]
        public void ConfigHandler_Poco_FromRedirectBody_ShouldCallIntoParserWithXmlValue()
        {
            InAnotherCastleHandler handler = new InAnotherCastleHandler();

            object expectedObject = new TestResultObject();

            string expectedXml = "<TestPassedIn></TestPassedIn>";
            int expectedCachDuration = 919191;
          

            //Setup mock components used by handler
            var type = typeof(MockProvider);
            SetConfigurationSection(type.AssemblyQualifiedName);
            InAnotherCastleHandler.RefreshConfiguration();
            //Allow the cache function to execute the get section functionality within the handler
            var mockProvider = (MockProvider)InAnotherCastleHandler.Configuration.ServiceProvider;
            var testCach = (MockProvider.MockCache)(mockProvider).GetConfigCacheValue;
            testCach.ExecuteCache = true;
            var mockStorage = (MockProvider.MockStorage)mockProvider.GetStorageValue;

            mockStorage.GetConfigurationSectionFunc = (n, s) =>
            {
                Assert.Fail("There should be no reason for the handler to use the storage function if the POCO body is set in the parser.");
                return null;
            };

            string resultingXml = null;

            handler.Parser = new TestParser()
            {
                ParseRedirectDetailsValue = (xml) => new MockRedirectIdentifier()
                {
                    Type = typeof(TestResultObject).AssemblyQualifiedName,
                    Mode = Mode.Poco,
                    CacheDurationInMinutes = expectedCachDuration,
                    //Note poco body is populated so it should be used instead of a db call
                    PocoBody = new PocoBody()
                    {
                        Value = expectedXml
                    }
                },
                POCOConfigSectionParseValue = (xml, t) =>
                {
                    resultingXml = xml;
                    return expectedObject;
                }
            };

            XmlDocument document = new XmlDocument();
            document.LoadXml("<TestSectionName></TestSectionName>");
            var resultingObject = handler.Create(null, null, document.FirstChild);

            Assert.AreEqual(resultingObject, expectedObject, "Object returned should be the expected as it has been mocked in the pipeline");
            Assert.AreEqual(resultingXml, expectedXml, "XML is expected to be based on the mock");
        }

    }
}
