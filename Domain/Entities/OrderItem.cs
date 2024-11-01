﻿using System;
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
        public int ProductId { get; set; }
        public int OrderId { get; set; }
        public string ProductCode { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TaxRate { get; set; }  // Tỉ lệ thuế
        public decimal TaxAmount { get; set; } // Số tiền thuế từng dòng
        public decimal TotalAmountNoTax { get; set; } // số tiền của sản phẩm (đơn giá x số lượng) ko tính thuế
        public decimal TaxDeductibleAmount { get; set; } // Thành tiền đã trừ thuế
        public Order Order { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}
