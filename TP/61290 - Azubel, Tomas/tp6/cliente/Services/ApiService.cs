using cliente.Models;
using cliente.Pages;
using System.Net.Http;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace cliente.Services;

public class ApiService
{
    private readonly HttpClient _http;
    private readonly string _apiBaseUrl = "http://localhost:5184";
    public ApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<Product>> GetProductsAsync(string? search = null)
{
    var url = $"{_apiBaseUrl}/products";
    if (!string.IsNullOrWhiteSpace(search))
    {
        url += $"?search={search}";
    }

    var products = await _http.GetFromJsonAsync<List<Product>>(url);
    return products ?? new List<Product>();
}

    public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _http.GetFromJsonAsync<Product>($"{_apiBaseUrl}/products/{id}");
        }

    public async Task<string> CreateCartAsync()
    {
        var response = await _http.PostAsync($"{_apiBaseUrl}/carts", null);
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<Dictionary<int, int>> GetCartAsync(string cartId)
{
    return await _http.GetFromJsonAsync<Dictionary<int, int>>(
        $"{_apiBaseUrl}/carts/{cartId}") ?? new();
}

    public async Task UpdateCartItemAsync(string cartId, int productId, int quantity)
{
    var content = JsonContent.Create(new { quantity });
    await _http.PutAsync($"{_apiBaseUrl}/carts/{cartId}/{productId}?quantity={quantity}", content);
}

    public async Task RemoveCartItemAsync(string cartId, int productId)
    {
        await _http.DeleteAsync($"{_apiBaseUrl}/carts/{cartId}/{productId}");
    }

    public async Task ClearCartAsync(string cartId)
    {
        await _http.DeleteAsync($"{_apiBaseUrl}/carts/{cartId}");
    }

    public async Task<int> ConfirmPurchaseAsync(string cartId, CustomerInfo customer)
{
    var response = await _http.PutAsJsonAsync($"{_apiBaseUrl}/carts/{cartId}/confirm", customer);
    var id = await response.Content.ReadAsStringAsync();
    return int.TryParse(id, out var parsed) ? parsed : -1;
}
}