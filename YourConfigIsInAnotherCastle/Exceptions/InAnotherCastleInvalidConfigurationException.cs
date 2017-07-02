using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YourConfigIsInAnotherCastle.Exceptions
{
    public class InAnotherCastleInvalidConfigurationException : Exception
    {
        public InAnotherCastleInvalidConfigurationException(string message) : base(message)
        {

        }
    }
}
