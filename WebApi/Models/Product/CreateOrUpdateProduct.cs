namespace WebApi.Models.Product
{
    public class CreateOrUpdateProduct
    {
        public string ProductName { get; set; }
        public string Unit { get; set; }  // Đơn vị tính        
        public decimal PurchasePrice { get; set; } // Giá nhập
        public decimal SellingPrice { get; set; }  // Giá bán
        public decimal TaxRatePaid { get; set; } // Mức thuế phải chịu của sản phẩm (VD 8% 10%)
        public bool IsActive { get; set; }
    }
}
