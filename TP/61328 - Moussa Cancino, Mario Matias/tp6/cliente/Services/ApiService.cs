// cliente/Services/ApiService.cs

using cliente.Modelos; // <--- ESTA ES LA LÍNEA QUE LE FALTABA A TU ARCHIVO
using System.Net.Http.Json;
using System.Text.Json;

namespace cliente.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Producto[]?> GetProductosAsync(string? searchTerm = null)
    {
        var url = string.IsNullOrWhiteSpace(searchTerm) ? "api/productos" : $"api/productos?q={searchTerm}";
        return await _httpClient.GetFromJsonAsync<Producto[]>(url, _jsonOptions);
    }

    public async Task<Guid?> CreateCartAsync()
    {
        var response = await _httpClient.PostAsync("api/carritos", null);
        if (!response.IsSuccessStatusCode) return null;
        var result = await response.Content.ReadFromJsonAsync<CreateCartResponse>(_jsonOptions);
        return result?.CarritoId;
    }

    public async Task<List<CarritoItem>?> AddProductToCartAsync(Guid carritoId, int productoId)
    {
        var response = await _httpClient.PutAsync($"api/carritos/{carritoId}/{productoId}", null);
        return await response.Content.ReadFromJsonAsync<List<CarritoItem>>(_jsonOptions);
    }

    public async Task<List<CarritoItem>?> RemoveProductFromCartAsync(Guid carritoId, int productoId)
    {
        var response = await _httpClient.DeleteAsync($"api/carritos/{carritoId}/{productoId}");
        return await response.Content.ReadFromJsonAsync<List<CarritoItem>>(_jsonOptions);
    }
    
    public async Task EmptyCartAsync(Guid carritoId)
    {
        await _httpClient.DeleteAsync($"api/carritos/{carritoId}");
    }

    public async Task<Compra?> ConfirmPurchaseAsync(Guid carritoId, DatosClienteDto datosCliente)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/carritos/{carritoId}/confirmar", datosCliente);
        if(response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<Compra>(_jsonOptions);
        }
        return null;
    }
}