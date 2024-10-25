using Application.DTOs;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        public ProductService(IProductRepository productRepository, IMapper mapper) 
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public Task ActiveOrUnActiveProduct(List<int> ids)
        {
            throw new NotImplementedException();
        }


        public async Task<bool> DeleteMultipleAsync(List<int> ids)
        {
            return await _productRepository.DeleteMultipleAsync(ids);
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            return await _productRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductAsync()
        {
            var products = await _productRepository.GetAllAsync();
            var productDtos = new List<ProductDto>();
            foreach(var p in products)
            {
                var productDto = _mapper.Map<ProductDto>(p);
                productDtos.Add(productDto);
            }
            return productDtos;
        }

        public async Task<ProductDto?> GetProductById(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if(product != null)
                return _mapper.Map<ProductDto>(product);

            return null;
        }

        public async Task<ProductDto?> UpdateProductAsync(int id, ProductDto productDto)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                return null;

            product.PurchasePrice = productDto.PurchasePrice;
            product.SellingPrice = productDto.SellingPrice;
            product.ProductName = productDto.ProductName;
            product.TaxRatePaid = productDto.TaxRatePaid;
            product.IsActive = productDto.IsActive;
            product.Unit = productDto.Unit;
            product.UpdatedDate = DateTime.Now;

            var result = await _productRepository.UpdateAsync(product);
            return _mapper.Map<ProductDto>(result);
        }

        public async Task<ProductDto> AddProductAsync(ProductDto productDto)
        {
            productDto.ProductCode = await GenerateProductCode();
            productDto.CreatedDate = DateTime.Now;


            var productEntity = _mapper.Map<Product>(productDto);
            var result = await _productRepository.AddAsync(productEntity);
            return  _mapper.Map<ProductDto>(result);
        }

        public async Task<List<ProductDto>> SearchProductByNameOrByCode(string searchTerm)
        {
            var l = await _productRepository.SearchAsync(searchTerm);
            return l.Select(p => _mapper.Map<ProductDto>(p)).ToList();
        }
        public async Task UpdateStatusProductsAsync(List<int> ids, bool isActive)
        {
            var products = await _productRepository.GetAllAsync();

            var query = products.Where(p => ids.Contains(p.Id)).ToList();

            foreach (var product in query)
            {
                product.IsActive = isActive; 
                product.UpdatedDate = DateTime.Now;
            }
            await _productRepository.UpdateRangeAsync(query);
        }

        // sinh mã code cho sản phẩm
        private async Task<string> GenerateProductCode()
        {
            var products = await _productRepository.GetAllAsync();
            var lastProduct = products.OrderByDescending(p => p.Id).FirstOrDefault();
            int nextId = (lastProduct == null) ? 1 : int.Parse(lastProduct.ProductCode.Substring(2)) + 1;

            return $"PR{nextId:D4}";
        }

    }
}
