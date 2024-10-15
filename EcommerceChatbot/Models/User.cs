using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EcommerceChatbot.Models;

public partial class User
{
    public int UserId { get; set; }

    [Required]
    [StringLength(100)] // Set max length for username
    public string UserName { get; set; }

    [Required]
    [EmailAddress] // Validate email format
    public string Email { get; set; }

    [Required]
    [StringLength(100)] // Set max length for password
    public string Password { get; set; }

    [Phone] // Validate phone number format
    public string Phone { get; set; }
    [Required]
    public string Role { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<ChatbotInteraction> ChatbotInteractions { get; set; } = new List<ChatbotInteraction>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
