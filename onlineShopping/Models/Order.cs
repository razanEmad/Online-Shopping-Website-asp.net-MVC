using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace onlineShopping.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        public DateTime OrderDate { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public decimal TotalAmount { get; set; }

        public string Status { get; set; } // e.g., "Paid", "Shipped"

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public string ShippingAddress { get; internal set; }
    }
}
