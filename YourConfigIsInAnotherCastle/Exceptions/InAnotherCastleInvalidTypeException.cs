using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YourConfigIsInAnotherCastle.Exceptions
{
  public  class InAnotherCastleInvalidTypeException : Exception
    {
        public InAnotherCastleInvalidTypeException(string message, TypeLoadException innerException) : base(message, innerException)
        {

        }
    }
}
