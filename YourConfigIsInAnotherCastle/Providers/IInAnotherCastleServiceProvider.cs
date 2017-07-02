using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YourConfigIsInAnotherCastle.Caching;
using YourConfigIsInAnotherCastle.Logging;
using YourConfigIsInAnotherCastle.Storage;

namespace YourConfigIsInAnotherCastle
{
    public interface IInAnotherCastleServiceProvider : IDisposable
    {
        IConfigCache GetConfigCache();
        ILogger GetLogger();
        IConfigurationStorage GetStorage();
    }
}
