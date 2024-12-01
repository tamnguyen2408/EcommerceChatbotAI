namespace EcommerceChatbot.Models
{
    public class OrderDetailsViewModel
    {
        public int OrderId { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string PaymentMethod { get; set; }
        public string Notes { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderStatus { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public List<OrderItemViewModel> OrderItems { get; set; }
    }

    public class OrderItemViewModel
    {
        public string ProductName { get; set; }
        public string ImageUrl { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }
    }

}
