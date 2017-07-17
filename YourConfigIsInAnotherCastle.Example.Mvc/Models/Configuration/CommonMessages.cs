using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace YourConfigIsInAnotherCastle.Example.Mvc.Models
{
    public class CommonMessages : ConfigurationSection, ICommonMessages
    {

        [ConfigurationProperty("ErrorSaving", DefaultValue = "", IsRequired = true)]
        public string ErrorSaving
        {
            get { return (string)this["ErrorSaving"]; }
            set { this["ErrorSaving"] = value; }
        }

        [ConfigurationProperty("UnauthorizedWarning", DefaultValue = "", IsRequired = true)]
        public string UnauthorizedWarning
        {
            get { return (string)this["UnauthorizedWarning"]; }
            set { this["UnauthorizedWarning"] = value; }
        }
        [ConfigurationProperty("UnsupportedFeature", DefaultValue = "", IsRequired = true)]
        public string UnsupportedFeature
        {
            get { return (string)this["UnsupportedFeature"]; }
            set { this["UnsupportedFeature"] = value; }
        }
        [ConfigurationProperty("Greeting", DefaultValue = "", IsRequired = true)]
        public string Greeting
        {
            get { return (string)this["Greeting"]; }
            set { this["Greeting"] = value; }
        }
    }
}