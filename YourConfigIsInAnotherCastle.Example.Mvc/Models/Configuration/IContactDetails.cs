using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YourConfigIsInAnotherCastle.Example.Mvc.Models
{
    public interface IContactDetails
    {
        IContact [] Contacts { get; }

    }
    public interface IContact
    {
        string DepartmentName { get; }
        string EmailContact { get; }
        string PhoneNumber { get; }
    }
}