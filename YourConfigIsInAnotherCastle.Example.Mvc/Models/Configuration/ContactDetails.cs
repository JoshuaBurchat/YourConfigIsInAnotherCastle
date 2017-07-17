using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace YourConfigIsInAnotherCastle.Example.Mvc.Models
{
    public class ContactDetails : ConfigurationSection, IContactDetails
    {
        public IContact[] Contacts
        {
            get
            {
                return ContactElements.ToArray();
            }
        }
        [ConfigurationProperty("", IsRequired = true, IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(ContactDetailsCollection), AddItemName = "Contact")]
        public ContactDetailsCollection ContactElements
        {
            get { return (ContactDetailsCollection)this[""]; }
        }
    }
    public class ContactDetailsCollection : ConfigurationElementCollection, IEnumerable<Contact>
    {

        private readonly List<Contact> _elements;

        public ContactDetailsCollection()
        {
            this._elements = new List<Contact>();
        }

        protected override ConfigurationElement CreateNewElement()
        {
            var element = new Contact();
            this._elements.Add(element);
            return element;
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((Contact)element).DepartmentName;
        }

        public new IEnumerator<Contact> GetEnumerator()
        {
            return this._elements.GetEnumerator();
        }

       
    }
    public class Contact : ConfigurationElement, IContact
    {

        [ConfigurationProperty("DepartmentName", DefaultValue = "", IsRequired = true)]
        public string DepartmentName
        {
            get { return (string)this["DepartmentName"]; }
            set { this["DepartmentName"] = value; }
        }
        [ConfigurationProperty("EmailContact", DefaultValue = "", IsRequired = true)]
        public string EmailContact
        {
            get { return (string)this["EmailContact"]; }
            set { this["EmailContact"] = value; }
        }
        [ConfigurationProperty("PhoneNumber", DefaultValue = "", IsRequired = true)]
        public string PhoneNumber
        {
            get { return (string)this["PhoneNumber"]; }
            set { this["PhoneNumber"] = value; }
        }
    }

}