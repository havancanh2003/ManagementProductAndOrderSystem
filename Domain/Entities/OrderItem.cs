using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    [Table("OrderItem")]
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TaxRate { get; set; }  // Tỉ lệ thuế
        public decimal TaxAmount { get; set; } // Số tiền thuế từng dòng

        public Order Order { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}
