using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YourConfigIsInAnotherCastle.Logging;
using YourConfigIsInAnotherCastle.Storage.Implementations.EF;
using System.Configuration;
using YourConfigIsInAnotherCastle.Caching;
using YourConfigIsInAnotherCastle.Exceptions;

namespace YourConfigIsInAnotherCastle
{
    public class DefaultServiceProvider : IInAnotherCastleServiceProvider
    {
        public const string ConnectionStringKey = "ConfigurationStorage";

        public void Dispose()
        {
        }

        public virtual Caching.IConfigCache GetConfigCache()
        {
            return new DefaultConfigMemCache();
        }

        public virtual Logging.ILogger GetLogger()
        {
            return new DefaultConsoleLogger();
        }

        public virtual Storage.IConfigurationStorage GetStorage()
        {
            var connectionString = ConfigurationManager.ConnectionStrings[DefaultServiceProvider.ConnectionStringKey];
            if (connectionString == null)
            {
                throw new InAnotherCastleInvalidConfigurationException(string.Format("Expected connection string under the name {0} for the default storage implementation", DefaultServiceProvider.ConnectionStringKey));
            }

            return new ConfigurationStorage(
                connectionString.ConnectionString,
                GetLogger()
            );
        }


    }
}
