using System.ComponentModel.DataAnnotations;

namespace onlineShopping.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string Address { get; set; }

        // Added property to resolve CS1061: view expects PhoneNumber on User model.
        [Phone]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        // Relationship: A user can have many items in their cart
        public virtual ICollection<CartItem> CartItems { get; set; }=new List<CartItem>();

        // Relationship: A user can have many orders
        public virtual ICollection<Order> Orders { get; set; }= new List<Order>();

        [Required]
        public string Role { get; set; } = "Customer"; // Default role is Customer
    }
}
