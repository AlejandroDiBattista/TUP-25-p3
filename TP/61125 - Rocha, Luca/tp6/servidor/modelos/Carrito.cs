using System.Collections.Generic;
using System.Linq;

public class Carrito
{
    public List<Producto> Productos { get; set; } = new();
    public decimal Total => Productos.Sum(p => p.Precio);
}