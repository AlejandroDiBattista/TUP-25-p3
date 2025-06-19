namespace Cliente.Modelo
{
#nullable enable
    public class ItemCarritoDto
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }

        // SIN ESTO NO SE MUESTRA EN RAZOR
        public Producto? Producto { get; set; }
    }
}

