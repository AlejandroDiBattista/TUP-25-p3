namespace Servidor.Models;

public class ItemCarrito
{
    public int Id { get; set; }

    public int ProductoId { get; set; }
    public Producto? Producto { get; set; }

    public int CarritoId { get; set; }
    public Carrito? Carrito { get; set; }

    public int Cantidad { get; set; }


    public decimal Total => Producto?.Precio * Cantidad ?? 0; // Calcula el total del item
}