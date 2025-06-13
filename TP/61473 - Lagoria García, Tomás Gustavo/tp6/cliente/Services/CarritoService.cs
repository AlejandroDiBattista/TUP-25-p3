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
    public async Task<Guid> CrearCrritoAsync()
    {
        var response = await _httpClient.PostAsync($"carritos",null);
        response.EnsureSuccessStatusCode();
        var carritoIdStr = await response.Content.ReadAsStringAsync();
        
        if (Guid.TryParse(carritoIdStr, out var carritoId))
        {
            _carritoId = carritoId;
            return carritoId;
        }
        else
        {
            throw new Exception("El backend devolvió un GUID inválido.");
        }
    }
    public async Task<Guid> ObtenerCarritoIdAsync()
    {   var response = await _httpClient.GetAsync("carritos/obtenerCarritoId");

    if (response.IsSuccessStatusCode)
    {
        var carritoIdBD = await response.Content.ReadFromJsonAsync<Guid>();
        return carritoIdBD ;
    }

    return await CrearCrritoAsync();
    }
/*
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

    public async Task VaciarCarritoAsync(Guid carritoId)
    {
        await _httpClient.DeleteAsync($"carritos/{carritoId}");
    }
    public async Task AumentarItemAsync(Guid? carritoId, int productoId)
    {
        var response = await _httpClient.PutAsync($"carritos/{carritoId}/{productoId}/aumentar", null);

        response.EnsureSuccessStatusCode();
    }

    public async Task DisminuirItemAsync(Guid? carritoId, int productoId)
{
    var response = await _httpClient.PutAsync($"carritos/{carritoId}/{productoId}/disminuir", null);
    response.EnsureSuccessStatusCode();
}

    public async Task EliminarItemAsync(Guid carritoId, int productoId)
    {
        await _httpClient.DeleteAsync($"carritos/{carritoId}/{productoId}");
    }

public class ItemCarrito
{
    public int ProductoId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal PrecioUnitario { get; set; }
    public int Cantidad { get; set; }
}
