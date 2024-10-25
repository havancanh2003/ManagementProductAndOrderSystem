using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    [Table("Order")]
    public class Order
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string OrderCode { get; set; }
        [Required]
        [MaxLength(100)]
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TaxAmountOrder { get; set; }
        public DateTime CreatedDate { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
