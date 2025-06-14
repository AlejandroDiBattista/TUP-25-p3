#nullable enable 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using modelos_compartidos; 

namespace cliente.Services
{
    public class CarritoService
    {
        private const string CarritoKey = "carrito";
        private readonly ILocalStorageService _localStorage;

        public List<CarritoItem> Items { get; private set; } = new List<CarritoItem>();

        // Evento para notificar cambios en el carrito a los componentes
        public event Action? OnChange;

        public CarritoService(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
            
            _ = CargarCarritoDesdeLocalStorage(); 
        }

        public int TotalItemsEnCarrito => Items.Sum(item => item.Cantidad);
        public decimal TotalCarrito => Items.Sum(item => item.Producto.Precio * item.Cantidad);


        public void AgregarAlCarrito(Producto producto)
        {
            var itemExistente = Items.FirstOrDefault(i => i.Producto.Id == producto.Id);

            if (itemExistente != null)
            {
                
                if (itemExistente.Cantidad < producto.Stock)
                {
                    itemExistente.Cantidad++;
                    Console.WriteLine($"--> DEBUG (CS): Cantidad de {producto.Nombre} aumentada en carrito. Nueva cantidad: {itemExistente.Cantidad}");
                    GuardarCarritoEnLocalStorage(); 
                    NotifyChange(); 
                }
                else
                {
                    Console.WriteLine($"--> ADVERTENCIA (CS): No se puede añadir más {producto.Nombre}. Stock máximo alcanzado ({producto.Stock}).");
                }
            }
            else
            {
                
                if (producto.Stock > 0)
                {
                    Items.Add(new CarritoItem { Producto = producto, Cantidad = 1 });
                    Console.WriteLine($"--> DEBUG (CS): Agregado {producto.Nombre} al carrito por primera vez. Cantidad: 1");
                    GuardarCarritoEnLocalStorage(); 
                    NotifyChange(); 
                }
                else
                {
                    Console.WriteLine($"--> ADVERTENCIA (CS): No se puede añadir {producto.Nombre}. Sin stock disponible.");
                }
            }
        }

        // Método que remueve un producto del carrito
        public void RemoverDelCarrito(int productoId)
        {
            var itemExistente = Items.FirstOrDefault(i => i.Producto.Id == productoId);

            if (itemExistente != null)
            {
                if (itemExistente.Cantidad > 1)
                {
                    itemExistente.Cantidad--;
                    Console.WriteLine($"--> DEBUG (CS): Cantidad de producto {productoId} reducida. Nueva cantidad: {itemExistente.Cantidad}");
                }
                else
                {
                    Items.Remove(itemExistente);
                    Console.WriteLine($"--> DEBUG (CS): Producto {productoId} removido del carrito.");
                }
                GuardarCarritoEnLocalStorage();
                NotifyChange(); 
            }
        }

       
        public void VaciarCarrito()
        {
            Items.Clear();
            GuardarCarritoEnLocalStorage(); 
            Console.WriteLine("--> DEBUG (CS): Carrito vaciado.");
        }


        private async Task CargarCarritoDesdeLocalStorage()
        {
            try
            {
                var storedItems = await _localStorage.GetItemAsync<List<CarritoItem>>(CarritoKey);
                if (storedItems != null)
                {
                    Items = storedItems;
                    Console.WriteLine("[CarritoService] Carrito cargado desde localStorage.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CarritoService] Error al cargar carrito desde localStorage: {ex.Message}");
                Items = new List<CarritoItem>(); 
            }
            NotifyChange(); 
        }

     
        private void GuardarCarritoEnLocalStorage()
        {
            _localStorage.SetItemAsync(CarritoKey, Items);
            Console.WriteLine("[CarritoService] Carrito guardado en localStorage.");
        }

       
        private void NotifyChange() => OnChange?.Invoke();
    }
}
