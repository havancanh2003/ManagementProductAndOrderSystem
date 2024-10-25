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

        public async Task<Product> AddAsync(Product product)
        {
            _dbContext.Products.Add(product);
            await _dbContext.SaveChangesAsync();
            return product;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _dbContext.Products.FindAsync(id);
            if (product == null) return false;

            _dbContext.Products.Remove(product);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteMultipleAsync(List<int> ids)
        {
            var products = await _dbContext.Products.Where(p => ids.Contains(p.Id)).ToListAsync();
            _dbContext.Products.RemoveRange(products);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _dbContext.Products.ToListAsync();
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            return await _dbContext.Products.FindAsync(id);
        }

        public Task<List<Product>> GetPagedAsync(int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Product>> SearchAsync(string searchTerm)
        {
            return await _dbContext.Products
                .Where(p => p.ProductName.Contains(searchTerm) || p.ProductCode.Contains(searchTerm))
                .ToListAsync();
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
    }
}
