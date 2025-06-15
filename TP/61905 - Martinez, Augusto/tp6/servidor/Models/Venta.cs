using System;
using System.Collections.Generic;

namespace servidor.Models
{
public class Venta
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
    public decimal Total { get; set; }
    public string NombreCliente { get; set; } = string.Empty; // 🔥 Debe coincidir con VentaService.cs
    public string ApellidoCliente { get; set; } = string.Empty;
    public string EmailCliente { get; set; } = string.Empty; // 🔥 No debe ser ClienteEmail

    public List<VentaItem> Items { get; set; } = new List<VentaItem>();
}

}