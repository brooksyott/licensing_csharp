using Licensing.Common;
using Licensing.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Licensing.Skus;

namespace Licensing.Keys
{
    /// <summary>
    /// Service for managing keys.
    /// </summary>
    public class KeyService : BaseService<KeyService>, IKeyService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KeyService"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="context">The database context.</param>
        public KeyService(ILogger<KeyService> logger, LicensingContext context) : base(logger, context) { }

        /// <summary>
        /// Generates a new key pair.
        /// </summary>
        /// <param name="keyGenRequest">The key generation request body.</param>
        /// <returns>Service result containing the generated key entity.</returns>
        public async Task<ServiceResult<KeyEntity>> GenerateKeys(KeyGenerationRequestBody keyGenRequest)
        {
            if (keyGenRequest == null || !keyGenRequest.IsValid())
            {
                return new ServiceResult<KeyEntity>()
                {
                    Status = ResultStatusCode.BadRequest,
                    ErrorMessage = new ErrorInformation("Invalid request body")
                };
            }

            try
            {
                using (RSA rsa = RSA.Create(2048))
                {
                    // Generate PEM format private and public keys
                    string privateKeyPem = PemUtils.ExportPrivateKey(rsa);
                    string publicKeyPem = PemUtils.ExportPublicKey(rsa);

                    // Create response content as files
                    var result = new
                    {
                        PrivateKey = privateKeyPem,
                        PublicKey = publicKeyPem
                    };

                    KeyEntity certs = new KeyEntity()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Label = keyGenRequest.Label,
                        CreatedBy = keyGenRequest.CreatedBy,
                        UpdatedBy = keyGenRequest.UpdatedBy,
                        PrivateKey = result.PrivateKey,
                        PublicKey = result.PublicKey,
                        Description = keyGenRequest.Description
                    };

                    _dbContext.Keys.Add(certs);
                    await _dbContext.SaveChangesAsync();

                    return new ServiceResult<KeyEntity>()
                    {
                        Status = ResultStatusCode.Success,
                        Data = certs
                    };
                }
            }
            catch (Exception ex)
            {
                return ReturnException<KeyEntity>(ex, $"Error generating certificate information by Id");
            }
        }

        /// <summary>
        /// Fetches a paginated list of keys.
        /// </summary>
        /// <param name="filter">Pagination filter.</param>
        /// <param name="redact">Indicates whether to redact the private keys.</param>
        /// <returns>Service result containing paginated list of keys.</returns>
        public async Task<ServiceResult<PaginatedResults>> GetKeysAsync(BasicQueryFilter filter, bool redact = true)
        {
            try
            {
                var keys = await _dbContext.Keys.OrderBy(x => x.CreatedAt).Skip(filter.Offset).Take(filter.Limit).AsNoTracking().ToListAsync();
                if (keys == null)
                {
                    return new ServiceResult<PaginatedResults>()
                    {
                        Status = ResultStatusCode.Success,
                        Data = new PaginatedResults() { Limit = filter.Limit, Offset = filter.Offset, Results = new object[] { } }
                    };
                }

                if (redact)
                {
                    foreach (var key in keys)
                    {
                        key.PrivateKey = KeyEntity.Redact;
                    }
                }

                return new ServiceResult<PaginatedResults>()
                {
                    Status = ResultStatusCode.Success,
                    Data = new PaginatedResults() { Limit = filter.Limit, Offset = filter.Offset, Count = keys.Count, Results = keys }
                };
            }
            catch (Exception ex)
            {
                return ReturnException<PaginatedResults>(ex, $"Error getting keys");
            }
        }

        /// <summary>
        /// Fetches a key by ID.
        /// </summary>
        /// <param name="keyId">The key ID.</param>
        /// <param name="redact">Indicates whether to redact the private key.</param>
        /// <returns>Service result containing the key.</returns>
        public async Task<ServiceResult<KeyEntity>> GetByIdAsync(string keyId, bool redact)
        {
            try
            {
                var key = await _dbContext.Keys.Where(x => x.Id == keyId).AsNoTracking().SingleOrDefaultAsync();
                if (key == null)
                {
                    return new ServiceResult<KeyEntity>() { Status = ResultStatusCode.NotFound };
                }

                if (redact)
                {
                    key.PrivateKey = KeyEntity.Redact;
                }

                return new ServiceResult<KeyEntity>() { Status = ResultStatusCode.Success, Data = key };
            }
            catch (Exception ex)
            {
                return ReturnException<KeyEntity>(ex, $"Error getting certificate information by Id");
            }
        }

        /// <summary>
        /// Downloads the public key by key ID.
        /// </summary>
        /// <param name="keyId">The key ID.</param>
        /// <returns>Service result containing the public key as a byte array.</returns>
        public async Task<ServiceResult<byte[]>> DownloadPublicKeyAsync(string keyId)
        {
            try
            {
                var certInfo = await _dbContext.Keys.Where(x => x.Id == keyId).AsNoTracking().SingleOrDefaultAsync();
                if ((certInfo == null) || (string.IsNullOrWhiteSpace(certInfo.PublicKey)))
                {
                    return new ServiceResult<byte[]>() { Status = ResultStatusCode.NotFound };
                }
                return new ServiceResult<byte[]>() { Status = ResultStatusCode.Success, Data = Encoding.UTF8.GetBytes(certInfo.PublicKey) };
            }
            catch (Exception ex)
            {
                return ReturnException<byte[]>(ex, $"Error getting certificate information by Id");
            }
        }

        /// <summary>
        /// Downloads the private key by key ID.
        /// </summary>
        /// <param name="keyId">The key ID.</param>
        /// <returns>Service result containing the private key as a byte array.</returns>
        public async Task<ServiceResult<byte[]>> DownloadPrivateKeyAsync(string? keyId)
        {
            try
            {
                var certInfo = await _dbContext.Keys.Where(x => x.Id == keyId).AsNoTracking().SingleOrDefaultAsync();
                if ((certInfo == null) || (string.IsNullOrWhiteSpace(certInfo.PrivateKey)))
                {
                    return new ServiceResult<byte[]>() { Status = ResultStatusCode.NotFound };
                }
                return new ServiceResult<byte[]>() { Status = ResultStatusCode.Success, Data = Encoding.UTF8.GetBytes(certInfo.PrivateKey) };
            }
            catch (Exception ex)
            {
                return ReturnException<byte[]>(ex, $"Error getting certificate information by Id");
            }
        }

        /// <summary>
        /// Updates an existing key.
        /// </summary>
        /// <param name="keyId">The key ID.</param>
        /// <param name="requestBody">The key update request body.</param>
        /// <param name="redact">Indicates whether to redact the private key.</param>
        /// <returns>Service result containing the updated key entity.</returns>
        public async Task<ServiceResult<KeyEntity>> UpdateKeyAsync(string keyId, KeyUpdateRequestBody requestBody, bool redact)
        {
            if (requestBody == null || !requestBody.IsValid())
            {
                return new ServiceResult<KeyEntity>()
                {
                    Status = ResultStatusCode.BadRequest,
                    ErrorMessage = new ErrorInformation("Invalid request body")
                };
            }

            try
            {
                var updatedKey = await _dbContext.Keys.Where(x => x.Id == keyId).AsNoTracking().SingleOrDefaultAsync();
                if (updatedKey == null)
                {
                    return new ServiceResult<KeyEntity>() { Status = ResultStatusCode.NotFound };
                }

                updatedKey.Label = requestBody.Label;
                updatedKey.UpdatedBy = requestBody.UpdatedBy;
                updatedKey.Description = requestBody.Description;
                var returedUpdatedSku = _dbContext.Keys.Update(updatedKey);

                await _dbContext.SaveChangesAsync();

                if (redact)
                {
                    returedUpdatedSku.Entity.PrivateKey = KeyEntity.Redact;
                }

                return new ServiceResult<KeyEntity>() { Status = ResultStatusCode.Success, Data = returedUpdatedSku.Entity };
            }
            catch (Exception ex)
            {
                return ReturnException<KeyEntity>(ex);
            }
        }

        /// <summary>
        /// Deletes a key by ID.
        /// </summary>
        /// <param name="keyId">The key ID.</param>
        /// <returns>Service result containing the deleted key entity.</returns>
        public async Task<ServiceResult<KeyEntity>> DeleteKeyAsync(string keyId)
        {
            try
            {
                var toDeleteKey = await _dbContext.Keys.Where(x => x.Id == keyId).AsNoTracking().SingleOrDefaultAsync();
                if (toDeleteKey == null)
                {
                    return new ServiceResult<KeyEntity>() { Status = ResultStatusCode.NotFound };
                }
                var returedDeletedSku = _dbContext.Keys.Remove(toDeleteKey);

                await _dbContext.SaveChangesAsync();
                return new ServiceResult<KeyEntity>() { Status = ResultStatusCode.Success, Data = null };
            }
            catch (Exception ex)
            {
                return ReturnException<KeyEntity>(ex);
            }
        }
    }
}

