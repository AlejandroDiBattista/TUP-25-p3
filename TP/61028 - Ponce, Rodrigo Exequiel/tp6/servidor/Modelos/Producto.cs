namespace servidor.Modelos
{
    public class Producto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Talle { get; set; } 
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public string ImagenUrl { get; set; } = string.Empty;

        public ICollection<ItemCompra> ItemsCompra { get; set; }
    }
}
