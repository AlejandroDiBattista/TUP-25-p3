namespace servidor.Models
{
    public class Producto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public int Stock { get; set; }
        public decimal Precio { get; set; }
        public string UrlImagen { get; set; } = string.Empty;
    }
}

