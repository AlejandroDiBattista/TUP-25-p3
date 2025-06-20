using System.Collections.Generic;
using tp6.cliente.Models;

public class CarritoService
{
    public string CarritoId { get; set; }
    public List<ItemCarrito> Carrito { get; set; } = new List<ItemCarrito>();
    public List<ProductoDto> Productos { get; set; } = new List<ProductoDto>();
}
