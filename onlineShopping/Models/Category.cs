using System.ComponentModel.DataAnnotations;

namespace onlineShopping.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Category name is required")]
        [StringLength(100)]
        public string? Name { get; set; }

        public virtual ICollection<Product>? Products { get; set; }
    }
}
