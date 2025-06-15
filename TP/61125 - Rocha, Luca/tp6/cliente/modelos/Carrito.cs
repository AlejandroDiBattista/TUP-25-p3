namespace cliente.Modelos;

public class CarritoC
{
    public List<Producto> Productos { get; set; } = new List<Producto>();
    public decimal Total => Productos.Sum(p => p.Precio * p.Cantidad);
}