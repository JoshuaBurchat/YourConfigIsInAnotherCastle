using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace YourConfigIsInAnotherCastle.Example.Mvc.Models
{
    public class FilePaths : ConfigurationSection, IFilePaths
    {

        [ConfigurationProperty("ServerTemporaryStoragePath", DefaultValue = "", IsRequired = true)]
        public string ServerTemporaryStoragePath
        {
            get { return (string)this["ServerTemporaryStoragePath"]; }
            set { this["ServerTemporaryStoragePath"] = value; }
        }

        [ConfigurationProperty("LoggingPath", DefaultValue = "", IsRequired = true)]
        public string LoggingPath
        {
            get { return (string)this["LoggingPath"]; }
            set { this["LoggingPath"] = value; }
        }
        [ConfigurationProperty("HelpDocumentPath", DefaultValue = "", IsRequired = true)]
        public string HelpDocumentPath
        {
            get { return (string)this["HelpDocumentPath"]; }
            set { this["HelpDocumentPath"] = value; }
        }

    }
}