using System;
using System.Collections.Generic;

public class Orden
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; } = DateTime.Now;
    public List<ItemOrden> Items { get; set; } = new();
    public decimal Total { get; set; }
}

public class ItemOrden
{
    public int Id { get; set; }
    public int ProductoId { get; set; }
    public string NombreProducto { get; set; }
    public decimal Precio { get; set; }
    public int Cantidad { get; set; }
}
