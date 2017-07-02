using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YourConfigIsInAnotherCastle
{
    public interface IRedirectIdentifier
    {
        Mode Mode { get; }
        string Name { get; }
        string SystemName { get; }
        string Type { get; }
        int CacheDurationInMinutes { get; }
        PocoBody PocoBody { get; }

    }
}