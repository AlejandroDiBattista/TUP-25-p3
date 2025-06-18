// Modelo de compra (carrito o compra confirmada)
using System;
using System.Collections.Generic;

public class Compra
{
    public int Id { get; set; } // Identificador único
    public DateTime Fecha { get; set; } // Fecha de creación
    public string NombreCliente { get; set; } = ""; // Nombre del cliente
    public string ApellidoCliente { get; set; } = ""; // Apellido del cliente
    public string EmailCliente { get; set; } = ""; // Email del cliente
    public decimal Total { get; set; } // Total de la compra
    public List<ItemCompra> Items { get; set; } = new List<ItemCompra>(); // Ítems
}
