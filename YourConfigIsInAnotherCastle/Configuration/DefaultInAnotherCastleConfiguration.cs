using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YourConfigIsInAnotherCastle.Configuration
{

    public class DefaultInAnotherCastleConfiguration : IInAnotherCastleConfiguration
    {
        public string ServiceProviderTypeName { get; set; }
        public IInAnotherCastleServiceProvider ServiceProvider { get; set; }

        public DefaultInAnotherCastleConfiguration()
        {
            ServiceProvider = new DefaultServiceProvider();
        }

    }
}
