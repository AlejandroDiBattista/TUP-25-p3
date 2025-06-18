using System.Collections.Generic;
using tp6.cliente.Models;

public class CarritoService
{
    public List<ItemCarrito> Carrito { get; set; } = new List<ItemCarrito>();
    
    // Nueva propiedad para almacenar los productos
    public List<ProductoDto> Productos { get; set; } = new List<ProductoDto>();
}

public class ItemCarrito
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public decimal Precio { get; set; }
    public int Cantidad { get; set; }
    public int Stock { get; set; } 
}