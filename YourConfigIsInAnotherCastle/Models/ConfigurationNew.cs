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
        public  List<int> TagIds { get; private set; }
        public string XML { get; set; }
        public string XMLSchema { get; set; }
        public string JSONSchema { get; set; }
        public ConfigurationNew()
        {
            TagIds = new List<int>();
        }
    }
}
