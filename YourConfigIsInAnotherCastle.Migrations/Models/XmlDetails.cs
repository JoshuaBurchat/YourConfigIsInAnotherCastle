using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;

namespace YourConfigIsInAnotherCastle.Migrations.Models
{
    public class XmlDetails
    {
        public string RawData { get;  set; }
        public string Schema { get;  set; }
        public XmlDocument Document
        {
            get
            {

                var document = new XmlDocument();
                document.LoadXml(this.RawData);
                StringReader reader = new StringReader(this.Schema);
                XmlSchema xmlSchema = XmlSchema.Read(reader, (s, e) => { });
                document.Schemas.Add(xmlSchema);
                return document;
            }
        }
    }
}
