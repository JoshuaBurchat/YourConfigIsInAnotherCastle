using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YourConfigIsInAnotherCastle.Manage.Client.Models
{
    public class DTOConfigurationUpdate
    {
        public int Id { get; set; }
        public string Value { get; set; }

        public FormatTypeId Format { get; set; }
    }
}
