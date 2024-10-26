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
        /// <summary>
        /// xóa nhiều sản phẩm
        /// </summary>
        /// <param name="ids">danh sách</param>
        /// <returns></returns>
        public async Task<bool> DeleteMultipleAsync(List<int> ids)
        {
            return await _productRepository.DeleteMultipleAsync(ids);
        }

        /// <summary>
        /// xóa 1 sản phẩm
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> DeleteProductAsync(int id)
        {
            return await _productRepository.DeleteAsync(id);
        }

        /// <summary>
        /// service lấy danh sách sản phẩm 
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<ProductDto>> GetAllProductAsync()
        {
            var products = await _productRepository.GetAllAsync();

            var productDtos = products.Select(p => _mapper.Map<ProductDto>(p)).ToList();

            return productDtos;
        }

        /// <summary>
        /// phân trang
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        public async Task<(IEnumerable<ProductDto> Products, int TotalCount)> GetPagedProductsAsync(int pageIndex, int pageSize, string searchTerm)
        {
            var (products, totalCount) = await _productRepository.GetPagedProductsAsync(pageIndex, pageSize, searchTerm);
            var productDtos = products.Select(p => _mapper.Map<ProductDto>(p));

            return (productDtos, totalCount);
        }

        /// <summary>
        /// lấy tất cả sản phẩm active (phục vụ cho việc thêm vào đơn hàng)
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<ProductDto>> GetAllProductActiveAsync()
        {
            var products = await _productRepository.GetAllActiveAsync();

            var productDtos = products.Select(p => _mapper.Map<ProductDto>(p)).ToList();

            return productDtos;
        }

        /// <summary>
        /// lấy thông tin sản phẩm
        /// </summary>
        /// <returns></returns>
        public async Task<ProductDto?> GetProductById(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if(product != null)
                return _mapper.Map<ProductDto>(product);

            return null;
        }

        /// <summary>
        /// lấy thông tin sản phẩm theo mã sản phẩm
        /// </summary>
        /// <returns></returns>
        public async Task<ProductDto?> GetProductByCode(string code)
        {
            var product = await _productRepository.GetByCodeAsync(code);
            if (product != null)
                return _mapper.Map<ProductDto>(product);

            return null;
        }

        /// <summary>
        /// cập nhật sản phẩm
        /// </summary>
        /// <param name="id"></param>
        /// <param name="productDto"> thông tin update</param>
        /// <returns></returns>
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

        /// <summary>
        /// Thêm mới sản phẩm
        /// </summary>
        /// <param name="productDto">Thông tin sản phẩm thêm mới</param>
        /// <returns></returns>
        public async Task<ProductDto> AddProductAsync(ProductDto productDto)
        {
            productDto.ProductCode = await GenerateProductCode();
            productDto.CreatedDate = DateTime.Now;


            var productEntity = _mapper.Map<Product>(productDto);
            var result = await _productRepository.AddAsync(productEntity);
            return  _mapper.Map<ProductDto>(result);
        }

        /// <summary>
        /// tra cứu theo tên hoặc mã 
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        public async Task<List<ProductDto>> SearchProductByNameOrByCode(string searchTerm)
        {
            var l = await _productRepository.SearchAsync(searchTerm);
            return l.Select(p => _mapper.Map<ProductDto>(p)).ToList();
        }

        /// <summary>
        /// cập nhật trạng thái của sản phẩm
        /// </summary>
        /// <param name="ids">danh sách id sản phẩm</param>
        /// <param name="isActive"></param>
        /// <returns></returns>
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

        /// <summary>
        /// sinh mã code cho sản phẩm
        /// </summary>
        /// <returns></returns>
        private async Task<string> GenerateProductCode()
        {
            var products = await _productRepository.GetAllAsync();
            var lastProduct = products.OrderByDescending(p => p.Id).FirstOrDefault();
            int nextId = (lastProduct == null) ? 1 : int.Parse(lastProduct.ProductCode.Substring(2)) + 1;

            return $"PR{nextId:D4}";
        }

    }
}
