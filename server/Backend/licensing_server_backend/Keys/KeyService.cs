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
    public class KeyService : IKeyService
    {
        private readonly ILogger<KeyService> _logger;
        private readonly LicensingContext _dbContext;

        public KeyService(ILogger<KeyService> logger, LicensingContext context)
        {
            _logger = logger;
            _dbContext = context;
        }

        public async Task<ServiceResult<PaginatedResults>> GetKeysAsync(BasicQueryFilter filter)
        {
            try
            {
                var skus = await _dbContext.Keys.OrderBy(x => x.CreatedAt).Skip(filter.Offset).Take(filter.Limit).AsNoTracking().ToListAsync();
                if (skus == null)
                {
                    return new ServiceResult<PaginatedResults>()
                    {
                        Status = ResultStatusCode.Success,
                        Data = new PaginatedResults() { Limit = filter.Limit, Offset = filter.Offset, Results = new object[] { } }
                    };
                }

                return new ServiceResult<PaginatedResults>()
                {
                    Status = ResultStatusCode.Success,
                    Data = new PaginatedResults() { Limit = filter.Limit, Offset = filter.Offset, Count = skus.Count, Results = skus }
                };
            }
            catch (Exception ex)
            {
                return ReturnException<PaginatedResults>(ex, $"Error getting keys");
            }
        }

        public async Task<ServiceResult<PaginatedResults>> GetByCustomerIdAsync(string customerId, BasicQueryFilter filter)
        {
            try
            {
                var skus = await _dbContext.Keys.Where( x => x.CustomerId == customerId).OrderBy(x => x.CreatedAt).Skip(filter.Offset).Take(filter.Limit).AsNoTracking().ToListAsync();
                if (skus == null)
                {
                    return new ServiceResult<PaginatedResults>()
                    {
                        Status = ResultStatusCode.Success,
                        Data = new PaginatedResults() { Limit = filter.Limit, Offset = filter.Offset, Results = new object[] { } }
                    };
                }

                return new ServiceResult<PaginatedResults>()
                {
                    Status = ResultStatusCode.Success,
                    Data = new PaginatedResults() { Limit = filter.Limit, Offset = filter.Offset, Count = skus.Count, Results = skus }
                };
            }
            catch (Exception ex)
            {
                return ReturnException<PaginatedResults>(ex, "Error getting keys");
            }
        }


        public async Task<ServiceResult<byte[]>> DownloadPublicKeyAsync(string keyId)
        {
            try
            {
                var certInfo = await _dbContext.Keys.Where(x => x.Id == keyId).AsNoTracking().SingleOrDefaultAsync();
                if (certInfo == null)
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

        public async Task<ServiceResult<byte[]>> DownloadPrivateKeyAsync(string keyId)
        {
            try
            {
                var certInfo = await _dbContext.Keys.Where(x => x.Id == keyId).AsNoTracking().SingleOrDefaultAsync();
                if (certInfo == null)
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


        public async Task<ServiceResult<KeyEntity>> GetByIdAsync(string keyId)
        {
            try
            {
                var sku = await _dbContext.Keys.Where(x => x.Id == keyId).AsNoTracking().SingleOrDefaultAsync();
                if (sku == null)
                {
                    return new ServiceResult<KeyEntity>() { Status = ResultStatusCode.NotFound };
                }
                return new ServiceResult<KeyEntity>() { Status = ResultStatusCode.Success, Data = sku };
            }
            catch (Exception ex)
            {
                return ReturnException<KeyEntity>(ex, $"Error getting certificate information by Id");
            }
        }

        public async Task<ServiceResult<KeyEntity>> GenerateKeys(KeyGenerationRequestBody keyGenRequest)
        {
            if (keyGenRequest == null || !keyGenRequest.IsValid())
            {
                return new ServiceResult<KeyEntity>()
                {
                    Status = ResultStatusCode.BadRequest,
                    ErrorMessage = new ErrorMessageStruct("Invalid request body")
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
                        CustomerId = keyGenRequest.CustomerId,
                        SiteId = keyGenRequest.SiteId,
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

        public async Task<ServiceResult<KeyEntity>> UpdateKeyAsync(string keyId, KeyUpdateRequestBody requestBody)
        {
            if (requestBody == null || !requestBody.IsValid())
            {
                return new ServiceResult<KeyEntity>()
                {
                    Status = ResultStatusCode.BadRequest,
                    ErrorMessage = new ErrorMessageStruct("Invalid request body")
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
                updatedKey.CustomerId = requestBody.CustomerId;
                updatedKey.SiteId = requestBody.SiteId;
                updatedKey.UpdatedBy = requestBody.UpdatedBy;
                updatedKey.Description = requestBody.Description;
                var returedUpdatedSku = _dbContext.Keys.Update(updatedKey);

                await _dbContext.SaveChangesAsync();
                return new ServiceResult<KeyEntity>() { Status = ResultStatusCode.Success, Data = returedUpdatedSku.Entity };
            }
            catch (Exception ex)
            {
                return ReturnException<KeyEntity>(ex);
            }
        }

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
                return new ServiceResult<KeyEntity>() { Status = ResultStatusCode.Success, Data = returedDeletedSku.Entity };
            }
            catch (Exception ex)
            {
                return ReturnException<KeyEntity>(ex);
            }
        }


        /// <summary>
        /// Returns a service result based on the specified exception
        /// </summary>
        /// <param name="ex"></param>
        private ServiceResult<T> ReturnException<T>(Exception ex, string logMessage = "")
        {
            if (String.IsNullOrWhiteSpace(logMessage))
                _logger.LogError($"{logMessage}");
            else
                _logger.LogError($"{logMessage}: {ex.Message}");

            return ExceptionHandler.ReturnException<T>(ex);
        }
    }
}
