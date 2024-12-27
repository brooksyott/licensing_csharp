using Licensing.auth;
using Licensing.Common;

namespace Licensing.Auth
{
    public interface IInternalAuthKeyService
    {
        Task<ServiceResult<InternalAuthKeyEntity>> AddAuthKeyAsync(InternalAuthKeyEntity authKey);
        Task<ServiceResult<InternalAuthKeyEntity>> CreateAuthKeyAsync(InternalAuthKeyEntity internalAuthKey);
        Task<ServiceResult<InternalAuthKeyEntity>> DeleteByIdAsync(string authKeyId);
        Task<ServiceResult<InternalAuthKeyEntity>> DeleteByAuthKeyAsync(string authKey);
        Task<ServiceResult<InternalAuthKeyEntity>> GetByIdAsync(string authKeyId);
        Task<ServiceResult<InternalAuthKeyEntity>> GetByKeyAsync(string key);
        Task<ServiceResult<PaginatedResults>> GetInternalAuthKeysAsync(BasicQueryFilter filter);
        Task<ServiceResult<InternalAuthKeyEntity>> UpdateAuthKeyAsync(string id, UpdateInternalAuthKeyRequestBody authKey);
    }
}