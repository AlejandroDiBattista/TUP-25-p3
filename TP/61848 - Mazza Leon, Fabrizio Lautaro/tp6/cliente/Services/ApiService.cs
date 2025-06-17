using System.Net.Http.Json;
using cliente.Models;

namespace cliente.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;

    public ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<int> CrearCarritoAsync()
    {

        var response = await _httpClient.PostAsync("/carritos", null);
        if (response.IsSuccessStatusCode)
        {
            var carrito = await response.Content.ReadFromJsonAsync<CarritoRespuesta>();
            return carrito.Id;
        }
        return 0;
    }

    public class CarritoRespuesta
    {
        public int Id { get; set; }
    }

    public async Task<List<Producto>> ObtenerProductosAsync()
    {
        try
        {
            var productos = await _httpClient.GetFromJsonAsync<List<Producto>>("/productos");
            return productos ?? new List<Producto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener productos: {ex.Message}");
            return new List<Producto>();
        }
    }

    public async Task<DatosRespuesta> ObtenerDatosAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<DatosRespuesta>("/api/datos");
            return response ?? new DatosRespuesta { Mensaje = "No se recibieron datos del servidor", Fecha = DateTime.Now };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener datos: {ex.Message}");
            return new DatosRespuesta { Mensaje = $"Error: {ex.Message}", Fecha = DateTime.Now };
        }
    }

    public async Task<bool> ConfirmarCompraAsync(IEnumerable<ItemCarrito> items, CompraFormulario formulario)
    {
        try
        {
            var compra = new
            {
                CarritoId = formulario.CarritoId,
                Nombre = formulario.Nombre,
                Apellido = formulario.Apellido,
                Email = formulario.Email,
                Items = items.Select(i => new { ProductoId = i.Producto.Id, Cantidad = i.Cantidad }).ToList()
            };

            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(compra));

            var response = await _httpClient.PostAsJsonAsync("/api/compras", compra);
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Status: {response.StatusCode}, Content: {content}");

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al confirmar compra: {ex.Message}");
            return false;
        }
    }
        public async Task VaciarCarritoAsync(int carritoId)
        {
            await _httpClient.DeleteAsync($"/carritos/{carritoId}");
        }
}

public class DatosRespuesta {
    public string Mensaje { get; set; }
    public DateTime Fecha { get; set; }
}