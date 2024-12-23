using Licensing.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
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
        public async Task<IEnumerable<object>> GetSkusAsync()
        {
            return await _dbContext.Skus.Select(s => new { s.Name, s.SkuCode }).ToListAsync();
        }

        /// <summary>
        /// Get SKU by code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<object?> GetSkusByCodeAsync(string code)
        {
            return await _dbContext.Skus.Where(x => x.SkuCode == code).Select(s => new { s.Name, s.SkuCode }).AsNoTracking().SingleOrDefaultAsync();
        }

        /// <summary>
        /// Get SKU by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<object?> GetSkusByNameAsync(string name)
        {
            return await _dbContext.Skus.Where(x => x.Name == name).Select(s => new { s.Name, s.SkuCode }).AsNoTracking().SingleOrDefaultAsync();
        }

        /// <summary>
        /// Get SKU by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<object?> GetSkusByIdAsync(int id)
        {
            return await _dbContext.Skus.Where(x => x.Id == id).Select(s => new { s.Name, s.SkuCode }).AsNoTracking().SingleOrDefaultAsync();
        }

    }
}
