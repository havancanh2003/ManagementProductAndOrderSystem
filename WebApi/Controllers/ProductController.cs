using Application.DTOs;
using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models.Product;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAllProductAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetProductById(id);
            if (product == null) return NotFound();

            return Ok(product);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchProductsNameOrCode(string term)
        {
            var products = await _productService.SearchProductByNameOrByCode(term);
            return Ok(products);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrUpdateProduct model)
        {
            try
            {
                if (model.PurchasePrice < 0 || model.SellingPrice < 0)
                    return BadRequest("Đơn giá không được âm.");

                var productDto = new ProductDto
                {
                    ProductName = model.ProductName,
                    Unit = model.Unit,
                    PurchasePrice = model.PurchasePrice,
                    SellingPrice = model.SellingPrice,
                    TaxRatePaid = model.TaxRatePaid,
                    IsActive = model.IsActive
                };

                var result = await _productService.AddProductAsync(productDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateOrUpdateProduct model)
        {
            try
            {
                if (model.PurchasePrice < 0 || model.SellingPrice < 0)
                    return BadRequest("Đơn giá không được âm.");

                var productDto = new ProductDto
                {
                    ProductName = model.ProductName,
                    Unit = model.Unit,
                    PurchasePrice = model.PurchasePrice,
                    SellingPrice = model.SellingPrice,
                    IsActive = model.IsActive,
                    TaxRatePaid = model.TaxRatePaid
                };

                var result = await _productService.UpdateProductAsync(id, productDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }


        [HttpPut("multiple")]
        public async Task<IActionResult> UpdateStatusProducts([FromBody] List<int> ids, bool isActive = true)
        {
            await _productService.UpdateStatusProductsAsync(ids,isActive);
            return Ok();
        }
       

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _productService.DeleteProductAsync(id);
            if (!result) return NotFound();

            return NoContent();
        }

        [HttpDelete("multiple")]
        public async Task<IActionResult> DeleteMultiple([FromBody] List<int> ids)
        {
            await _productService.DeleteMultipleAsync(ids);
            return NoContent();
        }
    }
}
