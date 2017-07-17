using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YourConfigIsInAnotherCastle.Example.Mvc.Models
{
    public interface ICommonMessages
    {
        string ErrorSaving { get; }
        string UnauthorizedWarning { get; }
        string UnsupportedFeature { get; }
        string Greeting { get; }
    }
}