using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YourConfigIsInAnotherCastle.Exceptions;
using System.Configuration;
using YourConfigIsInAnotherCastle.Storage.Implementations.EF;
using YourConfigIsInAnotherCastle.Logging;
using YourConfigIsInAnotherCastle.Caching;

namespace YourConfigIsInAnotherCastle.Test.Providers
{
    [TestClass]
    public class DefaultServiceProviderTests
    {

        [TestMethod]
        public void GetDefaultCache_DefaultCacheInstanceReturned()
        {
            DefaultServiceProvider provider = new DefaultServiceProvider();
            var results = provider.GetConfigCache();
            Assert.IsNotNull(results, "Should have default");
            Assert.IsInstanceOfType(results, typeof(DefaultConfigMemCache));

        }
        [TestMethod]
        public void GetDefaultDataStore_DefaultDataStoreInstanceReturned()
        {
            DefaultServiceProvider provider = new DefaultServiceProvider();

            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.ConnectionStrings.ConnectionStrings.Clear();
            //Ensure that connection is present
            config.ConnectionStrings.ConnectionStrings.Add(new ConnectionStringSettings(DefaultServiceProvider.ConnectionStringKey, "Server=LocalHost;Database=MyTests;Trusted_Connection=True;MultipleActiveResultSets=true;", "System.Data.SqlClient"));
            config.Save();
            ConfigurationManager.RefreshSection("connectionStrings");
            var results = provider.GetStorage();
            Assert.IsNotNull(results, "Should have default");
            Assert.IsInstanceOfType(results, typeof(ConfigurationStorage));
        }
        [TestMethod]
        [ExpectedException(typeof(InAnotherCastleInvalidConfigurationException))]
        public void GetDefaultDataStore_ConnectionStringNotInConfig_InAnotherCastleInvalidConfigurationThrown()
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.ConnectionStrings.ConnectionStrings.Clear();
            config.Save();
            ConfigurationManager.RefreshSection("connectionStrings");

            DefaultServiceProvider provider = new DefaultServiceProvider();
            var results = provider.GetStorage();

        }

        [TestMethod]
        public void GetDefaultLogger_DefaultLoggerInstanceReturned()
        {
            DefaultServiceProvider provider = new DefaultServiceProvider();
            var results = provider.GetLogger();
            Assert.IsNotNull(results, "Should have default");
            Assert.IsInstanceOfType(results, typeof(DefaultConsoleLogger));
        }
    }
}
