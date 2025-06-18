using System;
using System.Collections.Generic;

namespace servidor.Models;

public class Purchase
{
    public int Id { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public decimal Total { get; set; }

    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Email { get; set; } = "";

    public List<PurchaseItem> Items { get; set; } = new();
}