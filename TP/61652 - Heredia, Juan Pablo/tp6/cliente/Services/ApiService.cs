using Microsoft.JSInterop;
using System.Net.Http.Json;
using cliente.models;
namespace cliente.Services;

public class ApiService
{
    private readonly HttpClient _http;
    public event Action? CarritoActualizado;
    private int _contadorCarrito;
    public int ContadorCarrito
    {
        get => _contadorCarrito;
        private set
        {
            if (_contadorCarrito != value)
            {
                _contadorCarrito = value;
                CarritoActualizado?.Invoke();
            }
        }
    }
    public ApiService(HttpClient http) => _http = http;
    public async Task<List<Producto>> GetProductos(string? busqueda = null)
        {
            var url = "/productos" + (string.IsNullOrWhiteSpace(busqueda) ? "" : $"?busqueda={busqueda}");
            return await _http.GetFromJsonAsync<List<Producto>>(url) ?? new();
        }
}

