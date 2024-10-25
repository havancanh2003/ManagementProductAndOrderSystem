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
        Task<ProductDto?> GetProductById(int id);
        Task<IEnumerable<ProductDto>> GetAllProductAsync();
        Task<ProductDto> AddProductAsync(ProductDto productDto);
        Task<ProductDto?> UpdateProductAsync(int id, ProductDto productDto);
        Task<bool> DeleteProductAsync(int id);
        Task<bool> DeleteMultipleAsync(List<int> ids);
        Task ActiveOrUnActiveProduct(List<int> ids);
        Task<List<ProductDto>> SearchProductByNameOrByCode(string searchTerm);
        Task UpdateStatusProductsAsync(List<int> ids, bool isActive);
    }
}
