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
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        public OrderController(IProductService productService, IOrderService orderService)
        {
            _productService = productService;
            _orderService = orderService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _orderService.GetAllOrderAsync();
            return Ok(orders);
        }
        [HttpGet("{code}")]
        public async Task<IActionResult> GetOrderByCode(string code)
        {
            var order = await _orderService.GetOrderByCode(code);
            if (order == null) return NotFound();

            return Ok(order);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrUpdateOrder model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.CustomerName) || string.IsNullOrEmpty(model.CustomerPhone))
                    return BadRequest("Tên và số điện thoại là bắt buộc cho việc khởi tạo đơn hàng");

                var orderDto = new OrderDto
                {
                    CustomerName = model.CustomerName,
                    CustomerPhone = model.CustomerPhone,
                };

                foreach (var item in model.Items)
                {
                    var p = await _productService.GetProductById(item.ProductId);
                    if (p == null)
                        return StatusCode(403, $"Sản phẩm với ID {item.ProductId} không tồn tại.");

                    var orderItem = new OrderItemDto
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        Price = item.Price
                    };
                    orderDto.Items.Add(orderItem);
                }
                var result = await _orderService.AddOrderAsync(orderDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
    }
}
