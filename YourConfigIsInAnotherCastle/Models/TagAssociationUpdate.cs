using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YourConfigIsInAnotherCastle.Models
{
 public   class TagAssociationUpdate
    {
        public int ConfigurationId { get; set; }
        public List<int> TagIds { get; private set; }
        public TagAssociationUpdate()
        {
            TagIds = new List<int>();
        }
    }
}
