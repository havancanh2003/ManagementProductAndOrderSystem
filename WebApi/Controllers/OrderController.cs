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
                        ProductCode = p.ProductCode,
                        ProductId = p.Id,
                        Quantity = item.Quantity,
                        Price = p.SellingPrice,
                        TaxRate = p.TaxRatePaid,
                    };
                    orderItem.TotalAmountNoTax = Calculate.CalculateTotalAmountNoTax(orderItem.Quantity, orderItem.Price);
                    orderItem.TaxAmount = Calculate.CalculateTaxAmount(orderItem.TotalAmountNoTax, p.TaxRatePaid);
                    orderItem.TaxDeductibleAmount = Calculate.CalculateTotalAmountAndTax(orderItem.TotalAmountNoTax, orderItem.TaxAmount);


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
                    CustomerName = model.CustomerName,
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
                if(result.IsSuccess)
                    return Ok(result.orderDtoReturn);

                return BadRequest(result.mess);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
