
using EcommerceChatbot.Models;
using System.ComponentModel.DataAnnotations;

namespace ECommerceChatbot.Models
{
    public class CartItem
    {
        [Key]
        public int CartItemID { get; set; }

        [Required]
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }
        [Required]
        public int UserId { get; set; } // Thêm thuộc tính này để liên kết với User
        public int Quantity { get; set; }

        // Navigation property for Cart
        public Cart Cart { get; set; }

        // Navigation property for Product
        public Product Product { get; set; }
        public decimal Price { get; set; }
        public string ProductName { get; set; }

        public string ImageUrl { get; set; }

    }
}