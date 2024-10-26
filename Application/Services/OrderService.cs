using Application.DTOs;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        public OrderService(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }
        public async Task<List<OrderDto>> GetAllOrderAsync()
        {
            var orders = await _orderRepository.GetAllAsync();
            var listOrder = new List<OrderDto>();

            foreach (var o in orders)
            {
                var oDto = new OrderDto
                {
                    Id = o.Id,
                    OrderCode = o.OrderCode,
                    CustomerName = o.CustomerName,
                    CustomerPhone = o.CustomerPhone,
                    TotalAmount = o.TotalAmount,
                    TaxAmountOrder = o.TaxAmountOrder,
                    CreatedDate = o.CreatedDate,
                    UpdatedDate = o.UpdatedDate,
                    Items = o.OrderItems.Select(item => new OrderItemDto
                    {
                        Id = item.Id,
                        Price = item.Price,
                        Quantity = item.Quantity,
                        TaxAmount = item.TaxAmount,
                        TaxRate = item.TaxRate
                    }).ToList()
                };

                listOrder.Add(oDto);
            }

            return listOrder;
        }
        public async Task<OrderDto?> GetOrderById(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order != null)
                return _mapper.Map<OrderDto>(order);
            return null;
        }
        public async Task<OrderDto?> GetOrderByCode(string code)
        {
            var order = await _orderRepository.GetByCodeAsync(code);
            if (order != null)
                return _mapper.Map<OrderDto>(order);
            return null;
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
                    Quantity = itemDto.Quantity,
                    Price = itemDto.Price
                };

                order.OrderItems.Add(orderItem);
                order.TotalAmount += orderItem.Price * orderItem.Quantity;
            }

            var result = await _orderRepository.AddAsync(order);
            return _mapper.Map<OrderDto>(result);
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
    }
}
