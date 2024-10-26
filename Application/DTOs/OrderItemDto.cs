using Domain.Entities;

namespace Application.DTOs
{
    public class OrderItemDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TaxRate { get; set; }  // Tỉ lệ thuế
        public decimal TaxAmount { get; set; } // Số tiền thuế từng dòng
        
    }
}
