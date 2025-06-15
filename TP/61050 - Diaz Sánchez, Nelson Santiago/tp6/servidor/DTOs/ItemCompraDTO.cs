using System.Collections.Generic;


namespace servidor.DTOs {
    public class ItemCompraDTO
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public double PrecioUnitario { get; set; }
    }
}