using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("Product")]
    public class Product
    {
        [Key] 
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string ProductCode { get; set; }
        [Required]
        [MaxLength(50)]
        public string ProductName { get; set; }
        public string Unit { get; set; }  // Đơn vị tính        
        public decimal PurchasePrice { get; set; } // Giá nhập
        public decimal SellingPrice { get; set; }  // Giá bán
        public decimal TaxRatePaid { get; set; } // Mức thuế phải chịu của sản phẩm (VD 8% 10%)
        public bool IsActive { get; set; } 
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
