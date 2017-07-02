using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Xml;

namespace YourConfigIsInAnotherCastle
{

    public enum Mode
    {
        Poco,
        Standard
    }
    public class InAnotherCastleConfigSectionRedirect : System.Configuration.ConfigurationSection, IRedirectIdentifier
    {
        [ConfigurationProperty("Mode", DefaultValue = "Poco", IsRequired = true)]
        public Mode Mode
        {
            get { return (Mode)Enum.Parse(typeof(Mode), this["Mode"].ToString()); }
            set { this["Mode"] = value.ToString(); }
        }
        [ConfigurationProperty("Name", DefaultValue = "", IsRequired = true)]
        public string Name
        {
            get { return (string)this["Name"]; }
            set { this["Name"] = value; }
        }

        [ConfigurationProperty("SystemName", IsRequired = false)]
        public string SystemName
        {
            get { return (string)this["SystemName"]; }
            set { this["SystemName"] = value; }
        }
        [ConfigurationProperty("Type", DefaultValue = "", IsRequired = true)]
        public string Type
        {
            get { return (string)this["Type"]; }
            set { this["Type"] = value; }
        }
        [ConfigurationProperty("CacheDurationInMinutes", DefaultValue = "1", IsRequired = false)]
        public int CacheDurationInMinutes
        {
            get { return (int)this["CacheDurationInMinutes"]; }
            set { this["CacheDurationInMinutes"] = value; }
        }
        [ConfigurationProperty("PocoBody")]
        public PocoBody PocoBody
        {
            get { return (PocoBody)this["PocoBody"]; }
            set { this["PocoBody"] = value; }
        }
        public void ExposedDeserializeSection(XmlReader reader)
        {
            base.DeserializeSection(reader);
        }
    }
}
