using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YourConfigIsInAnotherCastle.Models;

namespace $rootnamespace$.Models
{
    public class DTOConfigurationIncludingSchemaDetails
    {
        public string Name { get; set; }
        public string SystemName { get; set; }
        public List<int> TagIds { get; private set; }
        public string XML { get; set; }
        public string XMLSchema { get; set; }
        public string JSONSchema { get; set; }

        public int? Id { get; set; }
        public List<Tag> Tags { get;  set; }
    }
}
