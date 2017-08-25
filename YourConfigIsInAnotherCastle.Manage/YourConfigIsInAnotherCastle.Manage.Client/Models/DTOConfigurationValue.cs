using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YourConfigIsInAnotherCastle.Manage.Client.Models
{
    public class DTOConfigurationValue
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SystemName { get; set; }
        public virtual List<DTOTag> Tags { get; set; }
        public string XML { get; set; }
        public string XMLSchema { get; set; }
        public string JSON { get; set; }
        public string JSONSchema { get; set; }
    }
}
