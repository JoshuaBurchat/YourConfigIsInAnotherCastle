using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YourConfigIsInAnotherCastle.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public string Value { get; set; }


        public virtual List<ConfigurationValue> ConfigurationValues { get; private set; }

        public Tag()
        {
            ConfigurationValues = new List<ConfigurationValue>();
        }
    }
}
