
using Licensing.Common;

namespace Licensing.License
{
    public interface ILicenseService
    {
        Task<ServiceResult<PaginatedResults>> GetLicensesAsync(BasicQueryFilter filter);
        Task<ServiceResult<PaginatedResults>> GetByCustomerIdAsync(string customerId, BasicQueryFilter filter);
        Task<ServiceResult<LicenseDetailsEntity>> GetByIdAsync(string licenseId);

        Task<ServiceResult<LicenseEntity>> GenerateLicenseAsync(GenerateLicenseRequestBody licenseRequest);
        Task<(bool, string)> ValidateJwt(string jwtToken);
    }
}