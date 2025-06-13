using cliente.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace cliente.Services
{
    public class ServicioCarrito
    {
        public List<ItemCarrito> Items { get; set; } = new();

        public event Action OnChange;

        // 🚨 Constructor para debug
        public ServicioCarrito()
        {
            Console.WriteLine("🛒 ServicioCarrito creado");
        }

        private void NotificarCambio() => OnChange?.Invoke();

        public void AgregarAlCarrito(Producto producto)
        {
            var item = Items.FirstOrDefault(i => i.Producto.Id == producto.Id);
            if (item != null)
                item.Cantidad++;
            else
                Items.Add(new ItemCarrito { Producto = producto, Cantidad = 1 });
            NotificarCambio();
        }

        public void QuitarDelCarrito(Producto producto)
        {
            var item = Items.FirstOrDefault(i => i.Producto.Id == producto.Id);
            if (item != null)
            {
                item.Cantidad--;
                if (item.Cantidad <= 0)
                    Items.Remove(item);
                NotificarCambio();
            }
        }

        public void VaciarCarrito()
        {
            Items.Clear();
            NotificarCambio();
        }
    }
}
