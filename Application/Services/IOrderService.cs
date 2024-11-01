﻿using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface IOrderService
    {
        Task<OrderDto?> GetOrderByCode(string code);
        Task<OrderDto?> GetOrderById(int id);
        Task<List<OrderDto>> GetAllOrderAsync();
        Task<(bool IsSuccess, string mess, OrderDto? orderDtoReturn)> UpdateOrderAsync(OrderDto orderDto);
        Task<OrderDto> AddOrderAsync(OrderDto model);
    }
}
