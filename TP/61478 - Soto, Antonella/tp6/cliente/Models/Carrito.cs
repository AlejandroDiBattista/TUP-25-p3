namespace cliente.Models
{
    public class Carrito
    {
        public int Id { get; set; }
        public List<ItemCarrito> Items { get; set; } = new();
    }

    public class ItemCarrito
    {
        public int Id { get; set; }

        public int ProductoId { get; set; }  // Necesario para eliminar o actualizar

        public Producto Producto { get; set; } = new();
        public int Cantidad { get; set; }

        // Propiedades auxiliares para facilitar el acceso en Razor
        public string Nombre => Producto.Nombre;
        public string ImagenUrl => Producto.ImagenUrl;
        public decimal Precio => Producto.Precio;
    }
}
