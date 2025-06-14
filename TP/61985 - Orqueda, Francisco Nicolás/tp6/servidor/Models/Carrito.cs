using System;
using System.Collections.Generic;
using servidor.Models;

namespace Servidor.Models
{
    public class Carrito
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public List<CarritoItem> Items { get; set; } = new();
        public bool Confirmado { get; set; } = false;
    }

    public class CarritoItem
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public Producto Producto { get; set; }
        public int Cantidad { get; set; }
    }
}