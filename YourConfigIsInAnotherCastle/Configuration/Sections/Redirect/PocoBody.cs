using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Xml;

namespace YourConfigIsInAnotherCastle
{
    //TY v\https://stackoverflow.com/questions/887437/how-to-get-configuration-element

    public class PocoBody : System.Configuration.ConfigurationElement
    {
        public string Value { get;  set; }

        protected override void DeserializeElement(XmlReader reader, bool s)
        {
            Value = reader.ReadElementContentAs(typeof(string), null) as string;
        }
    }
}
