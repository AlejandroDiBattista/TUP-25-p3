namespace tp6.cliente.Models
{
    public class ItemCarrito
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
        public int Stock { get; set; }
        public string ImagenUrl { get; set; }
    }
}