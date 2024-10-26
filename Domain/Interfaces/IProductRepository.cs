using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IProductRepository
    {
        Task<Product> AddAsync(Product product);
        Task<Product> UpdateAsync(Product product);
        Task<bool> DeleteAsync(int id);
        Task<bool> DeleteMultipleAsync(List<int> ids);
        Task<IEnumerable<Product>> GetAllAsync();
        Task<IEnumerable<Product>> GetAllActiveAsync();
        Task<Product?> GetByIdAsync(int id);
        Task<Product?> GetByCodeAsync(string code);
        Task<List<Product>> SearchAsync(string searchTerm);
        Task UpdateRangeAsync(IEnumerable<Product> products);
        Task<(IEnumerable<Product> Products, int TotalCount)> GetPagedProductsAsync(int pageIndex, int pageSize, string searchTerm);
    }
}
