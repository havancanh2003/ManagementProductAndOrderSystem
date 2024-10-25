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
        Task<List<Product>> GetPagedAsync(int pageNumber, int pageSize);
        Task<Product> GetByIdAsync(int id);
        Task<List<Product>> SearchAsync(string searchTerm);
        Task UpdateRangeAsync(IEnumerable<Product> products);
    }
}
