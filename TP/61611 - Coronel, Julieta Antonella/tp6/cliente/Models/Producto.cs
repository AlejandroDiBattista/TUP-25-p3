namespace cliente.Models
{
    public class Producto
    {
        public int Id { get; set; }
        // public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int Precio { get; set; }
        public string ImagenUrl { get; set; }
        public int Stock { get; set; }
    }
}
