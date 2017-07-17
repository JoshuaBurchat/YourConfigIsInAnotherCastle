using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YourConfigIsInAnotherCastle.Example.Mvc.Models
{
    public interface IFilePaths
    {
        string ServerTemporaryStoragePath { get; }
        string LoggingPath { get; }
        string HelpDocumentPath { get; }
    }
}