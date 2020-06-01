using Intuit.Ipp.Core;
using Intuit.Ipp.Data;
using Intuit.Ipp.DataService;
using Intuit.Ipp.QueryFilter;
using Intuit.Ipp.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace QuickbookHandler.Model
{
    public class QbHelper
    {
        internal static ServiceContext GetServiceContext(ClaimsPrincipal principal, string realmId)
        {
            var oauthValidator = new OAuth2RequestValidator(principal.FindFirst("access_token").Value);

            // Create a ServiceContext with Auth tokens and realmId
            var serviceContext = new ServiceContext(realmId, IntuitServicesType.QBO, oauthValidator);
            serviceContext.IppConfiguration.MinorVersion.Qbo = "23";

            return serviceContext;
        }

        internal static Customer AddCustomer(ServiceContext serviceContext)
        {
            Customer customer = GetCustomer();
            Customer addedCustomer = Add(serviceContext, customer);
            return addedCustomer;
        }

        internal static Customer AddCustomer(ServiceContext serviceContext, CustomerModel cModel)
        {
            Customer customer = GetCustomer(cModel);
            Customer addedCustomer = Add(serviceContext, customer);
            return addedCustomer;
        }

        internal static string GetCompanyInfo(ServiceContext serviceContext)
        {
            // Create a QuickBooks QueryService using ServiceContext
            var querySvc = new QueryService<CompanyInfo>(serviceContext);
            CompanyInfo companyInfo = querySvc.ExecuteIdsQuery("SELECT * FROM CompanyInfo").FirstOrDefault();

            if (companyInfo != null)
            {
                string output = "Company Name: " + companyInfo.CompanyName + " Company Address: " + companyInfo.CompanyAddr.Line1 + ", " + companyInfo.CompanyAddr.City + ", " + companyInfo.CompanyAddr.Country + " " + companyInfo.CompanyAddr.PostalCode;
                return output;

            }

            return string.Empty;
        }

        private static string GetGuid()
        {
            return Guid.NewGuid().ToString("N");
        }

        private static Customer GetCustomer()
        {
            var guid = GetGuid();
            var customer = new Customer { Taxable = false, TaxableSpecified = true };

            var billAddress = new PhysicalAddress
            {
                Line1 = "Line1",
                Line2 = "Line2",
                Line3 = "Line3",
                Line4 = "Line4",
                Line5 = "Line5",
                City = "City",
                Country = "India",
                CountrySubDivisionCode = "CountrySubDivisionCode",
                PostalCode = "PostalCode"
            };
            customer.BillAddr = billAddress;

            var shipAddress = new PhysicalAddress
            {
                Line1 = "Line1",
                Line2 = "Line2",
                Line3 = "Line3",
                Line4 = "Line4",
                Line5 = "Line5",
                City = "City",
                Country = "USA",
                CountrySubDivisionCode = "CountrySubDivisionCode",
                PostalCode = "PostalCode"
            };
            customer.ShipAddr = shipAddress;

            var otherAddressList = new List<PhysicalAddress>();
            var otherAddress = new PhysicalAddress
            {
                Line1 = "Line1",
                Line2 = "Line2",
                Line3 = "Line3",
                Line4 = "Line4",
                Line5 = "Line5",
                City = "City",
                Country = "UK",
                CountrySubDivisionCode = "CountrySubDivisionCode",
                PostalCode = "PostalCode"
            };
            otherAddressList.Add(otherAddress);
            customer.OtherAddr = otherAddressList.ToArray();

            customer.Notes = "Notes";
            customer.Job = false;
            customer.JobSpecified = true;
            customer.BillWithParent = false;
            customer.BillWithParentSpecified = true;

            customer.Balance = new decimal(100.00);
            customer.BalanceSpecified = true;

            customer.BalanceWithJobs = new decimal(100.00);
            customer.BalanceWithJobsSpecified = true;

            customer.PreferredDeliveryMethod = "Print";
            customer.ResaleNum = "ResaleNum";

            customer.Title = "Title";
            customer.GivenName = "GivenName";
            customer.MiddleName = "MiddleName";
            customer.FamilyName = "FamilyName";
            customer.Suffix = "Suffix";
            customer.FullyQualifiedName = "Name_" + guid;
            customer.CompanyName = "CompanyName";
            customer.DisplayName = "Name_" + guid;
            customer.PrintOnCheckName = "PrintOnCheckName";

            customer.Active = true;
            customer.ActiveSpecified = true;

            var primaryPhone = new TelephoneNumber { FreeFormNumber = "8290487145" };
            customer.PrimaryPhone = primaryPhone;

            var alternatePhone = new TelephoneNumber { FreeFormNumber = "9166622789" };
            customer.AlternatePhone = alternatePhone;

            var mobile = new TelephoneNumber { FreeFormNumber = "9829478685" };
            customer.Mobile = mobile;

            var fax = new TelephoneNumber { FreeFormNumber = "0141-556677" };
            customer.Fax = fax;

            var primaryEmailAddress = new EmailAddress { Address = "xyz.vssaini@gmail.com" };
            customer.PrimaryEmailAddr = primaryEmailAddress;

            var websiteAddress = new WebSiteAddress { URI = "http://www.google.com" };
            customer.WebAddr = websiteAddress;

            return customer;
        }

        private static Customer GetCustomer(CustomerModel cModel)
        {
            var guid = GetGuid();
            var customer = new Customer { Taxable = false, TaxableSpecified = true };

            var billAddress = new PhysicalAddress
            {
                Line1 = "Line1",
                Line2 = "Line2",
                Line3 = "Line3",
                Line4 = "Line4",
                Line5 = "Line5",
                City = "City",
                Country = "India",
                CountrySubDivisionCode = "CountrySubDivisionCode",
                PostalCode = "PostalCode"
            };
            customer.BillAddr = billAddress;

            var shipAddress = new PhysicalAddress
            {
                Line1 = "Line1",
                Line2 = "Line2",
                Line3 = "Line3",
                Line4 = "Line4",
                Line5 = "Line5",
                City = "City",
                Country = "USA",
                CountrySubDivisionCode = "CountrySubDivisionCode",
                PostalCode = "PostalCode"
            };
            customer.ShipAddr = shipAddress;

            var otherAddressList = new List<PhysicalAddress>();
            var otherAddress = new PhysicalAddress
            {
                Line1 = "Line1",
                Line2 = "Line2",
                Line3 = "Line3",
                Line4 = "Line4",
                Line5 = "Line5",
                City = "City",
                Country = "UK",
                CountrySubDivisionCode = "CountrySubDivisionCode",
                PostalCode = "PostalCode"
            };
            otherAddressList.Add(otherAddress);
            customer.OtherAddr = otherAddressList.ToArray();

            customer.Notes = "Notes";
            customer.Job = false;
            customer.JobSpecified = true;
            customer.BillWithParent = false;
            customer.BillWithParentSpecified = true;

            customer.Balance = new decimal(100.00);
            customer.BalanceSpecified = true;

            customer.BalanceWithJobs = new decimal(100.00);
            customer.BalanceWithJobsSpecified = true;

            customer.PreferredDeliveryMethod = "Print";
            customer.ResaleNum = "ResaleNum";

            customer.Title = cModel.Title;
            customer.GivenName = cModel.GivenName;
            customer.MiddleName = cModel.MiddleName;
            customer.FamilyName = cModel.FamilyName;
            customer.Suffix = "Suffix";
            customer.FullyQualifiedName = $"{cModel.GivenName}_{cModel.MiddleName}_{cModel.FamilyName}_{guid}";
            customer.CompanyName = cModel.CompanyName;
            customer.DisplayName = $"{cModel.GivenName}_{cModel.MiddleName}_{cModel.FamilyName}_{guid}";
            customer.PrintOnCheckName = "PrintOnCheckName";

            customer.Active = true;
            customer.ActiveSpecified = true;

            var primaryPhone = new TelephoneNumber { FreeFormNumber = cModel.PrimaryPhone };
            customer.PrimaryPhone = primaryPhone;

            var alternatePhone = new TelephoneNumber { FreeFormNumber = "9166622789" };
            customer.AlternatePhone = alternatePhone;

            var mobile = new TelephoneNumber { FreeFormNumber = "9829478685" };
            customer.Mobile = mobile;

            var fax = new TelephoneNumber { FreeFormNumber = "0141-556677" };
            customer.Fax = fax;

            var primaryEmailAddress = new EmailAddress { Address = cModel.PrimaryEmailAddr };
            customer.PrimaryEmailAddr = primaryEmailAddress;

            var websiteAddress = new WebSiteAddress { URI = "http://www.google.com" };
            customer.WebAddr = websiteAddress;

            return customer;
        }

        private static T Add<T>(ServiceContext context, T entity) where T : IEntity
        {
            //Initializing the Dataservice object with ServiceContext
            var service = new DataService(context);

            var added = service.Add(entity);

            return added;
        }
    }
}