using System;
using System.Collections.Generic;
using System.Linq;
using Cliente.Modelos;

namespace Cliente.Services
{
    public class CarritoFinal
    {
        private List<ItemCarrito> items = new List<ItemCarrito>();

        public event Action OnChange;
        public IReadOnlyList<ItemCarrito> Items => items;
        public int TotalItems => items.Sum(i => i.Cantidad);

        public Guid CarritoId { get; set; } = Guid.NewGuid();

        public async Task AgregarProductoAsync(ItemCarrito item)
        {
            var existente = items.FirstOrDefault(i => i.ProductoId == item.ProductoId);
            if (existente != null)
            {
                existente.Cantidad += 1; 
            }
            else
            {
                item.Cantidad = 1; 
                items.Add(item);
            }

            Console.WriteLine($"Producto agregado. TotalItems: {TotalItems}");
            OnChange?.Invoke();
        }

        public bool PuedeAgregar(int productoId, int stockMaximo)
        {
            var item = items.FirstOrDefault(i => i.ProductoId == productoId);
            return item == null || item.Cantidad < stockMaximo;
        }
        public void Vaciar()
        {
            items.Clear();
            Console.WriteLine("Carrito vaciado.");
            OnChange?.Invoke();
        }
        public int CantidadEnCarrito(int productoId)
        {
            return items.FirstOrDefault(i => i.ProductoId == productoId)?.Cantidad ?? 0;
        }
}
}