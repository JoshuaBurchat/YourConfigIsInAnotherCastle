using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using YourConfigIsInAnotherCastle.Models;

namespace YourConfigIsInAnotherCastle.Manage.Client.Models
{
    public class DTOConfigurationSaveResults
    {
        public bool Successful { get; set; }
        public IEnumerable<ErrorMap> Errors { get; set; }

        public DTOConfigurationValue Record { get; set; }

        public DTOConfigurationSaveResults()
        {
            Errors = new List<ErrorMap>();
        }
     
    }
}