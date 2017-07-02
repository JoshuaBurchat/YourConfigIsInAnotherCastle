using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourConfigIsInAnotherCastle.Configuration;

namespace YourConfigIsInAnotherCastle.Test.Mocks
{
    public class MockInAnotherCastleConfigurationSection : IInAnotherCastleConfiguration
    {
        public string ServiceProviderTypeName { get; set; }
        public IInAnotherCastleServiceProvider ServiceProvider { get; set; }
    }
}
