namespace TiendaOnline.Frontend.Models;

/// <summary>
/// Carrito en curso (editable hasta confirmar compra).
/// </summary>
public class Carrito
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime Creado { get; set; } = DateTime.UtcNow;
    public bool Cerrado { get; set; }

    public List<CarritoItem> Items { get; set; } = new();
}