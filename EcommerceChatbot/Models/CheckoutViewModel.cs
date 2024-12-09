using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EcommerceChatbot.Models
{
    public class CheckoutViewModel
    {
        [Required(ErrorMessage = "Full name is required.")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number.")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Please select a payment method.")]
        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; }
        // Add PaymentMethodId for Stripe payment integration
        public string PaymentMethodId { get; set; }

        [Display(Name = "Additional Notes")]
        public string Notes { get; set; }

        // Summary of cart items
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();

        // Total amount of the order
        public decimal TotalAmount { get; set; }
    }

    // Cart item model
    public class CartItem
    {
        public int CartItemID { get; set; }  // CartItemID để làm primary key
        public int Id { get; set; }  // ID của Cart
        public int ProductId { get; set; }  // ID của sản phẩm
        public int UserId { get; set; }  // ID người dùng, nếu có cần thiết cho liên kết người dùng
        public int Quantity { get; set; }  // Số lượng sản phẩm trong giỏ hàng

        // Navigation property for Cart (nếu cần thiết)
        public Cart Cart { get; set; }

        // Navigation property for Product (nếu cần thiết)
        public Product Product { get; set; }

        public decimal Price { get; set; }  // Giá của sản phẩm
        public string ProductName { get; set; }  // Tên sản phẩm
        public string ImageUrl { get; set; }  // Đường dẫn hình ảnh sản phẩm (nếu có)
    }
}
