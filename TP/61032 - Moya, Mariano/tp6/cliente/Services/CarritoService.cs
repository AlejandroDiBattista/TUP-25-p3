using System.Net.Http.Json;

namespace Cliente.Services;

public class CarritoService
{
    private readonly HttpClient _http;
    private const string ApiUrl = "http://localhost:5184/carritos";
    public int? CarritoId { get; private set; }

    public CarritoService(HttpClient http)
    {
        _http = http;
    }

    // Inicializa un carrito y guarda el Id
    public async Task<int> CrearCarritoAsync()
    {
        if (CarritoId != null) return CarritoId.Value;
        var resp = await _http.PostAsync(ApiUrl, null);
        var data = await resp.Content.ReadFromJsonAsync<CrearCarritoResponse>();
        CarritoId = data?.Id;
        return CarritoId ?? 0;
    }

    // Agrega un producto al carrito
    public async Task<bool> AgregarProductoAsync(int productoId, int cantidad)
    {
        var id = await CrearCarritoAsync();
        var url = $"{ApiUrl}/{id}/{productoId}?cantidad={cantidad}";
        var resp = await _http.PutAsync(url, null);
        return resp.IsSuccessStatusCode;
    }

    // Obtiene los ítems del carrito
    public async Task<List<ItemCarritoDto>> ObtenerItemsAsync()
    {
        if (CarritoId == null) return new List<ItemCarritoDto>();
        var url = $"{ApiUrl}/{CarritoId}";
        var carrito = await _http.GetFromJsonAsync<CarritoDto>(url);
        return carrito?.Items ?? new List<ItemCarritoDto>();
    }

    public class CrearCarritoResponse { public int Id { get; set; } }
    public class CarritoDto { public int Id { get; set; } public List<ItemCarritoDto> Items { get; set; } = new(); }
    public class ItemCarritoDto
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
        public string ImagenUrl { get; set; } = string.Empty;
    }
}
