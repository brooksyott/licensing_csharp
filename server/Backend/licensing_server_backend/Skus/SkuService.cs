﻿using Licensing.Common;
using Licensing.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Data.Common;
using System.Xml.Linq;

namespace Licensing.Skus
{
    public class SkuService : ISkuService
    {
        private readonly LicensingContext _dbContext;

        public SkuService(LicensingContext context)
        {
            _dbContext = context;
        }

        /// <summary>
        /// Get all SKUs
        /// </summary>
        /// <returns></returns>
        public async Task<ServiceResult<PaginatedResults>> GetSkusAsync(BasicQueryFilter filter)
        {
            var skus = await _dbContext.Skus.Skip(filter.Offset).Take(filter.Limit).AsNoTracking().ToListAsync();
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

        /// <summary>
        /// Get SKU by code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<ServiceResult<Sku>> GetSkusByCodeAsync(string code)
        {
            try
            {
                var sku = await _dbContext.Skus.Where(x => x.SkuCode == code).AsNoTracking().SingleOrDefaultAsync();
                if (sku == null)
                {
                    return new ServiceResult<Sku>() { Status = ResultStatusCode.NotFound };
                }
                return new ServiceResult<Sku>() { Status = ResultStatusCode.Success, Data = sku };
            }
            catch (Exception ex)
            {
                return ReturnException(ex);
            }
        }

        /// <summary>
        /// Get SKU by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<ServiceResult<Sku>> GetSkusByNameAsync(string name)
        {
            try
            {
                var sku = await _dbContext.Skus.Where(x => x.Name == name).AsNoTracking().SingleOrDefaultAsync();
                if (sku == null)
                {
                    return new ServiceResult<Sku>() { Status = ResultStatusCode.NotFound };
                }
                return new ServiceResult<Sku>() { Status = ResultStatusCode.Success, Data = sku };
            }
            catch (Exception ex)
            {
                return ReturnException(ex);
            }
        }

        /// <summary>
        /// Get SKU by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ServiceResult<Sku>> GetSkusByIdAsync(int id)
        {
            try
            {
                var sku = await _dbContext.Skus.Where(x => x.Id == id).AsNoTracking().SingleOrDefaultAsync();
                if (sku == null)
                {
                    return new ServiceResult<Sku>() { Status = ResultStatusCode.NotFound };
                }
                return new ServiceResult<Sku>() { Status = ResultStatusCode.Success, Data = sku };
            } 
            catch(Exception ex)
            {
                return ReturnException(ex);
            }
        }

        /// <summary>
        /// Add a new SKU
        /// </summary>
        /// <param name="sku"></param>
        /// <returns></returns>
        public async Task<ServiceResult<Sku>> AddSkuAsync(Sku sku)
        {
            try
            {
                var insSku = _dbContext.Skus.Add(sku);
                int addCount = await _dbContext.SaveChangesAsync();
                return new ServiceResult<Sku>() { Status = ResultStatusCode.Success, Data = insSku.Entity };
            }
            catch (Exception ex)
            {
                return ReturnException(ex);
            }

        }

        /// <summary>
        /// Update a sku
        /// </summary>
        /// <param name="sku"></param>"
        /// <returns>Bool</returns>
        public async Task<ServiceResult<Sku>> UpdateSkuAsync(int id, SkuUpdate sku)
        {

            try
            {
                var updatedSku = await _dbContext.Skus.Where(x => x.Id == id).AsNoTracking().SingleOrDefaultAsync();
                if (updatedSku == null)
                {
                    return new ServiceResult<Sku>() { Status = ResultStatusCode.NotFound };
                }

                updatedSku.Name = sku.Name;
                updatedSku.Description = sku.Description;
                var returedUpdatedSku = _dbContext.Skus.Update(updatedSku);

                await _dbContext.SaveChangesAsync();
                return new ServiceResult<Sku>() { Status = ResultStatusCode.Success, Data = returedUpdatedSku.Entity };
            }
            catch (Exception ex)
            {
                return ReturnException(ex);
            }
        }

        /// <summary>
        /// Update a sku
        /// </summary>
        /// <param name="sku"></param>"
        /// <returns>Bool</returns>
        public async Task<ServiceResult<Sku>> UpdateSkuAsync(string skuCode, SkuUpdate sku)
        {

            try
            {
                var updatedSku = await _dbContext.Skus.Where(x => x.SkuCode == skuCode).AsNoTracking().SingleOrDefaultAsync();
                if (updatedSku == null)
                {
                    return new ServiceResult<Sku>() { Status = ResultStatusCode.NotFound };
                }

                updatedSku.Name = sku.Name;
                updatedSku.Description = sku.Description;
                var returedUpdatedSku = _dbContext.Skus.Update(updatedSku);

                await _dbContext.SaveChangesAsync();
                return new ServiceResult<Sku>() { Status = ResultStatusCode.Success, Data = returedUpdatedSku.Entity };
            }
            catch (Exception ex)
            {
                return ReturnException(ex);
            }
        }

        /// <summary>
        /// Delete a sku by code
        /// </summary>
        /// <param name="skuCode"></param>
        /// <returns></returns>
        public async Task<ServiceResult<Sku>> DeleteSkuAsync(string skuCode)
        {

            try
            {
                var toDeleteSku = await _dbContext.Skus.Where(x => x.SkuCode == skuCode).AsNoTracking().SingleOrDefaultAsync();
                if (toDeleteSku == null)
                {
                    return new ServiceResult<Sku>() { Status = ResultStatusCode.NotFound };
                }
                var returedDeletedSku = _dbContext.Skus.Remove(toDeleteSku);

                await _dbContext.SaveChangesAsync();
                return new ServiceResult<Sku>() { Status = ResultStatusCode.Success, Data = returedDeletedSku.Entity };
            }
            catch (Exception ex)
            {
                return ReturnException(ex);
            }
        }

        public async Task<ServiceResult<Sku>> DeleteSkuAsync(int id)
        {

            try
            {
                var toDeleteSku = await _dbContext.Skus.Where(x => x.Id == id).AsNoTracking().SingleOrDefaultAsync();
                if (toDeleteSku == null)
                {
                    return new ServiceResult<Sku>() { Status = ResultStatusCode.NotFound };
                }
                var returedDeletedSku = _dbContext.Skus.Remove(toDeleteSku);

                await _dbContext.SaveChangesAsync();
                return new ServiceResult<Sku>() { Status = ResultStatusCode.Success, Data = returedDeletedSku.Entity };
            }
            catch (Exception ex)
            {
                return ReturnException(ex);
            }
        }

        /// <summary>
        /// Returns a service result based on the specified exception
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        private ServiceResult<Sku> ReturnException(Exception ex)
        {
            switch (ex)
            {
                case DbUpdateException dbUpdateEx:
                    if (dbUpdateEx.InnerException is Npgsql.PostgresException postgresExInner)
                    {
                        var errorMessage = postgresExInner.Message;
                        if (errorMessage.Contains("duplicate"))
                        {
                            return new ServiceResult<Sku>() { ErrorMessage = new ErrorMessageStruct(postgresExInner.Message), Status = ResultStatusCode.Conflict };
                        }

                        return new ServiceResult<Sku>() { ErrorMessage = new ErrorMessageStruct(postgresExInner.Message), Status = ResultStatusCode.BadRequest };
                    }
                    else
                    {
                        return new ServiceResult<Sku>() { ErrorMessage = new ErrorMessageStruct(dbUpdateEx.InnerException?.GetType().Name), Status = ResultStatusCode.InternalServerError };
                    }

                case Npgsql.PostgresException postgresEx:
                    return new ServiceResult<Sku>() { ErrorMessage = new ErrorMessageStruct(postgresEx.Message), Status = ResultStatusCode.InternalServerError };

                case InvalidOperationException invalidOpEx:
                    return new ServiceResult<Sku>() { ErrorMessage = new ErrorMessageStruct(invalidOpEx.Message), Status = ResultStatusCode.BadRequest };

                default:
                    return new ServiceResult<Sku>() { ErrorMessage = new ErrorMessageStruct(ex.Message), Status = ResultStatusCode.InternalServerError };
            }
        }

    }
}
