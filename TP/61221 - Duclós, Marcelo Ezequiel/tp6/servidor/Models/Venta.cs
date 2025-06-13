using System;
using System.Collections.Generic;

namespace servidor.Models;

public class Venta
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public decimal Total { get; set; }

    public string NombreCliente { get; set; }
    public string ApellidoCliente { get; set; }
    public string EmailCliente { get; set; }

    
    public List<DetalleVenta> Detalles { get; set; } = new();
}