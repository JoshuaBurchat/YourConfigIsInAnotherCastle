using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Xml;

namespace YourConfigIsInAnotherCastle.Configuration
{
    public partial class InAnotherCastleConfigSection  : ConfigurationSection, IInAnotherCastleConfiguration
    {

        [ConfigurationProperty("ServiceProviderType", DefaultValue = "", IsRequired = true)]
        public string ServiceProviderTypeName
        {
            get { return (string)this["ServiceProviderType"]; }
            set { this["ServiceProviderType"] = value; }
        }

        public IInAnotherCastleServiceProvider ServiceProvider { get; set; }

        public void ExposedDeserializeSection(XmlReader reader)
        {
            base.DeserializeSection(reader);
        }
    }
}
