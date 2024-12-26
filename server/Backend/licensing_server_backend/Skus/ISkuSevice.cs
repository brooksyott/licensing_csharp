using Licensing.Common;

namespace Licensing.Skus
{
    public interface ISkuService
    {
        Task<ServiceResult<PaginatedResults>> GetSkusAsync(BasicQueryFilter filter);
        Task<ServiceResult<List<Sku>?>> GetSkusAsync(List<string> skuList);

        Task<ServiceResult<Sku>> GetSkusByCodeAsync(string sku);
        Task<ServiceResult<Sku>> GetSkusByIdAsync(int id);
        Task<ServiceResult<Sku>> GetSkusByNameAsync(string name);

        Task<ServiceResult<Sku>> AddSkuAsync(Sku sku);

        Task<ServiceResult<Sku>> UpdateSkuAsync(int id, SkuUpdate sku);
        Task<ServiceResult<Sku>> UpdateSkuAsync(string skuCode, SkuUpdate sku);

        Task<ServiceResult<Sku>> DeleteSkuAsync(int id);
        Task<ServiceResult<Sku>> DeleteSkuAsync(string skuCode);
    }
}
