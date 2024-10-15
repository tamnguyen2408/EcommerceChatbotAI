using System.ComponentModel.DataAnnotations;

namespace EcommerceChatbot.Areas.Admin.Models
{
    public class CategoryDto
    {
        [Required(ErrorMessage = "Category Name is required.")]
        public string CategoryName { get; set; } = null!;
    }
}
