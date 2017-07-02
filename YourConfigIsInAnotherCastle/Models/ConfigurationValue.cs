using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YourConfigIsInAnotherCastle.Models
{
    public class ConfigurationValue
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SystemName { get; set; }
        public virtual List<Tag> Tags { get; private set; }
        public string XML { get; set; }
        public string XMLSchema { get; set; }
        public string JSON { get; set; }
        public string JSONSchema { get; set; }
        public ConfigurationValue()
        {
            Tags = new List<Tag>();
        }
    }
}
