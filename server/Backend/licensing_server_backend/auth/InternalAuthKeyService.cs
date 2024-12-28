using Licensing.Data;
using Licensing.Keys;
using Licensing.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;
using Licensing.Skus;
using Licensing.auth;

namespace Licensing.Auth
{
    /// <summary>
    /// Service for managing internal auth keys
    /// These keys are used to protect the web api endpoints, and are mapped to specific roles
    /// </summary>
    public class InternalAuthKeyService : BaseService<InternalAuthKeyService>, IInternalAuthKeyService
    {
        public InternalAuthKeyService(ILogger<InternalAuthKeyService> logger, LicensingContext context) : base(logger, context)
        {
        }

        /// <summary>
        /// Fetches a paginated list of internal auth keys.
        /// </summary>
        /// <param name="filter">Pagination filter.</param>
        /// <returns>Service result containing paginated list of internal auth keys.</returns>
        public async Task<ServiceResult<PaginatedResults>> GetInternalAuthKeysAsync(BasicQueryFilter filter)
        {
            try
            {
                var authKeys = await _dbContext.InternalAuthKeys.OrderBy(x => x.CreatedAt).Skip(filter.Offset).Take(filter.Limit).AsNoTracking().ToListAsync();
                if (authKeys == null)
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
                    Data = new PaginatedResults() { Limit = filter.Limit, Offset = filter.Offset, Count = authKeys.Count, Results = authKeys }
                };
            }
            catch (Exception ex)
            {
                return ReturnException<PaginatedResults>(ex, $"Error getting authKeys");
            }
        }

        /// <summary>
        /// Fetches an internal auth key by ID.
        /// </summary>
        /// <param name="authKeyId">Auth key ID.</param>
        /// <returns>Service result containing the internal auth key.</returns>
        public async Task<ServiceResult<InternalAuthKeyEntity>> GetByIdAsync(string authKeyId)
        {
            try
            {
                var authKey = await _dbContext.InternalAuthKeys.Where(x => x.Id == authKeyId).AsNoTracking().SingleOrDefaultAsync();
                if (authKey == null)
                {
                    return new ServiceResult<InternalAuthKeyEntity>() { Status = ResultStatusCode.NotFound };
                }
                return new ServiceResult<InternalAuthKeyEntity>() { Status = ResultStatusCode.Success, Data = authKey };
            }
            catch (Exception ex)
            {
                return ReturnException<InternalAuthKeyEntity>(ex, $"Error getting authKey");
            }
        }

        /// <summary>
        /// Fetches an internal auth key by key.
        /// </summary>
        /// <param name="key">Auth key.</param>
        /// <returns>Service result containing the internal auth key.</returns>
        public async Task<ServiceResult<InternalAuthKeyEntity>> GetByKeyAsync(string key)
        {
            try
            {
                var authKey = await _dbContext.InternalAuthKeys.Where(x => x.Key == key).AsNoTracking().SingleOrDefaultAsync();
                if (authKey == null)
                {
                    return new ServiceResult<InternalAuthKeyEntity>() { Status = ResultStatusCode.NotFound };
                }
                return new ServiceResult<InternalAuthKeyEntity>() { Status = ResultStatusCode.Success, Data = authKey };
            }
            catch (Exception ex)
            {
                return ReturnException<InternalAuthKeyEntity>(ex, $"Error getting authKey");
            }
        }

        /// <summary>
        /// Creates a new internal auth key.
        /// </summary>
        /// <param name="internalAuthKey">Internal auth key entity.</param>
        /// <returns>Service result containing the created internal auth key.</returns>
        public async Task<ServiceResult<InternalAuthKeyEntity>> CreateAuthKeyAsync(InternalAuthKeyEntity internalAuthKey)
        {
            try
            {
                _dbContext.InternalAuthKeys.Add(internalAuthKey);
                await _dbContext.SaveChangesAsync();
                return new ServiceResult<InternalAuthKeyEntity>() { Status = ResultStatusCode.Success, Data = internalAuthKey };
            }
            catch (Exception ex)
            {
                return ReturnException<InternalAuthKeyEntity>(ex, $"Error creating internalAuthKey");
            }
        }

        /// <summary>
        /// Adds a new internal auth key.
        /// </summary>
        /// <param name="internalAuthKey">Internal auth key entity.</param>
        /// <returns>Service result containing the added internal auth key.</returns>
        public async Task<ServiceResult<InternalAuthKeyEntity>> AddAuthKeyAsync(InternalAuthKeyEntity internalAuthKey)
        {
            try
            {
                var insInternalAuthKey = _dbContext.InternalAuthKeys.Add(internalAuthKey);
                int addCount = await _dbContext.SaveChangesAsync();
                return new ServiceResult<InternalAuthKeyEntity>() { Status = ResultStatusCode.Success, Data = insInternalAuthKey.Entity };
            }
            catch (Exception ex)
            {
                return ReturnException<InternalAuthKeyEntity>(ex);
            }
        }

        /// <summary>
        /// Updates an existing internal auth key.
        /// </summary>
        /// <param name="id">Auth key ID.</param>
        /// <param name="authKey">Auth key entity with updated data.</param>
        /// <returns>Service result containing updated internal auth key data.</returns>
        public async Task<ServiceResult<InternalAuthKeyEntity>> UpdateAuthKeyAsync(string id, UpdateInternalAuthKeyRequestBody authKey)
        {
            try
            {
                var updatedAuthKey = await _dbContext.InternalAuthKeys.Where(x => x.Id == id).AsNoTracking().SingleOrDefaultAsync();
                if (updatedAuthKey == null)
                {
                    return new ServiceResult<InternalAuthKeyEntity>() { Status = ResultStatusCode.NotFound };
                }

                if (!String.IsNullOrEmpty(authKey.Role)) 
                {
                    updatedAuthKey.Role = authKey.Role;
                }

                updatedAuthKey.Description = authKey.Description;
                _dbContext.InternalAuthKeys.Update(updatedAuthKey);

                await _dbContext.SaveChangesAsync();
                return new ServiceResult<InternalAuthKeyEntity>() { Status = ResultStatusCode.Success, Data = updatedAuthKey };
            }
            catch (Exception ex)
            {
                return ReturnException<InternalAuthKeyEntity>(ex, $"Error updating authKey");
            }
        }

        /// <summary>
        /// Deletes an internal auth key by ID.
        /// </summary>
        /// <param name="authKeyId">Auth key ID.</param>
        /// <returns>Service result containing the deleted internal auth key.</returns>
        public async Task<ServiceResult<InternalAuthKeyEntity>> DeleteByIdAsync(string authKeyId)
        {
            try
            {
                var authKey = await _dbContext.InternalAuthKeys.Where(x => x.Id == authKeyId).SingleOrDefaultAsync();
                if (authKey == null)
                {
                    return new ServiceResult<InternalAuthKeyEntity>() { Status = ResultStatusCode.NotFound };
                }
                var returnedDeletedAuthKey = _dbContext.InternalAuthKeys.Remove(authKey);
                await _dbContext.SaveChangesAsync();
                return new ServiceResult<InternalAuthKeyEntity>() { Status = ResultStatusCode.Success, Data = returnedDeletedAuthKey.Entity };
            }
            catch (Exception ex)
            {
                return ReturnException<InternalAuthKeyEntity>(ex, $"Error deleting authKey");
            }
        }

        /// <summary>
        /// Deletes an internal auth key by key.
        /// </summary>
        /// <param name="authKey">Auth key.</param>
        /// <returns>Service result containing the deleted internal auth key.</returns>
        public async Task<ServiceResult<InternalAuthKeyEntity>> DeleteByAuthKeyAsync(string authKey)
        {
            try
            {
                var returnAuthKey = await _dbContext.InternalAuthKeys.Where(x => x.Key == authKey).SingleOrDefaultAsync();
                if (returnAuthKey == null)
                {
                    return new ServiceResult<InternalAuthKeyEntity>() { Status = ResultStatusCode.NotFound };
                }
                var returedDeletedAuthKey = _dbContext.InternalAuthKeys.Remove(returnAuthKey);
                await _dbContext.SaveChangesAsync();
                return new ServiceResult<InternalAuthKeyEntity>() { Status = ResultStatusCode.Success, Data = returedDeletedAuthKey.Entity };
            }
            catch (Exception ex)
            {
                return ReturnException<InternalAuthKeyEntity>(ex, $"Error deleting authKey");
            }
        }
    }
}
