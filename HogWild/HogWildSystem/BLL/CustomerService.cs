﻿#nullable disable
using HogWildSystem.DAL;
using DMIT2018.Paginator;
using HogWildSystem.ViewModels;

namespace HogWildSystem.BLL
{
    public  class CustomerService
    {
        #region Fields
        /// <summary>
        /// The hog wild context
        /// </summary>
        private readonly HogWildContext _hogWildContext;

        #endregion

        // Constructor for the CustomerService class.
        internal CustomerService(HogWildContext hogWildContext)
        {
            // Initialize the _hogWildContext field with the provided HogWildContext instance.
            _hogWildContext = hogWildContext;
        }

        public Task<PagedResult<CustomerSearchView>> GetCustomers(string lastName, string phone,
                                                        int page, int pageSize, string sortColumn,
                                                        string direction)
        {
            //	Business Rule
            //	Thees are processing rules that need to ve satisfied
            //		for valid data

            //	Rule: Both last name and phone number cannot e empty
            //	Rule: RemoveFromViewFlag must be false
            if (string.IsNullOrWhiteSpace(lastName) && string.IsNullOrWhiteSpace(phone))
            {
                throw new ArgumentNullException("Please provide either a last name and/or phone number");
            }

            //	Need to update parameters so we are not searching on an empty value,
            //		otherwise, as emtpy string will return all records
            if (string.IsNullOrWhiteSpace(lastName))
            {
                lastName = Guid.NewGuid().ToString();
            }

            if (string.IsNullOrWhiteSpace(phone))
            {
                phone = Guid.NewGuid().ToString();
            }

            return Task.FromResult(_hogWildContext.Customers
                .Where(x => (x.LastName.Contains(lastName)
                             || x.Phone.Contains(phone))
                            && !x.RemoveFromViewFlag)
                .Select(x => new CustomerSearchView
                {
                    CustomerID = x.CustomerID,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    City = x.City,
                    Phone = x.Phone,
                    Email = x.Email,
                    StatusID = x.StatusID,
                    TotalSales = x.Invoices.Sum(x => x.SubTotal + x.Tax)
                }).AsQueryable()
                .OrderBy(sortColumn, direction)
                .ToPagedResult(page, pageSize));
        }

        /// Get the customer.
        public CustomerEditView GetCustomer(int customerID)
        {
            //  Business Rules
            //	These are processing rules that need to be satisfied
            //		for valid data
            //		rule:	customerID must be valid 

            if (customerID == 0)
            {
                throw new ArgumentNullException("Please provide a customer");
            }

            return _hogWildContext.Customers
                .Where(x => (x.CustomerID == customerID
                             && x.RemoveFromViewFlag == false))
                .Select(x => new CustomerEditView
                {
                    CustomerID = x.CustomerID,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Address1 = x.Address1,
                    Address2 = x.Address2,
                    City = x.City,
                    ProvStateID = x.ProvStateID,
                    CountryID = x.CountryID,
                    PostalCode = x.PostalCode,
                    Phone = x.Phone,
                    Email = x.Email,
                    StatusID = x.StatusID,
                    RemoveFromViewFlag = x.RemoveFromViewFlag
                }).FirstOrDefault();
        }
    }
}
