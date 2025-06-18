using System.Net.Http.Json;
using cliente.Modelos;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace cliente.Services;

public class CarritoService
{
    private readonly HttpClient _http;
    private Guid carritoId;

    public CarritoService(HttpClient http)
    {
        _http = http ?? throw new ArgumentNullException(nameof(http));
    }

    public async Task<Guid> ObtenerCarritoIdAsync()
    {
        if (carritoId == Guid.Empty)
        {
            var response = await _http.PostAsync("http://localhost:5184/api/carritos", null);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"❌ Error al obtener carrito: {response.StatusCode}");
                return Guid.NewGuid();
            }

            var data = await response.Content.ReadFromJsonAsync<CarritoRespuesta>();
            carritoId = data?.CarritoId ?? Guid.NewGuid();
            Console.WriteLine($"🛒 Carrito creado/restaurado con ID: {carritoId}");
        }

        return carritoId;
    }

    public async Task<bool> AgregarProductoAsync(int productoId)
    {
        var id = await ObtenerCarritoIdAsync();
        var response = await _http.PutAsync($"http://localhost:5184/api/carritos/{id}/productos/{productoId}", null);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> ConfirmarCompraAsync()
    {
        var id = await ObtenerCarritoIdAsync();
        var response = await _http.PostAsync($"http://localhost:5184/api/carritos/{id}/confirmar", null);

        return response.IsSuccessStatusCode;
    }

    private class CarritoRespuesta
    {
        public Guid CarritoId { get; set; }
    }
}