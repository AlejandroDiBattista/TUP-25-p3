using System;
using servidor.Data;

namespace servidor.Entidades
{
    public class ItemCompra
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public Producto Producto { get; set; }
        public int Cantidad { get; set; }
        public double PrecioUnitario { get; set; }
        public int CompraId { get; set; }
        public Compra Compra { get; set; }
    }
}
