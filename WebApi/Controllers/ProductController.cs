using Application.Common;
using Application.DTOs;
using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models.Product;

namespace WebApi.Controllers
{
    [Route("api/products/")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;
        public ProductController(IProductService productService, ILogger<ProductController> logger)
        {
            _productService = productService;
            _logger = logger;
        }
        /// <summary>
        /// Lấy danh sách tất cả sản phẩm
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllProduct()
        {
            var products = await _productService.GetAllProductAsync();
            return Ok(products);
        }

        /// <summary>
        /// Danh sách phân trang
        /// </summary>
        [HttpGet("paged")]
        public async Task<IActionResult> GetPagedProducts([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10, [FromQuery] string searchTerm = "")
        {
            var (products, totalCount) = await _productService.GetPagedProductsAsync(pageIndex, pageSize, searchTerm);

            var response = new
            {
                Products = products,
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            return Ok(response);
        }

        /// <summary>
        /// Lấy sản phẩm theo mã code sản phẩm
        /// </summary>
        [HttpGet("{code}")]
        public async Task<IActionResult> GetProductByCode(string code)
        {
            if (string.IsNullOrEmpty(code))
                return BadRequest("Mã sản phẩm không được null hoặc rỗng.");

            var product = await _productService.GetProductByCode(code.Trim());

            if (product == null) 
                return NotFound();

            return Ok(product);
        }

        /// <summary>
        /// Tìm kiếm sản phẩm theo tên và code
        /// </summary>
        [HttpGet("searchProducts")]
        public async Task<IActionResult> SearchProductsNameOrCode(string nameOrCode)
        {
            if (string.IsNullOrEmpty(nameOrCode))
            {
                return BadRequest("Tên hoặc mã sản phẩm không được null hoặc rỗng.");
            }
            var products = await _productService.SearchProductByNameOrByCode(nameOrCode.Trim());
            return Ok(products);
        }

        /// <summary>
        /// Thêm mới sản phẩm
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateOrUpdateProduct model)
        {
            var validate = ValidateProductModel(model);
            if (!string.IsNullOrEmpty(validate))
                return BadRequest(validate);

            try
            {

                var productDto = new ProductDto
                {
                    ProductName = model.ProductName.Trim(),
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
                _logger.LogError(ex, "Lỗi khi tạo sản phẩm.");
                return StatusCode(500,
                   new { message = ConstMess.MessServer, details = ex.Message });
            }
        }

        /// <summary>
        /// Cập nhật sản phẩm
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] CreateOrUpdateProduct model)
        {
            var validate = ValidateProductModel(model);
            if (!string.IsNullOrEmpty(validate))
                return BadRequest(validate);

            try
            {
                var productDto = new ProductDto
                {
                    ProductName = model.ProductName.Trim(),
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
                _logger.LogError(ex, "Lỗi khi cập nhật sản phẩm với ID: {Id}.", id);
                return StatusCode(500,
                    new { message = ConstMess.MessServer, details = ex.Message });
            }
        }

        /// <summary>
        /// Cập nhật trạng thái hoạt động của sản phẩm
        /// </summary>
        [HttpPut("multiple")]
        public async Task<IActionResult> UpdateStatusProducts([FromBody] List<int> ids, bool isActive = true)
        {
            if (ids == null || !ids.Any())
            {
                return BadRequest("Danh sách ID sản phẩm không được null hoặc rỗng.");
            }
            try
            {
                await _productService.UpdateStatusProductsAsync(ids, isActive);
                return Ok("Update Thành công");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật trạng thái sản phẩm.");
                return StatusCode(500,
                    new { message = ConstMess.MessServer, details = ex.Message });
            }
        }

        /// <summary>
        /// Xóa sản phẩm
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var result = await _productService.DeleteProductAsync(id);
                if (!result)
                    return NotFound(new { message = "Sản phẩm không tồn tại." });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa sản phẩm với ID {Id}.", id);
                return StatusCode(500,
                    new { message = ConstMess.MessServer, details = ex.Message });
            }

        }

        /// <summary>
        /// Xóa nhiều sản phẩm
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete("multiple")]
        public async Task<IActionResult> DeleteMultipleProducts([FromBody] List<int> ids)
        {
            if (ids == null || !ids.Any())
            {
                return BadRequest("Danh sách ID sản phẩm không được null hoặc rỗng.");
            }
            try
            {
                await _productService.DeleteMultipleAsync(ids);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa nhiều sản phẩm với IDs: {Ids}", string.Join(", ", ids));
                return StatusCode(500,
                    new { message = ConstMess.MessServer, details = ex.Message });
            }
        }

        // dịch vụ riêng
        private string ValidateProductModel(CreateOrUpdateProduct model)
        {

            if (string.IsNullOrEmpty(model.ProductName))
                return "Tên sản phẩm là bắt buộc.";

            if (string.IsNullOrEmpty(model.Unit))
                return "Đơn vị tính là bắt buộc.";

            if (model.PurchasePrice < 0)
                return "Giá nhập không được âm.";

            if (model.SellingPrice < 0)
                return "Giá bán không được âm.";

            if (model.TaxRatePaid < 0 || model.TaxRatePaid > 100)
                return "Mức thuế phải chịu của sản phầm là 8% hoặc 10%";

            return string.Empty;
        }
    }
}
