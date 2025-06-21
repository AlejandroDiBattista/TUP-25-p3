namespace cliente.Models
{
   
    public class CarritoDto
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int Precio { get; set; }
    }

}