using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourConfigIsInAnotherCastle.Logging;

namespace YourConfigIsInAnotherCastle.Test.Mocks
{
    public class MockLogger : ILogger
    {

        public List<string> Errors = new List<string>();
        public List<string> Infos = new List<string>();
        public List<string> Traces = new List<string>();

        public void LogError(string message, Exception exc)
        {
            this.Errors.Add(message + ":" + exc.ToString());
        }

        public void LogInfo(string message)
        {
            this.Infos.Add(message);
        }

        public void LogTrace(string message)
        {
            this.Traces.Add(message);
        }
    }
}
