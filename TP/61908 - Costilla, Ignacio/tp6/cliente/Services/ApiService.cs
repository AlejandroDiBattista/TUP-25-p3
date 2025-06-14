using System.Net.Http.Json;
using cliente.Modelos;

namespace cliente.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;

    public ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    
    public async Task<List<Producto>> GetProductosAsync(string? search = null)
    {
        var url = "/productos";
        if (!string.IsNullOrEmpty(search))
        {
            url += $"?search={search}";
        }
        return await _httpClient.GetFromJsonAsync<List<Producto>>(url);
    }

  
    public async Task<int> CrearCarritoAsync()
    {
        var response = await _httpClient.PostAsync("/carritos", null);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<int>();
    }

    public async Task<List<ItemCompra>> GetCarritoAsync(int carritoId)
    {
        return await _httpClient.GetFromJsonAsync<List<ItemCompra>>($"/carritos/{carritoId}");
    }

    public async Task AgregarAlCarritoAsync(int carritoId, int productoId)
    {
        await _httpClient.PutAsync($"/carritos/{carritoId}/{productoId}", null);
    }

    public async Task QuitarDelCarritoAsync(int carritoId, int productoId)
    {
        await _httpClient.DeleteAsync($"/carritos/{carritoId}/{productoId}");
    }

    public async Task VaciarCarritoAsync(int carritoId)
    {
        await _httpClient.DeleteAsync($"/carritos/{carritoId}");
    }

   
    public async Task ConfirmarCompraAsync(int carritoId, Compra cliente)
    {
        await _httpClient.PutAsJsonAsync($"/carritos/{carritoId}/confirmar", cliente);
    }
}