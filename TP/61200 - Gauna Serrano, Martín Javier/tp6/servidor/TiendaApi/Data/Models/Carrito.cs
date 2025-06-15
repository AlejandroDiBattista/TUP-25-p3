using System;
using System.Collections.Generic;
using System.Linq;

namespace TiendaApi.Models
{
    public class Carrito
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public List<ItemCarrito> Items { get; set; } = new List<ItemCarrito>();

        public void AgregarItem(int productoId, string nombre, decimal precioUnitario, int cantidad)
        {
            var itemExistente = Items.FirstOrDefault(i => i.ProductoId == productoId);
            if (itemExistente != null)
            {
                itemExistente.Cantidad += cantidad;
            }
            else
            {
                Items.Add(new ItemCarrito
                {
                    ProductoId = productoId,
                    Nombre = nombre,
                    PrecioUnitario = precioUnitario,
                    Cantidad = cantidad
                });
            }
        }

        public void EliminarItem(int productoId)
        {
            var item = Items.FirstOrDefault(i => i.ProductoId == productoId);
            if (item != null)
            {
                Items.Remove(item);
            }
        }

        // Método para confirmar la compra: retorna el total y vacía el carrito
        public decimal ConfirmarCompra()
        {
            decimal total = Items.Sum(i => i.PrecioUnitario * i.Cantidad);
            Items.Clear();
            return total;
        }
    }

    public class ItemCarrito
    {
        public int ProductoId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal PrecioUnitario { get; set; }
        public int Cantidad { get; set; }
    }

}