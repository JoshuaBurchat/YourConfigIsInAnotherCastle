using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YourConfigIsInAnotherCastle.Logging
{
    public class DefaultConsoleLogger : ILogger
    {
        public void LogError(string message, Exception exc)
        {
            Console.WriteLine("{0}: {1}, {2}", DateTime.Now, message, exc.ToString());
        }

        public void LogInfo(string message)
        {
            Console.WriteLine("{0}: {1}", DateTime.Now, message);
        }

        public void LogTrace(string message)
        {
            Console.WriteLine("{0}: {1}", DateTime.Now, message);
        }
    }
}
