namespace cliente.Modelos;

    public class ItemCarrito
    {
        public Producto Producto { get; set; } = new Producto();
        public int Cantidad { get; set; }
        
        public decimal Importe => Producto.Precio * Cantidad;
    }

