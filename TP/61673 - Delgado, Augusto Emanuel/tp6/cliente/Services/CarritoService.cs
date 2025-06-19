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

        public event Action? OnChange;
        public event Action? OnStockChange;

        public CarritoService(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
            
            _ = CargarCarritoDesdeLocalStorage(); 
        }

        public int TotalItemsEnCarrito => Items.Sum(item => item.Cantidad);
        public decimal TotalCarrito => Items.Sum(item => item.Producto.Precio * item.Cantidad);


        public void AgregarAlCarrito(Producto producto, List<Producto>? productos = null)
        {
            var itemExistente = Items.FirstOrDefault(i => i.Producto.Id == producto.Id);
            if (itemExistente != null)
            {
                if (itemExistente.Cantidad < producto.Stock)
                {
                    itemExistente.Cantidad++;
                    if (productos != null)
                    {
                        var prod = productos.FirstOrDefault(p => p.Id == producto.Id);
                        if (prod != null) prod.Stock--;
                    }
                    GuardarCarritoEnLocalStorage();
                    NotifyChange();
                    NotifyStockChange();
                }
            }
            else
            {
                if (producto.Stock > 0)
                {
                    Items.Add(new CarritoItem { Producto = producto, Cantidad = 1 });
                    if (productos != null)
                    {
                        var prod = productos.FirstOrDefault(p => p.Id == producto.Id);
                        if (prod != null) prod.Stock--;
                    }
                    GuardarCarritoEnLocalStorage();
                    NotifyChange();
                    NotifyStockChange();
                }
            }
        }

        public void RemoverDelCarrito(int productoId, List<Producto>? productos = null)
        {
            var itemExistente = Items.FirstOrDefault(i => i.Producto.Id == productoId);
            if (itemExistente != null)
            {
                if (itemExistente.Cantidad > 1)
                {
                    itemExistente.Cantidad--;
                    if (productos != null)
                    {
                        var prod = productos.FirstOrDefault(p => p.Id == productoId);
                        if (prod != null) prod.Stock++;
                    }
                }
                else
                {
                    Items.Remove(itemExistente);
                    if (productos != null)
                    {
                        var prod = productos.FirstOrDefault(p => p.Id == productoId);
                        if (prod != null) prod.Stock++;
                    }
                }
                GuardarCarritoEnLocalStorage();
                NotifyChange();
                NotifyStockChange();
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
        private void NotifyStockChange() => OnStockChange?.Invoke();
    }
}
