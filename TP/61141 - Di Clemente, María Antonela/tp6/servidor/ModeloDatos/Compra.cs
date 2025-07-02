using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace servidor.ModeloDatos;

public class Compra
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; } = DateTime.Now;
    public decimal Total { get; set; }
    public string NombreCliente { get; set; } = null!;
    public string ApellidoCliente { get; set; } = null!;
    public string EmailCliente { get; set; } = null!;

    public List<ItemCompra> ItemsCompra { get; set; } = new List<ItemCompra>();
}
