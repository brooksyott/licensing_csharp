
using Licensing.Common;

namespace Licensing.License
{
    public interface ITokenService
    {
        Task<ServiceResult<PaginatedResults>> GetLicensesAsync(BasicQueryFilter filter);
        Task<ServiceResult<PaginatedResults>> GetByCustomerIdAsync(string customerId, BasicQueryFilter filter);
        Task<ServiceResult<LicenseEntity>> GetByIdAsync(string licenseId);

        Task<ServiceResult<LicenseEntity>> GenerateTokenAsync(GenerateLicenseRequestBody licenseRequest);
        Task<(bool, string)> ValidateJwt(string jwtToken);
    }
}