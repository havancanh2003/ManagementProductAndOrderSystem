using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order> AddAsync(Order order);
        Task<Order> UpdateAsync(Order order);
        Task<List<Order>> GetAllAsync();
        Task<Order?> GetByIdAsync(int id);
        Task<Order?> GetByCodeAsync(string code);
    }
}
