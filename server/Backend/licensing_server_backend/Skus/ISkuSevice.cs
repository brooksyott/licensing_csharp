using Licensing.Common;

namespace Licensing.Skus
{
    public interface ISkuService
    {
        // Create
        Task<ServiceResult<SkuEntity>> AddSkuAsync(SkuEntity sku);

        // Read
        Task<ServiceResult<PaginatedResults>> GetSkusAsync(BasicQueryFilter filter);
        Task<ServiceResult<List<SkuEntity>?>> GetSkusAsync(List<string> skuList);

        Task<ServiceResult<SkuEntity>> GetSkusByCodeAsync(string sku);
        Task<ServiceResult<SkuEntity>> GetSkusByNameAsync(string name);

        // Update
        Task<ServiceResult<SkuEntity>> UpdateSkuAsync(string skuCode, SkuUpdate sku);

        // Delete
        Task<ServiceResult<SkuEntity>> DeleteSkuAsync(string skuCode);
    }
}
