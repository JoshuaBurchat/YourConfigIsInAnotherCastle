using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YourConfigIsInAnotherCastle.Test.Mocks
{
   public class MockRedirectIdentifier : IRedirectIdentifier
    {
        public Mode Mode { get; set;  }
        public string Name { get; set; }
        public string SystemName { get; set; }

        public string Type { get; set; }

        public int CacheDurationInMinutes { get; set; }

        public PocoBody PocoBody { get; set; }
    }
}
