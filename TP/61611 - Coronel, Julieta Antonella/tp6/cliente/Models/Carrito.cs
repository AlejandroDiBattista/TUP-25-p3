namespace cliente.Models
{
    public class Carrito
    {
        public int Cantidad { get; set; }
        public int ProductoId { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
    }
}