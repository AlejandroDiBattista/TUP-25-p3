namespace servidor.Models 
{
    public class CarritoItem
    {
        public int Id { get; set; }
        public Guid CarritoId { get; set; }
        public int ProductoId { get; set; }
        public Producto? Producto { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
       
        // Propiedad para calcular el total del item
        public decimal Total => Cantidad * PrecioUnitario;
    }
}


