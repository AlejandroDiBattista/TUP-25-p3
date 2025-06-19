using cliente.Modelos;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace cliente.Services
{
    public class ServicioCarrito
    {
        public event Action OnChange;
        public List<ItemCarrito> Items { get; set; } = new();

        // Constructor para debug
        public ServicioCarrito()
        {
            Console.WriteLine("🛒 ServicioCarrito creado");
        }

        private void NotificarCambio() => OnChange?.Invoke();

        public bool AgregarAlCarrito(Producto producto)
        {
            var item = Items.FirstOrDefault(i => i.Producto.Id == producto.Id);
            if (item != null)
            {
                if (item.Cantidad < producto.Stock)
                {
                    item.Cantidad++;
                    NotificarCambio();
                    return true;
                }
                else
                {
                    // Ya no hay más stock disponible para agregar
                    return false;
                }
            }
            else
            {
                if (producto.Stock > 0)
                {
                    Items.Add(new ItemCarrito { Producto = producto, Cantidad = 1 });
                    NotificarCambio();
                    return true;
                }
                else
                {
                    // No hay stock para agregar
                    return false;
                }
            }
        }

        public async Task<bool> AgregarAlCarritoAsync(Producto producto, HttpClient http)
        {
            var item = Items.FirstOrDefault(i => i.Producto.Id == producto.Id);
            int cantidadAReservar = 1;
            if (item != null)
                cantidadAReservar = 1; // O la cantidad que quieras sumar

            var response = await http.PostAsJsonAsync("/reservar", new { ProductoId = producto.Id, Cantidad = cantidadAReservar });
            if (!response.IsSuccessStatusCode)
                return false;

            if (item != null)
                item.Cantidad++;
            else
                Items.Add(new ItemCarrito { Producto = producto, Cantidad = 1 });

            NotificarCambio();
            return true;
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

        public async Task QuitarDelCarritoAsync(Producto producto, HttpClient http)
        {
            var item = Items.FirstOrDefault(i => i.Producto.Id == producto.Id);
            if (item != null)
            {
                await http.PostAsJsonAsync("/liberar", new { ProductoId = producto.Id, Cantidad = 1 });
                item.Cantidad--;
                if (item.Cantidad <= 0)
                    Items.Remove(item);
                NotificarCambio();
            }
        }

        public async Task<bool> SumarCantidadAsync(Producto producto, HttpClient http)
        {
            var item = Items.FirstOrDefault(i => i.Producto.Id == producto.Id);
            if (item != null && item.Cantidad < item.Producto.Stock)
            {
                var response = await http.PostAsJsonAsync("/reservar", new { ProductoId = producto.Id, Cantidad = 1 });
                if (!response.IsSuccessStatusCode)
                    return false;

                item.Cantidad++;
                OnChange?.Invoke();
                return true;
            }
            return false;
        }

        public async Task<bool> RestarCantidadAsync(Producto producto, HttpClient http)
        {
            var item = Items.FirstOrDefault(i => i.Producto.Id == producto.Id);
            if (item != null && item.Cantidad > 1)
            {
                var response = await http.PostAsJsonAsync("/liberar", new { ProductoId = producto.Id, Cantidad = 1 });
                if (!response.IsSuccessStatusCode)
                    return false;

                item.Cantidad--;
                OnChange?.Invoke();
                return true;
            }
            return false;
        }

        public async Task<bool> RemoverItemDelCarritoAsync(Producto producto, HttpClient http)
        {
            var item = Items.FirstOrDefault(i => i.Producto.Id == producto.Id);
            if (item == null)
                return false;

            var response = await http.PostAsJsonAsync("/liberar", new { ProductoId = producto.Id, Cantidad = item.Cantidad });
            if (!response.IsSuccessStatusCode)
                return false;

            Items.Remove(item);
            NotificarCambio();
            return true;
        }

        public void RemoverItemDelCarrito(Producto producto)
        {
            var item = Items.FirstOrDefault(i => i.Producto.Id == producto.Id);
            if (item != null)
            {
                Items.Remove(item);
                NotificarCambio();
            }
        }

        public void VaciarCarrito()
        {
            Items.Clear();
            NotificarCambio();
        }

        public async Task VaciarCarritoAsync(HttpClient http)
        {
            foreach (var item in Items.ToList())
            {
                await http.PostAsJsonAsync("/liberar", new { ProductoId = item.Producto.Id, Cantidad = item.Cantidad });
            }
            Items.Clear();
            NotificarCambio();
        }
    }
}
