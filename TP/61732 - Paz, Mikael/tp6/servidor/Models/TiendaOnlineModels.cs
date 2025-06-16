namespace servidor.Models
{
    public class Producto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public string ImagenUrl { get; set; } = string.Empty;
        public List<ItemCompra>? ItemsCompra { get; set; }
    }

    public class Compra
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public decimal Total { get; set; }
        public string NombreCliente { get; set; } = string.Empty;
        public string ApellidoCliente { get; set; } = string.Empty;
        public string EmailCliente { get; set; } = string.Empty;
        public List<ItemCompra>? Items { get; set; }
    }

    public class ItemCompra
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public Producto? Producto { get; set; }
        public int CompraId { get; set; }
        public Compra? Compra { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }
}
