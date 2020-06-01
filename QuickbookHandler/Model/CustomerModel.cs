using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuickbookHandler.Model
{
    public class CustomerModel
    {
        public string Title { get; set; }
        public string GivenName { get; set; }
        public string MiddleName { get; set; }
        public string FamilyName { get; set; }
        public string PrimaryEmailAddr { get; set; }
        public string PrimaryPhone { get; set; }
        public string CompanyName { get; set; }
    }
}