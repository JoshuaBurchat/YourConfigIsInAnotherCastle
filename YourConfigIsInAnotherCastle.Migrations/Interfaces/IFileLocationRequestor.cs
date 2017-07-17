using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YourConfigIsInAnotherCastle.Migrations
{

    /// <summary>
    /// This interface is used to switch the responsiblity of accessing the file system, and how it will be done based on your application
    /// Note that migrations is not meant to run on a server, it should be done during local development
    /// </summary>
    public interface ILocationRequestor
    {
        bool UseWebConfiguration { get;  }
        string DestinationFolder { get; }
        string ConfigurationFilePath { get; }
        string ConnectionString { get; }

        //TODO, just a though, maybe the output should be a stream and the created file should be zipped... then
        //The output stream could be a web url, and we could add an HTTPHandler for it?
        string GetDestinationFolder();
        string GetConfigurationFilePath();
        string GetConnectionString();
    }
}
