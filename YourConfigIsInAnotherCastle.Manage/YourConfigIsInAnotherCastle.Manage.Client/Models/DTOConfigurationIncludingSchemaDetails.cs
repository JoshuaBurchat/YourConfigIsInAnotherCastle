using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YourConfigIsInAnotherCastle.Models;

namespace YourConfigIsInAnotherCastle.Manage.Client.Models
{
    public class DTOConfigurationIncludingSchemaDetails
    {
        public string Name { get; set; }
        public string SystemName { get; set; }
        public string XML { get; set; }
        public string XMLSchema { get; set; }
        public string JSONSchema { get; set; }

        public int? Id { get; set; }
        public List<Tag> Tags { get;  set; }
    }
}
