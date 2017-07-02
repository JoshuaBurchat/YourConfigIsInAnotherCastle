using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YourConfigIsInAnotherCastle.Storage;

namespace YourConfigIsInAnotherCastle.Models
{
    public class ErrorMap
    {
        public DataStorageError Code { get; set; }
        public string Message { get; set; }
    }
    public class ConfigurationSaveResults
    {
        public bool Successful { get { return !Errors.Any(); } }
        public List<ErrorMap> Errors { get; set; }

        public ConfigurationValue Record { get; set; }

        public ConfigurationSaveResults()
        {
            Errors = new List<ErrorMap>();
        }
        public ConfigurationSaveResults AddError(string message, DataStorageError code)
        {
            this.Errors.Add(new ErrorMap()
            {
                Code = code,
                Message = message
            });

            return this;
        }
    }
}
