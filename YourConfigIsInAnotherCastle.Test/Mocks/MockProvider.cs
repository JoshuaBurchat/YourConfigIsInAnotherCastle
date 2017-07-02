using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourConfigIsInAnotherCastle.Caching;
using YourConfigIsInAnotherCastle.Logging;
using YourConfigIsInAnotherCastle.Models;
using YourConfigIsInAnotherCastle.Storage;

namespace YourConfigIsInAnotherCastle.Test.Mocks
{
    public partial class MockProvider : IInAnotherCastleServiceProvider
    {
        public IConfigCache GetConfigCacheValue { get; set; }
        public ILogger GetLoggerValue { get; set; }
        public IConfigurationStorage GetStorageValue { get; set; }
        public bool WasDisposed { get; set; }
        public MockProvider()
        {
            GetConfigCacheValue = new MockCache();
            GetLoggerValue = new MockLogger();
            GetStorageValue = new MockStorage();
        }
        public void Dispose()
        {
            WasDisposed = true;
        }

        public IConfigCache GetConfigCache()
        {
            return GetConfigCacheValue;
        }

        public ILogger GetLogger()
        {
            return GetLoggerValue;
        }

        public IConfigurationStorage GetStorage()
        {
            return GetStorageValue;
        }
        public class MockStorage : IConfigurationStorage
        {
            public Func<string, string, ConfigurationValue> GetConfigurationSectionFunc { get; set; }

            public ConfigurationSaveResults AddConfigurationSection(ConfigurationNew section)
            {
                throw new NotImplementedException();
            }

            public void Dispose()
            {
            }

            public ConfigurationValue GetConfigurationSection(string name, string systemName = null)
            {
                return GetConfigurationSectionFunc(name, systemName);
            }

            public ConfigurationValue GetConfigurationSection(int id)
            {
                throw new NotImplementedException();
            }

            public IQueryable<ConfigurationValue> GetConfigurationSections(IEnumerable<int> tags = null)
            {
                throw new NotImplementedException();
            }

            public IQueryable<ConfigurationValue> GetConfigurationSections(string systemName)
            {
                throw new NotImplementedException();
            }

            public ConfigurationSaveResults RemoveConfigurationSection(int id)
            {
                throw new NotImplementedException();
            }

            public ConfigurationSaveResults UpdateConfigurationSectionJson(ConfigurationUpdate update)
            {
                throw new NotImplementedException();
            }

            public ConfigurationSaveResults UpdateConfigurationSectionXml(ConfigurationUpdate update)
            {
                throw new NotImplementedException();
            }
        }
    }
}
