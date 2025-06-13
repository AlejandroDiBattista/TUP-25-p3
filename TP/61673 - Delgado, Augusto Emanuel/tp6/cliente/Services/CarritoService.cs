using cliente.Models;
using System.Collections.Generic;
using System.Linq;
using System;
using Blazored.LocalStorage;
using System.Text.Json;

namespace cliente.Services
{
    public class CarritoService
    {
        private const string CarritoStorageKey = "carritoDeCompras";

        public List<CarritoItem> Items { get; private set; } = new List<CarritoItem>();

        public event Action? OnChange;

        public int TotalItemsEnCarrito => Items.Sum(item => item.Cantidad);
        public decimal TotalCarrito => Items.Sum(item => item.Producto.Precio * item.Cantidad);

        private readonly ILocalStorageService _localStorageService;

        public CarritoService(ILocalStorageService localStorageService)
        {
            _localStorageService = localStorageService;
            LoadCarritoFromLocalStorage();
        }

        private async void LoadCarritoFromLocalStorage()
        {
            try
            {
                var jsonCarrito = await _localStorageService.GetItemAsStringAsync(CarritoStorageKey);

                if (!string.IsNullOrEmpty(jsonCarrito))
                {
                    var loadedItems = JsonSerializer.Deserialize<List<CarritoItem>>(jsonCarrito);
                    if (loadedItems != null)
                    {
                        Items = loadedItems;
                        Console.WriteLine("[CarritoService] Carrito cargado desde localStorage.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CarritoService] Error al cargar carrito desde localStorage: {ex.Message}");
            }
            NotifyStateChanged();
        }

        private async void SaveCarritoToLocalStorage()
        {
            try
            {
                var jsonCarrito = JsonSerializer.Serialize(Items);
                await _localStorageService.SetItemAsStringAsync(CarritoStorageKey, jsonCarrito);
                Console.WriteLine("[CarritoService] Carrito guardado en localStorage.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CarritoService] Error al guardar carrito en localStorage: {ex.Message}");
            }
        }

        public void AgregarOActualizarEnCarrito(Producto producto)
        {
            var existingItem = Items.FirstOrDefault(item => item.Producto.Id == producto.Id);

            if (existingItem != null)
            {
                if (existingItem.Cantidad < producto.Stock)
                {
                    existingItem.Cantidad++;
                    Console.WriteLine($"[CarritoService] Incrementada cantidad de {producto.Nombre}. Cantidad: {existingItem.Cantidad}");
                }
                else
                {
                    Console.WriteLine($"[CarritoService] No hay suficiente stock para {producto.Nombre} (actual: {existingItem.Cantidad}, max: {producto.Stock})");
                }
            }
            else
            {
                if (producto.Stock > 0)
                {
                    Items.Add(new CarritoItem { Producto = producto, Cantidad = 1 });
                    Console.WriteLine($"[CarritoService] Agregado {producto.Nombre} al carrito por primera vez. Cantidad: 1");
                }
                else
                {
                    Console.WriteLine($"[CarritoService] Producto {producto.Nombre} sin stock disponible.");
                }
            }
            SaveCarritoToLocalStorage();
            NotifyStateChanged();
        }

        public void RemoverDelCarrito(int productoId)
        {
            var itemToRemove = Items.FirstOrDefault(item => item.Producto.Id == productoId);
            if (itemToRemove != null)
            {
                Items.Remove(itemToRemove);
                Console.WriteLine($"[CarritoService] Removido producto con Id: {productoId}");
                SaveCarritoToLocalStorage();
                NotifyStateChanged();
            }
        }

        public void VaciarCarrito()
        {
            Items.Clear();
            Console.WriteLine("[CarritoService] Carrito vaciado.");
            SaveCarritoToLocalStorage();
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}