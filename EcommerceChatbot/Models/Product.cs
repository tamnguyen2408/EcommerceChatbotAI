using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EcommerceChatbot.Models;

public partial class Product
{
    public int ProductId { get; set; }

    [Required]
    [StringLength(100)]
    public string ProductName { get; set; } = null!;

    [StringLength(500)]
    public string? Description { get; set; }

    [Range(0.01, 10000)]
    public decimal Price { get; set; }

    [Range(0, 1000)]
    public int StockQuantity { get; set; }

    public int? CategoryId { get; set; }

    [Url]
    public string? ImageUrl { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual ProductCategory? Category { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
