using System.ComponentModel.DataAnnotations;

namespace EcommerceChatbot.Models
{
    public partial class ProductCategory
    {
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Category Name is required.")]
        public string CategoryName { get; set; } = null!;

        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
