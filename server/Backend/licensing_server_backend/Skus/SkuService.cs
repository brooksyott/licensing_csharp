using Licensing.Common;
using Licensing.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Data.Common;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;

namespace Licensing.Skus
{
    public class SkuService : ISkuService
    {
        private readonly LicensingContext _dbContext;
        private readonly ILogger<SkuService> _logger;

        public SkuService(ILogger<SkuService> logger, LicensingContext context)
        {
            _dbContext = context;
            _logger = logger;
        }

        //public SkuService(LicensingContext context)
        //{
        //    _dbContext = context;
        //}

        /// <summary>
        /// Get all SKUs
        /// </summary>
        /// <returns></returns>
        public async Task<ServiceResult<PaginatedResults>> GetSkusAsync(BasicQueryFilter filter)
        {
            try
            {
                var skus = await _dbContext.Skus.OrderBy(x => x.Id).Skip(filter.Offset).Take(filter.Limit).AsNoTracking().ToListAsync();
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
                _logger.LogError(ex, $"Error getting skus: {ex}");
                return ReturnException<PaginatedResults>(ex);
            }
        }

        /// <summary>
        /// Get all SKUs
        /// </summary>
        /// <returns></returns>
        public async Task<ServiceResult<List<SkuEntity>?>> GetSkusAsync(List<string> skuList)
        {
            try
            {
                if (skuList.Count > 1000)
                {
                    return new ServiceResult<List<SkuEntity>?>()
                    {
                        Status = ResultStatusCode.BadRequest,
                        ErrorMessage = new ErrorMessageStruct("The maximum number of SKUs that can be retrieved is 1000")
                    };
                }

                var skus = await _dbContext.Skus
                               .Where(s => skuList.Contains(s.SkuCode))
                               .AsNoTracking().ToListAsync();

                if (skus == null)
                {
                    return new ServiceResult<List<SkuEntity>?>()
                    {
                        Status = ResultStatusCode.Success,
                        Data = null
                    };
                }

                return new ServiceResult<List<SkuEntity>?>()
                {
                    Status = ResultStatusCode.Success,
                    Data =  skus
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting skus: {ex}");
                return ReturnException<List<SkuEntity>?>(ex);
            }
        }

        /// <summary>
        /// Get SKU by code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<ServiceResult<SkuEntity>> GetSkusByCodeAsync(string code)
        {
            try
            {
                var sku = await _dbContext.Skus.Where(x => x.SkuCode == code).AsNoTracking().SingleOrDefaultAsync();
                if (sku == null)
                {
                    return new ServiceResult<SkuEntity>() { Status = ResultStatusCode.NotFound };
                }
                return new ServiceResult<SkuEntity>() { Status = ResultStatusCode.Success, Data = sku };
            }
            catch (Exception ex)
            {
                return ReturnException<SkuEntity>(ex, $"Error getting SKU by code");
            }
        }

        /// <summary>
        /// Get SKU by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<ServiceResult<SkuEntity>> GetSkusByNameAsync(string name)
        {
            try
            {
                var sku = await _dbContext.Skus.Where(x => x.Name == name).AsNoTracking().SingleOrDefaultAsync();
                if (sku == null)
                {
                    return new ServiceResult<SkuEntity>() { Status = ResultStatusCode.NotFound };
                }
                return new ServiceResult<SkuEntity>() { Status = ResultStatusCode.Success, Data = sku };
            }
            catch (Exception ex)
            {
                return ReturnException<SkuEntity>(ex);
            }
        }

        /// <summary>
        /// Get SKU by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ServiceResult<SkuEntity>> GetSkusByIdAsync(int id)
        {
            try
            {
                var sku = await _dbContext.Skus.Where(x => x.Id == id).AsNoTracking().SingleOrDefaultAsync();
                if (sku == null)
                {
                    return new ServiceResult<SkuEntity>() { Status = ResultStatusCode.NotFound };
                }
                return new ServiceResult<SkuEntity>() { Status = ResultStatusCode.Success, Data = sku };
            }
            catch (Exception ex)
            {
                return ReturnException<SkuEntity>(ex);
            }
        }

        /// <summary>
        /// Add a new SKU
        /// </summary>
        /// <param name="sku"></param>
        /// <returns></returns>
        public async Task<ServiceResult<SkuEntity>> AddSkuAsync(SkuEntity sku)
        {
            try
            {
                var insSku = _dbContext.Skus.Add(sku);
                int addCount = await _dbContext.SaveChangesAsync();
                return new ServiceResult<SkuEntity>() { Status = ResultStatusCode.Success, Data = insSku.Entity };
            }
            catch (Exception ex)
            {
                return ReturnException<SkuEntity>(ex);
            }

        }

        /// <summary>
        /// Update a sku
        /// </summary>
        /// <param name="sku"></param>"
        /// <returns>Bool</returns>
        public async Task<ServiceResult<SkuEntity>> UpdateSkuAsync(int id, SkuUpdate sku)
        {

            try
            {
                var updatedSku = await _dbContext.Skus.Where(x => x.Id == id).AsNoTracking().SingleOrDefaultAsync();
                if (updatedSku == null)
                {
                    return new ServiceResult<SkuEntity>() { Status = ResultStatusCode.NotFound };
                }

                updatedSku.Name = sku.Name;
                updatedSku.Description = sku.Description;
                var returedUpdatedSku = _dbContext.Skus.Update(updatedSku);

                await _dbContext.SaveChangesAsync();
                return new ServiceResult<SkuEntity>() { Status = ResultStatusCode.Success, Data = returedUpdatedSku.Entity };
            }
            catch (Exception ex)
            {
                return ReturnException<SkuEntity>(ex);
            }
        }

        /// <summary>
        /// Update a sku
        /// </summary>
        /// <param name="sku"></param>"
        /// <returns>Bool</returns>
        public async Task<ServiceResult<SkuEntity>> UpdateSkuAsync(string skuCode, SkuUpdate sku)
        {

            try
            {
                var updatedSku = await _dbContext.Skus.Where(x => x.SkuCode == skuCode).AsNoTracking().SingleOrDefaultAsync();
                if (updatedSku == null)
                {
                    return new ServiceResult<SkuEntity>() { Status = ResultStatusCode.NotFound };
                }

                updatedSku.Name = sku.Name;
                updatedSku.Description = sku.Description;
                var returedUpdatedSku = _dbContext.Skus.Update(updatedSku);

                await _dbContext.SaveChangesAsync();
                return new ServiceResult<SkuEntity>() { Status = ResultStatusCode.Success, Data = returedUpdatedSku.Entity };
            }
            catch (Exception ex)
            {
                return ReturnException<SkuEntity>(ex);
            }
        }

        /// <summary>
        /// Delete a sku by code
        /// </summary>
        /// <param name="skuCode"></param>
        /// <returns></returns>
        public async Task<ServiceResult<SkuEntity>> DeleteSkuAsync(string skuCode)
        {

            try
            {
                var toDeleteSku = await _dbContext.Skus.Where(x => x.SkuCode == skuCode).AsNoTracking().SingleOrDefaultAsync();
                if (toDeleteSku == null)
                {
                    return new ServiceResult<SkuEntity>() { Status = ResultStatusCode.NotFound };
                }
                var returedDeletedSku = _dbContext.Skus.Remove(toDeleteSku);

                await _dbContext.SaveChangesAsync();
                return new ServiceResult<SkuEntity>() { Status = ResultStatusCode.Success, Data = returedDeletedSku.Entity };
            }
            catch (Exception ex)
            {
                return ReturnException<SkuEntity>(ex);
            }
        }

        public async Task<ServiceResult<SkuEntity>> DeleteSkuAsync(int id)
        {

            try
            {
                var toDeleteSku = await _dbContext.Skus.Where(x => x.Id == id).AsNoTracking().SingleOrDefaultAsync();
                if (toDeleteSku == null)
                {
                    return new ServiceResult<SkuEntity>() { Status = ResultStatusCode.NotFound };
                }
                var returedDeletedSku = _dbContext.Skus.Remove(toDeleteSku);

                await _dbContext.SaveChangesAsync();
                return new ServiceResult<SkuEntity>() { Status = ResultStatusCode.Success, Data = returedDeletedSku.Entity };
            }
            catch (Exception ex)
            {
                return ReturnException<SkuEntity>(ex);
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
