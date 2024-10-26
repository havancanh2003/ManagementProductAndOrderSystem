using Application.Common;
using Application.DTOs;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Repositories;

namespace Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductService _productService;
        private readonly IMapper _mapper;
        public OrderService(IOrderRepository orderRepository, IProductService productService, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _productService = productService;
            _mapper = mapper;
        }
        public async Task<List<OrderDto>> GetAllOrderAsync()
        {
            var orders = await _orderRepository.GetAllAsync();
            return orders.Select(o => MapToOrderDto(o)).ToList();
        }
        public async Task<OrderDto?> GetOrderById(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            return order != null ? MapToOrderDto(order) : null;
        }
        public async Task<OrderDto?> GetOrderByCode(string code)
        {
            var order = await _orderRepository.GetByCodeAsync(code);
            return order != null ? MapToOrderDto(order) : null;
        }
        public async Task<OrderDto?> AddOrderAsync(OrderDto model)
        {
            var order = new Order
            {
                OrderCode = await GenerateOrderCode(),
                CustomerName = model.CustomerName,
                CustomerPhone = model.CustomerPhone,
                CreatedDate = DateTime.UtcNow,
                TotalAmount = 0
            };
            foreach (var itemDto in model.Items)
            {
                //var salePrice = itemDto.SalePrice > 0 ? itemDto.SalePrice : product.SalePrice;
                var orderItem = new OrderItem
                {
                    ProductId = itemDto.ProductId,
                    ProductCode = itemDto.ProductCode,
                    Quantity = itemDto.Quantity,
                    Price = itemDto.Price,
                    TaxAmount = itemDto.TaxAmount,
                    TaxRate = itemDto.TaxRate,
                    TotalAmountNoTax = itemDto.TotalAmountNoTax,
                    TaxDeductibleAmount = itemDto.TaxDeductibleAmount,
                };

                order.OrderItems.Add(orderItem);
            }
            order.TotalAmount = order.OrderItems.Sum(i => i.TaxDeductibleAmount);
            order.TaxAmountOrder = order.OrderItems.Sum(i => i.TaxAmount);

            var result = await _orderRepository.AddAsync(order);
            return MapToOrderDto(result);
        }

        public async Task<(bool IsSuccess, string mess, OrderDto? orderDtoReturn)> UpdateOrderAsync(OrderDto orderDto)
        {
            var existingOrder = await _orderRepository.GetByIdAsync(orderDto.Id);
            if (existingOrder == null)
                return (false,"Không tồn tại đơn hàng trong hệ thống",null);

            if (existingOrder.CustomerName != orderDto.CustomerName)
                existingOrder.CustomerName = orderDto.CustomerName;

            if (existingOrder.CustomerPhone != orderDto.CustomerPhone)
                existingOrder.CustomerPhone = orderDto.CustomerPhone;

            // So sánh danh sách OrderItems cũ và mới
            var newItemIds = orderDto.Items.Select(i => i.ProductId).ToHashSet();

            // Cập nhật các OrderItem hiện có hoặc thêm mới nếu cần
            foreach (var itemDto in orderDto.Items)
            {
                var orderItem = existingOrder.OrderItems.FirstOrDefault(oi => oi.ProductId == itemDto.ProductId);

                if (orderItem != null)
                {
                    // Nếu số lượng hoặc giá bán thay đổi, cập nhật lại
                    if (orderItem.Quantity != itemDto.Quantity)
                    {
                        orderItem.Quantity = itemDto.Quantity;
                        orderItem.TotalAmountNoTax = Calculate.CalculateTotalAmountNoTax(orderItem.Quantity, orderItem.Price);
                        orderItem.TaxAmount = Calculate.CalculateTaxAmount(orderItem.TotalAmountNoTax, orderItem.TaxRate);
                        orderItem.TaxDeductibleAmount = Calculate.CalculateTotalAmountAndTax(orderItem.TotalAmountNoTax, orderItem.TaxAmount);
                    }
                }
                else
                {
                    // Thêm sản phẩm mới vào đơn hàng nếu chưa tồn tại
                    var product = await _productService.GetProductById(itemDto.ProductId);
                    if (product == null)
                        return (false, $"Sản phẩm với ID {itemDto.ProductId} không tồn tại.",null);

                    orderItem = new OrderItem
                    {
                        ProductId = product.Id,
                        Quantity = itemDto.Quantity,
                        ProductCode = product.ProductCode,
                        Price = product.SellingPrice,
                        TaxRate = product.TaxRatePaid,
                    };
                    orderItem.TotalAmountNoTax = Calculate.CalculateTotalAmountNoTax(orderItem.Quantity, orderItem.Price);
                    orderItem.TaxAmount = Calculate.CalculateTaxAmount(orderItem.TotalAmountNoTax, orderItem.TaxRate);
                    orderItem.TaxDeductibleAmount = Calculate.CalculateTotalAmountAndTax(orderItem.TotalAmountNoTax, orderItem.TaxAmount);
                    existingOrder.OrderItems.Add(orderItem);
                }
            }

            // Xóa các OrderItem không còn trong danh sách mới
            existingOrder.OrderItems.RemoveAll(oi => !newItemIds.Contains(oi.ProductId));

            // Tính lại tiền 
            existingOrder.TotalAmount = existingOrder.OrderItems.Sum(i => i.TaxDeductibleAmount);
            existingOrder.TaxAmountOrder = existingOrder.OrderItems.Sum(i => i.TaxAmount);

            var result = await _orderRepository.UpdateAsync(existingOrder);

            return (true, string.Empty, MapToOrderDto(result));
        }

        private async Task<string> GenerateOrderCode()
        {
            var today = DateTime.UtcNow.ToString("yyMMdd");
            var orders = await _orderRepository.GetAllAsync();
            var lastOrder = orders
                .Where(o => o.OrderCode.StartsWith("OD" + today))
                .OrderByDescending(o => o.OrderCode)
                .FirstOrDefault();

            int nextId = (lastOrder == null) ? 1 : int.Parse(lastOrder.OrderCode.Substring(8)) + 1;
            return $"OD{today}{nextId:D3}";
        }
        private OrderDto MapToOrderDto(Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                OrderCode = order.OrderCode,
                CustomerName = order.CustomerName,
                CustomerPhone = order.CustomerPhone,
                TotalAmount = order.TotalAmount,
                TaxAmountOrder = order.TaxAmountOrder,
                CreatedDate = order.CreatedDate,
                UpdatedDate = order.UpdatedDate,
                Items = order.OrderItems.Select(item => new OrderItemDto
                {
                    Price = item.Price,
                    Quantity = item.Quantity,
                    TaxAmount = item.TaxAmount,
                    TaxRate = item.TaxRate,
                    TotalAmountNoTax = item.TotalAmountNoTax,
                    TaxDeductibleAmount = item.TaxDeductibleAmount,
                    ProductCode = item.ProductCode,
                    ProductId = item.ProductId,
                    OrderId = order.Id,
                }).ToList()
            };
        }
    }
}
