namespace Licensing.Skus
{
    public interface ISkuService
    {
        Task<IEnumerable<object>> GetSkusAsync();
        Task<object?> GetSkusByCodeAsync(string sku);
        Task<object?> GetSkusByIdAsync(int id);
        Task<object?> GetSkusByNameAsync(string name);
    }
}
