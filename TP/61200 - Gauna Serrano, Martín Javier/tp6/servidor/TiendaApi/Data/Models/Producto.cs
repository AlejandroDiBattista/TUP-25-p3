
#nullable enable
    namespace TiendaApi.Models
{
    public class Producto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty; // Esta línea es necesaria
        public decimal Precio { get; set; }
        public int Stock { get; set; }
    }
}