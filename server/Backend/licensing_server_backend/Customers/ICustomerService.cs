using Licensing.Common;

namespace Licensing.Customers
{
    public interface ICustomerService
    {
        Task<ServiceResult<CustomerEntity>> AddCustomerAsync(CustomerEntity customer);
        Task<ServiceResult<CustomerEntity>> CreateCustomerAsync(CustomerEntity customer);
        Task<ServiceResult<CustomerEntity>> GetByIdAsync(string customerId);
        Task<ServiceResult<CustomerEntity>> GetByNameAsync(string name);
        Task<ServiceResult<PaginatedResults>> GetCustomersAsync(BasicQueryFilter filter);
        Task<ServiceResult<CustomerEntity>> UpdateCustomerAsync(string id, UpdateCustomerRequestBody customer);
        Task<ServiceResult<CustomerEntity>> DeleteByIdAsync(string customerId);
        Task<ServiceResult<CustomerEntity>> DeleteByNameAsync(string name);

    }
}