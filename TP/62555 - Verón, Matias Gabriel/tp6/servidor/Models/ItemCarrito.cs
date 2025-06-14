using Servidor.Models;

namespace Servidor.Models;
public class ItemCarrito
{
    public int Id { get; set; }
    public int ProductoId { get; set; }
    public int CarritoId { get; set; }
    public int Cantidad { get; set; }
    public Producto Producto { get; set; }

}