using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YourConfigIsInAnotherCastle.Configuration
{

    public interface IInAnotherCastleConfiguration
    {
        string ServiceProviderTypeName { get; set; }
        IInAnotherCastleServiceProvider ServiceProvider { get; set; }
    }
}
