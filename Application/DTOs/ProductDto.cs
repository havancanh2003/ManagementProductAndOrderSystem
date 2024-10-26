using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string Unit { get; set; }  // Đơn vị tính        
        public decimal PurchasePrice { get; set; } // Giá nhập
        public decimal SellingPrice { get; set; }  // Giá bán
        public decimal TaxRatePaid { get; set; } // Mức thuế phải chịu của sản phẩm (VD 8% 10%)
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
