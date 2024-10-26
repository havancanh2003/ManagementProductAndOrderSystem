using Application.DTOs;

namespace WebApi.Models.Order
{
    public class CreateOrUpdateOrder
    {
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public List<OrderItemModel> Items { get; set; } = new List<OrderItemModel>();
    }
}
