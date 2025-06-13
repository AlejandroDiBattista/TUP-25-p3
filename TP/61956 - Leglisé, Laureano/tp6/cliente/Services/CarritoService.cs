using System.Net.Http.Json;
using System.Web;
using cliente.Models;

namespace cliente.Services;

public class CarritoService
{
    private readonly HttpClient _http;
    private readonly Dictionary<int, (Producto Producto, int Cantidad)> carrito = new();
    public string CarritoId { get; private set; } = string.Empty;
    public event Action? OnChange;

    public CarritoService(HttpClient http)
    {
        _http = http;
        // Asegúrate de que la base URL esté configurada
        if (_http.BaseAddress == null)
        {
            _http.BaseAddress = new Uri("http://localhost:5184/"); // Ajusta según tu configuración
        }
    }

    public async Task InitCarritoAsync()
    {
        try
        {
            var resp = await _http.PostAsJsonAsync("carritos", new { }); // Envía objeto vacío en lugar de string
            resp.EnsureSuccessStatusCode(); // Lanza excepción si hay error HTTP
            
            var obj = await resp.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();

            if (obj.TryGetProperty("carritoId", out var id))
                CarritoId = id.GetString() ?? Guid.NewGuid().ToString();
            else
                throw new Exception("Respuesta inválida del servidor al inicializar carrito.");
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"Error de conectividad: {ex.Message}");
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al inicializar carrito: {ex.Message}");
        }
    }

    public async Task AgregarProductoAsync(Producto producto, int cantidad)
    {
        try
        {
            if (string.IsNullOrEmpty(CarritoId))
                await InitCarritoAsync();

            if (carrito.ContainsKey(producto.Id))
                carrito[producto.Id] = (producto, carrito[producto.Id].Cantidad + cantidad);
            else
                carrito[producto.Id] = (producto, cantidad);

            // Usar interpolación de string más limpia
            var url = $"carritos/{CarritoId}/{producto.Id}?cantidad={cantidad}";
            var response = await _http.PutAsync(url, null);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error del servidor: {response.StatusCode} - {errorContent}");
            }

            OnChange?.Invoke();
        }
        catch (Exception ex)
        {
            // Log del error y re-lanzar
            Console.WriteLine($"Error agregando producto: {ex.Message}");
            throw;
        }
    }

    public async Task ModificarCantidadAsync(int productoId, int nuevaCantidad)
    {
        try
        {
            if (string.IsNullOrEmpty(CarritoId)) return;

            if (carrito.ContainsKey(productoId))
            {
                if (nuevaCantidad <= 0)
                {
                    carrito.Remove(productoId);
                    var deleteResponse = await _http.DeleteAsync($"carritos/{CarritoId}/{productoId}");
                    deleteResponse.EnsureSuccessStatusCode();
                }
                else
                {
                    var producto = carrito[productoId].Producto;
                    carrito[productoId] = (producto, nuevaCantidad);
                    var putResponse = await _http.PutAsync($"carritos/{CarritoId}/{productoId}?cantidad={nuevaCantidad}", null);
                    putResponse.EnsureSuccessStatusCode();
                }

                OnChange?.Invoke();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error modificando cantidad: {ex.Message}");
            throw;
        }
    }

    public async Task QuitarProductoAsync(int productoId)
    {
        try
        {
            if (string.IsNullOrEmpty(CarritoId)) return;

            if (carrito.ContainsKey(productoId))
            {
                carrito.Remove(productoId);
                var response = await _http.DeleteAsync($"carritos/{CarritoId}/{productoId}");
                response.EnsureSuccessStatusCode();
                OnChange?.Invoke();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error quitando producto: {ex.Message}");
            throw;
        }
    }

    public async Task VaciarCarritoAsync()
    {
        try
        {
            if (string.IsNullOrEmpty(CarritoId)) return;

            carrito.Clear();
            var response = await _http.DeleteAsync($"carritos/{CarritoId}");
            response.EnsureSuccessStatusCode();
            OnChange?.Invoke();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error vaciando carrito: {ex.Message}");
            throw;
        }
    }

    public async Task ConfirmarCompraAsync(string nombre, string apellido, string email)
    {
        try
        {
            if (string.IsNullOrEmpty(CarritoId)) return;

            var data = new
            {
                NombreCliente = nombre,
                ApellidoCliente = apellido,
                EmailCliente = email
            };

            var resp = await _http.PutAsJsonAsync($"carritos/{CarritoId}/confirmar", data);
            resp.EnsureSuccessStatusCode();

            carrito.Clear();
            CarritoId = string.Empty;
            OnChange?.Invoke();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error confirmando compra: {ex.Message}");
            throw;
        }
    }

    public async Task CargarCarritoDesdeServidor()
    {
        try
        {
            if (string.IsNullOrEmpty(CarritoId)) return;

            var respuesta = await _http.GetFromJsonAsync<List<ItemCarritoDTO>>($"carritos/{CarritoId}");
            carrito.Clear();

            if (respuesta != null)
            {
                foreach (var item in respuesta)
                {
                    carrito[item.Producto.Id] = (item.Producto, item.Cantidad);
                }
            }

            OnChange?.Invoke();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error cargando carrito: {ex.Message}");
            throw;
        }
    }

    public List<(Producto, int)> ObtenerItems() => carrito.Values.ToList();

    public int TotalItems => carrito.Values.Sum(i => i.Item2);

    public decimal TotalImporte => carrito.Values.Sum(i => i.Item1.Precio * i.Item2);
}
    
public class ItemCarritoDTO
{
    public Producto Producto { get; set; } = default!;
    public int Cantidad { get; set; }
}