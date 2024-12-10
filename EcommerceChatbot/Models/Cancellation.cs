namespace EcommerceChatbot.Models
{
    public class Cancellation
    {
        public int CancellationId { get; set; }
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public DateTime CancellationDate { get; set; }
        public string Reason { get; set; }
        public Order Order { get; set; }

    }


}
