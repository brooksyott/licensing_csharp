using Licensing.Data;
using Licensing.Keys;

using Licensing.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;
using Licensing.Skus;

namespace Licensing.Customers
{
    /// <summary>
    /// CustomerService class provides methods to manage customer data.
    /// </summary>
    public class CustomerService : BaseService<CustomerService>, ICustomerService
    {
        /// <summary>
        /// Constructor to initialize logger and context.
        /// </summary>
        /// <param name="logger">Logger instance.</param>
        /// <param name="context">Database context instance.</param>
        public CustomerService(ILogger<CustomerService> logger, LicensingContext context) : base(logger, context)
        {
        }

        /// <summary>
        /// Method to get a paginated list of customers based on the provided filter.
        /// </summary>
        /// <param name="filter">Filter containing pagination parameters.</param>
        /// <returns>Service result containing paginated customer data.</returns>
        public async Task<ServiceResult<PaginatedResults>> GetCustomersAsync(CustomerQueryFilter filter)
        {
            try
            {
                // Fetch customers from the database with pagination
                var customers = await _dbContext.Customers.Where(x => x.Visibility == filter.Visiblity).OrderBy(x => x.CreatedAt).Skip(filter.Offset).Take(filter.Limit).AsNoTracking().ToListAsync();
                if (customers == null)
                {
                    // Return an empty result if no customers are found
                    return new ServiceResult<PaginatedResults>()
                    {
                        Status = ResultStatusCode.Success,
                        Data = new PaginatedResults() { Limit = filter.Limit, Offset = filter.Offset, Results = new object[] { } }
                    };
                }
                // Return the list of customers
                return new ServiceResult<PaginatedResults>()
                {
                    Status = ResultStatusCode.Success,
                    Data = new PaginatedResults() { Limit = filter.Limit, Offset = filter.Offset, Count = customers.Count, Results = customers }
                };
            }
            catch (Exception ex)
            {
                // Handle exceptions and return an error result
                return ReturnException<PaginatedResults>(ex, $"Error getting customers");
            }
        }

        /// <summary>
        /// Method to get a customer by their ID.
        /// </summary>
        /// <param name="customerId">Customer ID.</param>
        /// <returns>Service result containing customer data.</returns>
        public async Task<ServiceResult<CustomerEntity>> GetByIdAsync(string customerId)
        {
            try
            {
                // Fetch the customer from the database by ID
                var customer = await _dbContext.Customers.Where(x => x.Id == customerId).AsNoTracking().SingleOrDefaultAsync();
                if (customer == null)
                {
                    // Return a not found result if the customer does not exist
                    return new ServiceResult<CustomerEntity>() { Status = ResultStatusCode.NotFound };
                }
                // Return the customer data
                return new ServiceResult<CustomerEntity>() { Status = ResultStatusCode.Success, Data = customer };
            }
            catch (Exception ex)
            {
                // Handle exceptions and return an error result
                return ReturnException<CustomerEntity>(ex, $"Error getting customer");
            }
        }

        /// <summary>
        /// Method to get a customer by their name.
        /// </summary>
        /// <param name="name">Customer name.</param>
        /// <returns>Service result containing customer data.</returns>
        public async Task<ServiceResult<CustomerEntity>> GetByNameAsync(string name)
        {
            try
            {
                // Fetch the customer from the database by name
                var customer = await _dbContext.Customers.Where(x => x.Name == name).AsNoTracking().SingleOrDefaultAsync();
                if (customer == null)
                {
                    // Return a not found result if the customer does not exist
                    return new ServiceResult<CustomerEntity>() { Status = ResultStatusCode.NotFound };
                }
                // Return the customer data
                return new ServiceResult<CustomerEntity>() { Status = ResultStatusCode.Success, Data = customer };
            }
            catch (Exception ex)
            {
                // Handle exceptions and return an error result
                return ReturnException<CustomerEntity>(ex, $"Error getting customer");
            }
        }


        /// <summary>
        /// Method to add a new customer (similar to CreateCustomerAsync).
        /// </summary>
        /// <param name="customer">Customer entity to be added.</param>
        /// <returns>Service result containing added customer data.</returns>
        public async Task<ServiceResult<CustomerEntity>> AddCustomerAsync(CustomerEntity customer)
        {
            try
            {
                // Add the new customer to the database
                var insCustomer = _dbContext.Customers.Add(customer);
                int addCount = await _dbContext.SaveChangesAsync();
                // Return the added customer data
                return new ServiceResult<CustomerEntity>() { Status = ResultStatusCode.Success, Data = insCustomer.Entity };
            }
            catch (Exception ex)
            {
                // Handle exceptions and return an error result
                return ReturnException<CustomerEntity>(ex);
            }
        }

        /// <summary>
        /// Method to update an existing customer.
        /// </summary>
        /// <param name="id">Customer ID.</param>
        /// <param name="customer">Customer entity with updated data.</param>
        /// <returns>Service result containing updated customer data.</returns>
        public async Task<ServiceResult<CustomerEntity>> UpdateCustomerAsync(string id, UpdateCustomerRequestBody customer)
        {
            try
            {
                // Fetch the existing customer from the database by ID
                var updatedCustomer = await _dbContext.Customers.Where(x => x.Id == id).AsNoTracking().SingleOrDefaultAsync();
                if (updatedCustomer == null)
                {
                    // Return a not found result if the customer does not exist
                    return new ServiceResult<CustomerEntity>() { Status = ResultStatusCode.NotFound };
                }

                // Update the customer data
                updatedCustomer.Name = customer.Name;
                updatedCustomer.Description = customer.Description;
                updatedCustomer.Visibility = customer.Visibility;
                _dbContext.Customers.Update(updatedCustomer);
                await _dbContext.SaveChangesAsync();
                // Return the updated customer data
                return new ServiceResult<CustomerEntity>() { Status = ResultStatusCode.Success, Data = updatedCustomer };
            }
            catch (Exception ex)
            {
                // Handle exceptions and return an error result
                return ReturnException<CustomerEntity>(ex, $"Error updating customer");
            }
        }

        public async Task<ServiceResult<CustomerEntity>> DeleteByIdAsync(string customerId)
        {
            try
            {
                var customer = await _dbContext.Customers.Where(x => x.Id == customerId).SingleOrDefaultAsync();
                if (customer == null)
                {
                    return new ServiceResult<CustomerEntity>() { Status = ResultStatusCode.NotFound };
                }
                var returedDeletedCustomer = _dbContext.Customers.Remove(customer);
                await _dbContext.SaveChangesAsync();
                return new ServiceResult<CustomerEntity>() { Status = ResultStatusCode.Success, Data = returedDeletedCustomer.Entity };
            }
            catch (Exception ex)
            {
                return ReturnException<CustomerEntity>(ex, $"Error deleting customer");

            }
        }

        public async Task<ServiceResult<CustomerEntity>> DeleteByNameAsync(string name)
        {
            try
            {
                var customer = await _dbContext.Customers.Where(x => x.Name == name).SingleOrDefaultAsync();
                if (customer == null)
                {
                    return new ServiceResult<CustomerEntity>() { Status = ResultStatusCode.NotFound };
                }
                var returedDeletedCustomer = _dbContext.Customers.Remove(customer);
                await _dbContext.SaveChangesAsync();
                return new ServiceResult<CustomerEntity>() { Status = ResultStatusCode.Success, Data = returedDeletedCustomer.Entity };
            }
            catch (Exception ex)
            {
                return ReturnException<CustomerEntity>(ex, $"Error deleting customer");
            }
        }
    }
}
