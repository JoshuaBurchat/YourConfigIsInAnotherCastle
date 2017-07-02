using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YourConfigIsInAnotherCastle.Logging
{
    public interface ILogger
    {
        void LogError(string message, Exception exc);
        void LogInfo(string message);
        void LogTrace(string message);
    }
}
