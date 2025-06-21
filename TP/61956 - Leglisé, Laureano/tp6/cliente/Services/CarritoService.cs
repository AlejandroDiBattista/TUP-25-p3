using System.Net.Http.Json;

public class CarritoService
{
    private readonly HttpClient _http;

    public CarritoService(HttpClient http)
    {
        _http = http;
    }

    public async Task<Guid> CrearCarrito()
    {
        var response = await _http.PostAsync("/carritos", null);
        return await response.Content.ReadFromJsonAsync<Guid>();
    }

    public async Task<List<ItemCarrito>> ObtenerItems(Guid carritoId)
    {
        try
        {
            return await _http.GetFromJsonAsync<List<ItemCarrito>>($"/carritos/{carritoId}");
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            // Si el carrito no existe, devuelve una lista vacía
            return new List<ItemCarrito>();
        }
    }

    public async Task<bool> AgregarOActualizarItem(Guid carritoId, int productoId, int cantidad)
    {
        var response = await _http.PutAsync($"/carritos/{carritoId}/{productoId}?cantidad={cantidad}", null);
        return response.IsSuccessStatusCode;
    }

    public async Task VaciarCarrito(Guid carritoId)
    {
        await _http.DeleteAsync($"/carritos/{carritoId}");
    }

    public async Task EliminarItem(Guid carritoId, int productoId)
    {
        await _http.DeleteAsync($"/carritos/{carritoId}/{productoId}");
    }

    public async Task<bool> ConfirmarCompra(Guid carritoId, object datosCliente)
    {
        var response = await _http.PutAsJsonAsync($"/carritos/{carritoId}/confirmar", datosCliente);
        return response.IsSuccessStatusCode;
    }

    public class ItemCarrito
    {
        public Producto Producto { get; set; }
        public int Cantidad { get; set; }
        public decimal Importe { get; set; }
    }
    public class Producto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public string ImagenUrl { get; set; }
    }
}