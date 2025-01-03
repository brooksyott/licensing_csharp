﻿
using Licensing.Common;

namespace Licensing.License
{
    public interface ILicenseService
    {
        // Create
        Task<ServiceResult<LicenseEntity>> GenerateLicenseAsync(GenerateLicenseRequestBody licenseRequest);

        // Read
        Task<ServiceResult<PaginatedResults>> GetLicensesAsync(BasicQueryFilter filter);
        Task<ServiceResult<PaginatedResults>> GetByCustomerIdAsync(string customerId, BasicQueryFilter filter);
        Task<ServiceResult<LicenseDetailsEntity>> GetByIdAsync(string licenseId);

        // Delete
        Task<ServiceResult<LicenseEntity>> DeleteLicenseAsync(string licenseId);
    }
}