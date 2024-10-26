using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public ProductRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        #region DeleteMethod
        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _dbContext.Products.FindAsync(id);
            if (product == null) return false;

            product.IsDeleted = true;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteMultipleAsync(List<int> ids)
        {
            var products = await _dbContext.Products
                .Where(p => ids.Contains(p.Id))
                .ToListAsync();

            if (products == null || products.Count == 0)
                return false;

            foreach (var product in products)
            {
                product.IsDeleted = true;
            }

            await _dbContext.SaveChangesAsync();
            return true;
        }
        #endregion

        #region GetMethod
        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _dbContext.Products.Where(p => !p.IsDeleted).ToListAsync();
        }
        public async Task<IEnumerable<Product>> GetAllActiveAsync()
        {
            return await _dbContext.Products.Where(p => !p.IsDeleted && p.IsActive).ToListAsync();
        }
        public async Task<(IEnumerable<Product> Products, int TotalCount)> GetPagedProductsAsync(int pageIndex, int pageSize, string searchTerm)
        {
            var query = _dbContext.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p => p.ProductName.Contains(searchTerm.Trim()) || p.ProductCode.Contains(searchTerm.Trim()));
            }

            var totalCount = await query.CountAsync(); 

            var products = await query
                .Where(p => !p.IsDeleted)
                .OrderBy(p => p.ProductName)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (products, totalCount);
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _dbContext.Products.FindAsync(id);
        }
        public async Task<Product?> GetByCodeAsync(string code)
        {
            return await _dbContext.Products.FirstOrDefaultAsync(p => p.ProductCode == code);
        }

        public async Task<List<Product>> SearchAsync(string searchTerm)
        {
            return await _dbContext.Products
                .Where(p => !p.IsDeleted)
                .Where(p => p.ProductName.Contains(searchTerm) || p.ProductCode.Contains(searchTerm))
                .ToListAsync();
        }
        #endregion

        #region InputMethod
        public async Task<Product> AddAsync(Product product)
        {
            _dbContext.Products.Add(product);
            await _dbContext.SaveChangesAsync();
            return product;
        }
        public async Task<Product> UpdateAsync(Product product)
        {
            _dbContext.Products.Update(product);
            await _dbContext.SaveChangesAsync();
            return product;
        }
        public async Task UpdateRangeAsync(IEnumerable<Product> products)
        {
            _dbContext.Products.UpdateRange(products);
            await _dbContext.SaveChangesAsync();
        }
        #endregion
    }
}
