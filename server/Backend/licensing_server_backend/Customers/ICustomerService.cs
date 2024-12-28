using Licensing.Common;

namespace Licensing.Customers
{
    public interface ICustomerService
    {
        // Create
        Task<ServiceResult<CustomerEntity>> AddCustomerAsync(CustomerEntity customer);

        // Read
        Task<ServiceResult<CustomerEntity>> GetByIdAsync(string customerId);
        Task<ServiceResult<CustomerEntity>> GetByNameAsync(string name);
        Task<ServiceResult<PaginatedResults>> GetCustomersAsync(BasicQueryFilter filter);

        // Update
        Task<ServiceResult<CustomerEntity>> UpdateCustomerAsync(string id, UpdateCustomerRequestBody customer);

        // Delete
        Task<ServiceResult<CustomerEntity>> DeleteByIdAsync(string customerId);
        Task<ServiceResult<CustomerEntity>> DeleteByNameAsync(string name);

    }
}