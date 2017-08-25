using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YourConfigIsInAnotherCastle.Models
{
    public class ConfigurationNew
    {
        public string Name { get; set; }
        public string SystemName { get; set; }
        public  List<Tag> Tags { get;  set; }
        public string XML { get; set; }
        public string XMLSchema { get; set; }
        public string JSONSchema { get; set; }
        public ConfigurationNew()
        {
            this.Tags = new List<Tag>();
        }
    }
}
