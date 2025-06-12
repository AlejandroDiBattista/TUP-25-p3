using System.Collections.Generic;
using System; 

namespace Compartido
{
    public class Producto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public string Descripcion { get; set; } = "";
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public string ImagenUrl { get; set; } = "";
        public List<ItemCompra> ItemsCompra { get; set; } = new List<ItemCompra>();
    }

    public class Compra
    {
        public Guid Id { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public DateTime? FechaCompra { get; set; }
        public string? NombreCliente { get; set; }
        public string? ApellidoCliente { get; set; }
        public string? EmailCliente { get; set; }
        public decimal Total { get; set; }
        public List<ItemCompra> Items { get; set; } = new List<ItemCompra>();
    }

    public class ItemCompra
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public Producto? Producto { get; set; }
        public Guid CompraId { get; set; }
        public Compra? Compra { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; } 
    }

    public class CantidadRequest
    {
        public int Cantidad { get; set; }
    }

    public class ConfirmarCompraRequest
    {
        public string Nombre { get; set; } = "";
        public string Apellido { get; set; } = "";
        public string Email { get; set; } = "";
    }
}
