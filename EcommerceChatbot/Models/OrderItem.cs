using System;
using System.Collections.Generic;

namespace EcommerceChatbot.Models;

public partial class OrderItem
{
    public int OrderItemId { get; set; }

    public int? OrderId { get; set; }

    public int? ProductId { get; set; }
    public string ProductName { get; set; } // Tên sản phẩm
    public string ImageUrl { get; set; }   // URL hình ảnh

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal Subtotal { get; set; }

    public virtual Order? Order { get; set; }

    public virtual Product? Product { get; set; }

}
