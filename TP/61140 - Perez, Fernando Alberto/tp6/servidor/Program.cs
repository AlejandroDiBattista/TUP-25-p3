using Microsoft.EntityFrameworkCore;
using TiendaOnline.Api.Models;
using Microsoft.EntityFrameworkCore;
using TiendaOnline.Api.Data;
using TiendaOnline.Api.Models;



namespace TiendaOnline.Api.Models;

public class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public string ImagenUrl { get; set; } = string.Empty;
}
namespace TiendaOnline.Api.Models;

public class Compra
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; } = DateTime.Now;
    public decimal Total { get; set; }
    public string NombreCliente { get; set; } = string.Empty;
    public string ApellidoCliente { get; set; } = string.Empty;
    public string EmailCliente { get; set; } = string.Empty;

    public List<ItemCompra> Items { get; set; } = new();
}

namespace TiendaOnline.Api.Models;

public class ItemCompra
{
    public int Id { get; set; }
    public int ProductoId { get; set; }
    public Producto Producto { get; set; } = null!;
    public int CompraId { get; set; }
    public Compra Compra { get; set; } = null!;
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
}