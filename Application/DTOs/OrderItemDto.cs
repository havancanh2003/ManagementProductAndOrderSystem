using Domain.Entities;

namespace Application.DTOs
{
    public class OrderItemDto
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public string ProductCode { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TaxRate { get; set; }  // Tỉ lệ thuế
        public decimal TaxAmount { get; set; } // Số thuế từng dòng = Giá bán x Tỉ lệ thuế
        public decimal TotalAmountNoTax { get; set; } // Giá bán từng dòng = Số lượng x Đơn giá
        public decimal TaxDeductibleAmount { get; set; } // Thành tiền từng dòng = Giá bán – Thuế

    }
}
