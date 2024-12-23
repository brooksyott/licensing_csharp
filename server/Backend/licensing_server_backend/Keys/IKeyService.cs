using Licensing.Common;
using Licensing.Skus;

namespace Licensing.Keys
{
    public enum KeyType
    {
        Private,
        Public
    }

    public interface IKeyService
    {
        Task<ServiceResult<KeyEntity>> GenerateKeys(KeyGenerationRequestBody keyGenRequest);

        Task<ServiceResult<PaginatedResults>> GetKeysAsync(BasicQueryFilter filter);
        Task<ServiceResult<PaginatedResults>> GetByCustomerIdAsync(string customerId, BasicQueryFilter filter);
        Task<ServiceResult<KeyEntity>> GetByIdAsync(string keyId);

        Task<ServiceResult<byte[]>> DownloadPublicKeyAsync(string keyId);
        Task<ServiceResult<byte[]>> DownloadPrivateKeyAsync(string keyId);

        Task<ServiceResult<KeyEntity>> UpdateKeyAsync(string keyId, KeyUpdateRequestBody requestBody);

        Task<ServiceResult<KeyEntity>> DeleteKeyAsync(string keyId);

    }
}
