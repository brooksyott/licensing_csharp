using Licensing.auth;
using Licensing.Common;

namespace Licensing.Auth
{
    /// <summary>
    /// Service for managing internal auth keys
    /// </summary>
    public interface IInternalAuthKeyService
    {
        // Create
        Task<ServiceResult<InternalAuthKeyEntity>> AddAuthKeyAsync(InternalAuthKeyEntity authKey);

        // Read
        Task<ServiceResult<PaginatedResults>> GetInternalAuthKeysAsync(BasicQueryFilter filter);
        Task<ServiceResult<InternalAuthKeyEntity>> GetByIdAsync(string authKeyId);
        Task<ServiceResult<InternalAuthKeyEntity>> GetByKeyAsync(string key);

        // Update
        Task<ServiceResult<InternalAuthKeyEntity>> UpdateAuthKeyAsync(string id, UpdateInternalAuthKeyRequestBody authKey);

        // Delete
        Task<ServiceResult<InternalAuthKeyEntity>> DeleteByIdAsync(string authKeyId);
        Task<ServiceResult<InternalAuthKeyEntity>> DeleteByAuthKeyAsync(string authKey);
    }
}