using System.Text.Json.Serialization;

namespace TiendaOnline.Frontend.Models;
/// <summary>
/// Contiene los productos en el carrito de compras.
/// </summary>
public class CarritoItem
{
    public int Id { get; set; }

    public Guid CarritoId { get; set; }
    [JsonIgnore]
    public Carrito? Carrito { get; set; }
    public int ProductoId { get; set; }
    public Producto? Producto { get; set; }

    public int Cantidad { get; set; }
}