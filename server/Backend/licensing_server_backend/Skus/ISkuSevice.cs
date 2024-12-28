using Licensing.Common;

namespace Licensing.Skus
{
    public interface ISkuService
    {
        Task<ServiceResult<PaginatedResults>> GetSkusAsync(BasicQueryFilter filter);
        Task<ServiceResult<List<SkuEntity>?>> GetSkusAsync(List<string> skuList);

        Task<ServiceResult<SkuEntity>> GetSkusByCodeAsync(string sku);
        Task<ServiceResult<SkuEntity>> GetSkusByNameAsync(string name);

        Task<ServiceResult<SkuEntity>> AddSkuAsync(SkuEntity sku);

        Task<ServiceResult<SkuEntity>> UpdateSkuAsync(string skuCode, SkuUpdate sku);

        Task<ServiceResult<SkuEntity>> DeleteSkuAsync(string skuCode);
    }
}
