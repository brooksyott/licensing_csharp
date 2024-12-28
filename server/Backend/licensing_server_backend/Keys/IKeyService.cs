using Licensing.Common;
using Licensing.Skus;

namespace Licensing.Keys
{

    public interface IKeyService
    {
        // Create
        Task<ServiceResult<KeyEntity>> GenerateKeys(KeyGenerationRequestBody keyGenRequest);

        // Read
        Task<ServiceResult<PaginatedResults>> GetKeysAsync(BasicQueryFilter filter, bool redact);
        Task<ServiceResult<KeyEntity>> GetByIdAsync(string keyId, bool redact);

        // Read - Downloads the public and private keys in a format that can be saved to a file
        Task<ServiceResult<byte[]>> DownloadPublicKeyAsync(string keyId);
        Task<ServiceResult<byte[]>> DownloadPrivateKeyAsync(string? keyId);

        // Update
        Task<ServiceResult<KeyEntity>> UpdateKeyAsync(string keyId, KeyUpdateRequestBody requestBody, bool redact);

        // Delete
        Task<ServiceResult<KeyEntity>> DeleteKeyAsync(string keyId);
    }
}
