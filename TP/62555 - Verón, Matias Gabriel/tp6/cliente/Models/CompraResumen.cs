
namespace cliente.Models;

using System;

public class CompraResumen
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public decimal Total { get; set; }
    public string NombreCliente { get; set; }
    public string ApellidoCliente { get; set; }
    public string EmailCliente { get; set; }
    public List<DetalleCompraResumen> Detalles { get; set; }
}
