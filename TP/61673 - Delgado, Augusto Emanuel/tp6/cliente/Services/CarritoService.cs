using cliente.Models;
using System.Collections.Generic; // Asegúrate de tener este using
using System.Linq; // Asegúrate de tener este using
using System; // Asegúrate de tener este using

namespace cliente.Services
{
    public class CarritoService
    {
        public List<CarritoItem> Items { get; private set; } = new List<CarritoItem>();

        // Evento para notificar a los componentes cuando el carrito cambie
        public event Action? OnChange; // <--- CRÍTICO: Este evento debe estar aquí

        public int TotalItemsEnCarrito => Items.Sum(item => item.Cantidad);
        public decimal TotalCarrito => Items.Sum(item => item.Producto.Precio * item.Cantidad);

        public void AgregarOActualizarEnCarrito(Producto producto)
        {
            var existingItem = Items.FirstOrDefault(item => item.Producto.Id == producto.Id);

            if (existingItem != null)
            {
                if (existingItem.Cantidad < producto.Stock)
                {
                    existingItem.Cantidad++;
                    Console.WriteLine($"[CarritoService] Incrementada cantidad de {producto.Nombre}. Cantidad: {existingItem.Cantidad}"); // Añadido para depuración
                }
                else
                {
                    Console.WriteLine($"[CarritoService] No hay suficiente stock para {producto.Nombre} (actual: {existingItem.Cantidad}, max: {producto.Stock})"); // Añadido para depuración
                }
            }
            else
            {
                if (producto.Stock > 0)
                {
                    Items.Add(new CarritoItem { Producto = producto, Cantidad = 1 });
                    Console.WriteLine($"[CarritoService] Agregado {producto.Nombre} al carrito por primera vez. Cantidad: 1"); // Añadido para depuración
                }
                else
                {
                    Console.WriteLine($"[CarritoService] Producto {producto.Nombre} sin stock disponible."); // Añadido para depuración
                }
            }
            NotifyStateChanged(); // <--- CRÍTICO: Esto debe llamarse al final de cada operación de modificación
        }

        public void RemoverDelCarrito(int productoId)
        {
            var itemToRemove = Items.FirstOrDefault(item => item.Producto.Id == productoId);
            if (itemToRemove != null)
            {
                Items.Remove(itemToRemove);
                Console.WriteLine($"[CarritoService] Removido producto con Id: {productoId}"); // Añadido para depuración
                NotifyStateChanged();
            }
        }

        public void VaciarCarrito()
        {
            Items.Clear();
            Console.WriteLine("[CarritoService] Carrito vaciado."); // Añadido para depuración
            NotifyStateChanged();
        }

        // Método para notificar cambios a los suscriptores
        private void NotifyStateChanged() => OnChange?.Invoke(); // <--- CRÍTICO: Este método dispara el evento
    }
}