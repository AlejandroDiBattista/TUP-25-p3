
namespace cliente.Services;

public class ApiService
{
    private readonly HttpClient _http;

    public ApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<DatosRespuesta> ObtenerDatosAsync()
    {
        try
        {
            var response = await _http.GetFromJsonAsync<DatosRespuesta>("/api/datos");
            return response ?? new DatosRespuesta { Mensaje = "No se recibieron datos del servidor", Fecha = DateTime.Now };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener datos: {ex.Message}");
            return new DatosRespuesta { Mensaje = $"Error: {ex.Message}", Fecha = DateTime.Now };
        }
    }

    public async Task<List<Producto>> ObtenerProductosAsync()
    {
        return await _http.GetFromJsonAsync<List<Producto>>("/productos");
    }
}

public class DatosRespuesta
{
    public string Mensaje { get; set; }
    public DateTime Fecha { get; set; }
}

public class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public int Stock { get; set; }
    public int Precio { get; set; }
    public string ImagenUrl { get; set; }
}