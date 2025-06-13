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
      private Guid _carritoId = Guid.Empty;

    public CarritoService(HttpClient httpClient, IJSRuntime js)
    {
        _httpClient = httpClient;
        _js = js;
    }
    

    public async Task<List<ItemCarrito>> ObtenerCarritoAsync(Guid carritoId)
    {
        return await _httpClient.GetFromJsonAsync<List<ItemCarrito>>($"carritos/{carritoId}");
        var response = await _httpClient.GetFromJsonAsync<CarritoResponse>($"carritos/{carritoId}");
    var items= response.Carrito;
    var total = response.Total;
        return items;
    }
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

    public async Task<Guid> AgregarOActualizarItemAsync(Guid? carritoId, int productoId, AgregarCarritoRequest? Nuevo)
    {
        var json = JsonSerializer.Serialize(Nuevo);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var carritoIdString = await _js.InvokeAsync<string>("localStorage.getItem", "carritoId");
        if (carritoId == null)
        {
            var response = await _httpClient.PostAsync($"carritos", content);
            response.EnsureSuccessStatusCode();
            var carrito = await response.Content.ReadAsStringAsync();
            var nuevoCarritoId = Guid.Parse(carrito.Replace("\"", ""));
            await GuardarCarritoIdAsync(nuevoCarritoId);
           return nuevoCarritoId;
        }
        else
        {
            var response = await _httpClient.PutAsync($"carritos/{carritoId}/{productoId}/aumentar", null);
            response.EnsureSuccessStatusCode();
             return carritoId.Value;
        }
        
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

    public async Task ConfirmarCompraAsync(Guid carritoId, ConfirmacionRequest confirmacion)
    {
        var json = JsonSerializer.Serialize(confirmacion);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync($"carritos/{carritoId}/confirmar", content);
        response.EnsureSuccessStatusCode();
    }
}

public class CarritoResponse
{
    public List<ItemCarrito> Carrito { get; set; }
    public decimal Total { get; set; }
}

public class AgregarCarritoRequest
{
    public int ProductoId { get; set; }
    public int Cantidad { get; set; }
};
public class ConfirmacionRequest
{
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class Carrito
{
    public Guid Id { get; set; }
    public List<ItemCarrito> Items { get; set; } = new List<ItemCarrito>();
}
public class ItemCarrito
{
    public int ProductoId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal PrecioUnitario { get; set; }
    public int Cantidad { get; set; }
}
