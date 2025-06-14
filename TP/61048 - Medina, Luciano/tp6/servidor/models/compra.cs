using System;
using System.Collections.Generic;

namespace servidor.Models
{
    public class Compra
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }

        // --- CORRECCIÓN AQUÍ ---
        // Hacemos que las propiedades del cliente puedan ser nulas
        public string? NombreCliente { get; set; }
        public string? ApellidoCliente { get; set; }
        public string? EmailCliente { get; set; }

        // Relación con Items de Compra
        public ICollection<ItemCompra> Items { get; set; } = new List<ItemCompra>();
    }
}