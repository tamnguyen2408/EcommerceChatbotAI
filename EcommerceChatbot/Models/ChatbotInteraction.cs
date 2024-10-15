using System;
using System.Collections.Generic;

namespace EcommerceChatbot.Models;

public partial class ChatbotInteraction
{
    public int InteractionId { get; set; }

    public int? UserId { get; set; }

    public string Query { get; set; } = null!;

    public string Response { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual User? User { get; set; }
}
