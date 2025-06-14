namespace cliente.Modelos
{
    public class CarritoItem
    {
        public Producto Producto { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario => Producto.Precio;
        public decimal Importe => Producto.Precio * Cantidad;
    }
}