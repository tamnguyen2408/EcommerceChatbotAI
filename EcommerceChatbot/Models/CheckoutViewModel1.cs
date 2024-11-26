using EcommerceChatbot.Controllers;

namespace EcommerceChatbot.Models
{
    public class CheckoutViewModel1
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public decimal TotalPrice { get; set; }
        public List<CartItem> CartItems { get; set; }
    }
}
