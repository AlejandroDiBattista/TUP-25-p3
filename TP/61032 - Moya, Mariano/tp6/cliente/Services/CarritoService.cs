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

    // Vacía el carrito
    public async Task<bool> VaciarCarritoAsync()
    {
        if (CarritoId == null) return false;
        var url = $"{ApiUrl}/{CarritoId}";
        var resp = await _http.DeleteAsync(url);
        return resp.IsSuccessStatusCode;
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

    // Elimina una unidad de un producto del carrito
    public async Task<bool> QuitarUnidadProductoAsync(int productoId)
    {
        if (CarritoId == null) return false;
        // Intentar restar 1 unidad. Primero obtener el item actual
        var items = await ObtenerItemsAsync();
        var item = items.FirstOrDefault(x => x.ProductoId == productoId);
        if (item == null) return false;
        if (item.Cantidad <= 1)
        {
            // Si solo queda una, eliminar el producto del carrito
            var url = $"{ApiUrl}/{CarritoId}/{productoId}";
            var resp = await _http.DeleteAsync(url);
            return resp.IsSuccessStatusCode;
        }
        else
        {
            // Si hay más de una, restar una unidad
            var url = $"{ApiUrl}/{CarritoId}/{productoId}?cantidad=-1";
            // Usamos el mismo endpoint de suma, pero con cantidad negativa
            var resp = await _http.PutAsync(url, null);
            return resp.IsSuccessStatusCode;
        }
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

    // DTO para datos del cliente
    public class DatosClienteDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    // Confirmar compra
    public async Task<bool> ConfirmarCompraAsync(DatosClienteDto datos)
    {
        if (CarritoId == null) return false;
        var url = $"{ApiUrl}/{CarritoId}/confirmar";
        var resp = await _http.PostAsJsonAsync(url, datos);
        return resp.IsSuccessStatusCode;
    }
}
