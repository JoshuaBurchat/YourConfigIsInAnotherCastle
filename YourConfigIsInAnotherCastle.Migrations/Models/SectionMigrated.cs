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
    public class SectionMigrated
    {
        public string SectionName { get; set; }

        public XmlDetails XmlDetails { get; set; }
        public JsonDetails JsonDetails { get; set; }
        public string SectionTypeName { get; internal set; }
    }
   

   

}
