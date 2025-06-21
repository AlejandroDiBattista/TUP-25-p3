using System.Net.Http.Json;
using System.Text.Json;
using cliente.Models;
using Microsoft.JSInterop;

namespace cliente.Services
{
    public class CarritoService
    {
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _jsRuntime;
        private readonly List<ItemCarrito> _items = new();

        private const string LocalStorageKey = "carrito";

        public event Action? OnChange;
        public Action<int, int>? OnStockActualizar; // productoId, cambio (positivo o negativo)

        public CarritoService(HttpClient httpClient, IJSRuntime jsRuntime)
        {
            _httpClient = httpClient;
            _jsRuntime = jsRuntime;
        }

        /// <summary>
        /// Inicializa el carrito cargando los datos desde LocalStorage
        /// </summary>
        public async Task InicializarAsync()
        {
            var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", LocalStorageKey);
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    var items = JsonSerializer.Deserialize<List<ItemCarrito>>(json);
                    if (items != null)
                    {
                        _items.Clear();
                        _items.AddRange(items);
                    }
                }
                catch
                {
                    // Si no pudo deserializar, ignorar y dejar el carrito vacío
                }
            }
            OnChange?.Invoke();
        }

        public IReadOnlyList<ItemCarrito> ObtenerItems() => _items.AsReadOnly();

        public int ObtenerCantidadTotal() => _items.Sum(x => x.Cantidad);

        public decimal ObtenerTotal() => _items.Sum(x => x.Subtotal);

        public async void AgregarItem(ProductoDTO producto, int cantidad = 1)
        {
            if (cantidad <= 0) return;

            var item = _items.FirstOrDefault(x => x.ProductoId == producto.Id);

            if (item == null)
            {
                int cantidadFinal = Math.Min(cantidad, producto.Stock);
                item = new ItemCarrito
                {
                    ProductoId = producto.Id,
                    Nombre = producto.Nombre,
                    Precio = producto.Precio,
                    Cantidad = cantidadFinal
                };
                _items.Add(item);
                OnStockActualizar?.Invoke(producto.Id, -cantidadFinal);
            }
            else
            {
                int nuevaCantidad = Math.Min(item.Cantidad + cantidad, producto.Stock);
                int diferencia = nuevaCantidad - item.Cantidad;
                item.Cantidad = nuevaCantidad;
                OnStockActualizar?.Invoke(producto.Id, -diferencia);
            }

            await GuardarCarritoAsync();
            NotificarCambios();
        }

        public async void ActualizarCantidad(int productoId, int cantidad)
        {
            var item = _items.FirstOrDefault(x => x.ProductoId == productoId);
            if (item != null)
            {
                if (cantidad <= 0)
                {
                    OnStockActualizar?.Invoke(productoId, item.Cantidad);
                    _items.Remove(item);
                }
                else
                {
                    int diferencia = cantidad - item.Cantidad;
                    item.Cantidad = cantidad;
                    OnStockActualizar?.Invoke(productoId, -diferencia);
                }

                await GuardarCarritoAsync();
                NotificarCambios();
            }
        }

        public async void EliminarItem(int productoId)
        {
            var item = _items.FirstOrDefault(x => x.ProductoId == productoId);
            if (item != null)
            {
                OnStockActualizar?.Invoke(productoId, item.Cantidad);
                _items.Remove(item);

                await GuardarCarritoAsync();
                NotificarCambios();
            }
        }

        public async void LimpiarCarrito()
        {
            foreach (var item in _items)
            {
                OnStockActualizar?.Invoke(item.ProductoId, item.Cantidad);
            }
            _items.Clear();

            await GuardarCarritoAsync();
            NotificarCambios();
        }

        private async Task GuardarCarritoAsync()
        {
            var json = JsonSerializer.Serialize(_items);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", LocalStorageKey, json);
        }

        public async Task<bool> ProcesarCompraAsync(string nombre, string apellido, string email)
        {
            if (!_items.Any()) return false;

            try
            {
                var orden = new OrdenCompraDTO
                {
                    Items = _items.Select(x => new ItemOrdenDTO
                    {
                        ProductoId = x.ProductoId,
                        Cantidad = x.Cantidad
                    }).ToList()
                };

                var response = await _httpClient.PostAsJsonAsync("/api/compras", new
                {
                    Items = orden.Items,
                    NombreCliente = nombre,
                    ApellidoCliente = apellido,
                    EmailCliente = email
                });

                if (response.IsSuccessStatusCode)
                {
                    LimpiarCarrito();
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al procesar la compra: {ex.Message}");
                return false;
            }
        }

        private void NotificarCambios() => OnChange?.Invoke();
    }
}
