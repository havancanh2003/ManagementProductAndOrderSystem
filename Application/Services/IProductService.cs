using Application.DTOs;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface IProductService
    {
        Task<List<ProductDto>> SearchProductByNameOrByCode(string searchTerm);
        Task<ProductDto?> GetProductById(int id);
        Task<ProductDto?> GetProductByCode(string code);
        Task<IEnumerable<ProductDto>> GetAllProductAsync();
        Task<(IEnumerable<ProductDto> Products, int TotalCount)> GetPagedProductsAsync(int pageIndex, int pageSize, string searchTerm);
        Task<ProductDto> AddProductAsync(ProductDto productDto);
        Task<ProductDto?> UpdateProductAsync(int id, ProductDto productDto);
        Task UpdateStatusProductsAsync(List<int> ids, bool isActive);
        Task<bool> DeleteProductAsync(int id);
        Task<bool> DeleteMultipleAsync(List<int> ids);
    }
}
