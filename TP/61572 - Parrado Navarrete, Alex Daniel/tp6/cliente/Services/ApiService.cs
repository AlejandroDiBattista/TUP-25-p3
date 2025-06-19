using System.Net.Http.Json;
using cliente.Models;

namespace cliente.Services;

public class ApiService
{
    private readonly HttpClient _http;
    public int CurrentCartId { get; private set; }

    public ApiService(HttpClient http) => _http = http;

    public async Task<List<Producto>> GetProductsAsync(string? q = null)
    {
        var uri = string.IsNullOrEmpty(q) ? "/productos" 
                  : $"/productos?search={Uri.EscapeDataString(q)}";
        return await _http.GetFromJsonAsync<List<Producto>>(uri) ?? new();
    }

    public async Task<int> CreateCartAsync()
    {
        var resp = await _http.PostAsync("/carritos", null);
        resp.EnsureSuccessStatusCode();
        var loc = resp.Headers.Location!.Segments.Last();
        CurrentCartId = int.Parse(loc);
        return CurrentCartId;
    }

    public async Task<List<ItemCompra>> GetCartItemsAsync()
    {
        if (CurrentCartId == 0) await CreateCartAsync();
        return await _http.GetFromJsonAsync<List<ItemCompra>>($"/carritos/{CurrentCartId}") ?? new();
    }

    public Task AddOrUpdateItemAsync(int prodId, int qty) =>
        _http.PutAsJsonAsync($"/carritos/{CurrentCartId}/{prodId}", new { Cantidad = qty });

    public Task RemoveItemAsync(int prodId) =>
        _http.DeleteAsync($"/carritos/{CurrentCartId}/{prodId}");

    public Task EmptyCartAsync() =>
        _http.DeleteAsync($"/carritos/{CurrentCartId}");

    public Task ConfirmCartAsync(Compra cliente) =>
        _http.PutAsJsonAsync($"/carritos/{CurrentCartId}/confirmar", cliente);
}