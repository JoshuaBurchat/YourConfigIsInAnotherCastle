using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YourConfigIsInAnotherCastle.Example.Mvc.Models;

namespace YourConfigIsInAnotherCastle.Example.Mvc.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            HomeModel model = new HomeModel()
            {
                CommonMessages = (ICommonMessages)ConfigurationManager.GetSection("CommonMessages"),
                ContactDetails = (IContactDetails)ConfigurationManager.GetSection("ContactDetails"),
                FilePaths = (IFilePaths)ConfigurationManager.GetSection("FilePaths"),
            };
            
            return View(model);
        }
    }
}