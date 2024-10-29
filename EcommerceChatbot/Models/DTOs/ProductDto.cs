namespace EcommerceChatbot.Models.DTOs
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; } // Đường dẫn ảnh
        public string? CategoryName { get; set; } // Tên thể loại
    }
}
