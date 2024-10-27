using Application.Common;
using Application.DTOs;
using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models.Order;
using WebApi.Models.Product;

namespace WebApi.Controllers
{
    [Route("api/orders/")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly ILogger<OrderController> _logger;
        public OrderController(IProductService productService, IOrderService orderService, ILogger<OrderController> logger)
        {
            _productService = productService;
            _orderService = orderService;
            _logger = logger;
        }
        /// <summary>
        /// danh sách đơn hàng
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _orderService.GetAllOrderAsync();
            return Ok(orders);
        }

        /// <summary>
        /// lấy danh sách đơn hàng theo code
        /// </summary>
        [HttpGet("code")]
        public async Task<IActionResult> GetOrderByCode(string code)
        {
            if (string.IsNullOrEmpty(code))
                return BadRequest("Mã code đơn hàng không tồn tại");

            var order = await _orderService.GetOrderByCode(code);
            if (order == null)
                return NotFound();

            return Ok(order);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            if (id <= 0)
                return BadRequest("Id đơn hàng không tồn tại");

            var order = await _orderService.GetOrderById(id);
            if (order == null)
                return NotFound();

            return Ok(order);
        }
        /// <summary>
        /// Tạo mới đơn hàng
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrUpdateOrder model)
        {
            if (string.IsNullOrEmpty(model.CustomerName) || string.IsNullOrEmpty(model.CustomerPhone))
            {
                return BadRequest("Tên và số điện thoại là bắt buộc cho việc khởi tạo đơn hàng.");
            }

            try
            {
                var orderDto = new OrderDto
                {
                    CustomerName = model.CustomerName.Trim(),
                    CustomerPhone = model.CustomerPhone,
                    Items = new List<OrderItemDto>()
                };

                foreach (var item in model.Items)
                {
                    var product = await _productService.GetProductById(item.ProductId);
                    if (product == null)
                    {
                        return NotFound(new { message = $"Sản phẩm với ID {item.ProductId} không tồn tại." });
                    }

                    var orderItem = new OrderItemDto
                    {
                        ProductCode = product.ProductCode,
                        ProductId = product.Id,
                        Price = product.SellingPrice,
                        TaxRate = product.TaxRatePaid,
                        Quantity = item.Quantity,
                    };

                    orderItem.TotalAmountNoTax = Calculate.CalculateTotalAmountNoTax(orderItem.Quantity, orderItem.Price);
                    orderItem.TaxAmount = Calculate.CalculateTaxAmount(orderItem.TotalAmountNoTax, product.TaxRatePaid);
                    orderItem.TaxDeductibleAmount = Calculate.CalculateTotalAmountAndTax(orderItem.TotalAmountNoTax, orderItem.TaxAmount);

                    orderDto.Items.Add(orderItem);
                }

                var result = await _orderService.AddOrderAsync(orderDto);
                return CreatedAtAction(nameof(GetOrderByCode), new { code = result.OrderCode }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo đơn hàng.");
                return StatusCode(500,
                   new { message = ConstMess.MessServer, details = ex.Message });
            }
        }

        /// <summary>
        /// cập nhật đơn hàng
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateOrUpdateOrder model)
        {
            try
            {
                var order = await _orderService.GetOrderById(id);
                if (order == null)
                    return NotFound();

                if (string.IsNullOrEmpty(model.CustomerName) || string.IsNullOrEmpty(model.CustomerPhone))
                    return BadRequest("Tên và số điện thoại khách hàng là bắt buộc cho 1 đơn hàng");

                var orderDto = new OrderDto
                {
                    Id = order.Id,
                    CustomerName = model.CustomerName.Trim(),
                    CustomerPhone = model.CustomerPhone,
                };
                foreach (var iupdate in model.Items)
                {
                    var itemUpdate = new OrderItemDto
                    {
                        ProductId = iupdate.ProductId,
                        Quantity = iupdate.Quantity,
                    };
                    orderDto.Items.Add(itemUpdate);
                }
                var result = await _orderService.UpdateOrderAsync(orderDto);
                if (result.IsSuccess)
                    return Ok(result.orderDtoReturn);

                return BadRequest(result.mess);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật đơn hàng với ID: {Id}.", id);
                return StatusCode(500, 
                    new { message = ConstMess.MessServer, details = ex.Message });
            }
        }
    }
}
