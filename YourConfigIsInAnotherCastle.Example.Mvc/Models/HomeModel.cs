using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YourConfigIsInAnotherCastle.Example.Mvc.Models
{
    public class HomeModel
    {
        public IFilePaths FilePaths { get; set; }
        public ICommonMessages CommonMessages { get; set; }
        public IContactDetails ContactDetails { get; set; }
    }
}