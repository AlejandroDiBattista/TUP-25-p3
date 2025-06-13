using System.Net.Http.Json;
using System.Text.Json;
using System.Text;
using System.Net.Http;
using Microsoft.JSInterop;


namespace cliente.Services;
public class CarritoService
{
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _js;

    public CarritoService(HttpClient httpClient, IJSRuntime js)
    {
        _httpClient = httpClient;
        _js = js;
    }
    
    public async Task<List<ItemCarrito>> ObtenerCarritoAsync(Guid carritoId)
    {
        return await _httpClient.GetFromJsonAsync<List<ItemCarrito>>($"carritos/{carritoId}");
    }

    public async Task<Guid?> ObtenerCarritoIdAsync()
{
    var carritoIdStr = await _js.InvokeAsync<string>("localStorage.getItem", "carritoId");
        if (Guid.TryParse(carritoIdStr, out var carritoId))
        {
           return carritoId;
        }
     
    return null;
}

public async Task GuardarCarritoIdAsync(Guid carritoId)
{
    await _js.InvokeVoidAsync("localStorage.setItem", "carritoId", carritoId.ToString());
}

