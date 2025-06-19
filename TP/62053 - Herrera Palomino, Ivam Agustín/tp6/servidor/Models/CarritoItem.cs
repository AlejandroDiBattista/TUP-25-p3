using System.Text.Json.Serialization;

namespace TiendaOnline.Backend.Models;

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