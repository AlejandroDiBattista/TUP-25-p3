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

    public int Count => ListaProductos?.Count ?? 0;
    public CompraPendienteDto Compra;
    public List<ItemCompraGtDto> ListaProductos = new();

    public void AgregarProducto(ItemCompraGtDto producto)
    {
        ListaProductos.Add(producto);
    }

    public async Task ObtenerCompraPendiente()
    {
        try
        {
            Compra = await _httpClient.GetFromJsonAsync<CompraPendienteDto>("pendientes");
            if (Compra != null)
            {
                ListaProductos = await _httpClient.GetFromJsonAsync<List<ItemCompraGtDto>>($"carrito/{Compra.Id_compra}");
            }

        }
        catch (System.Exception ex)
        {
            Console.WriteLine($"Error al obtener datos: {ex.Message}");
        }
    }
    public async Task<DatosRespuesta<List<Producto>>> ObtenerProductos()
    {
        try
        {
            var res = await _httpClient.GetFromJsonAsync<List<Producto>>("productos");

            return new DatosRespuesta<List<Producto>> { Message = "exito", Response = res };
        }
        catch (System.Exception ex)
        {
            Console.WriteLine($"Error al obtener datos: {ex.Message}");
            return new DatosRespuesta<List<Producto>> { Message = ex.Message, Response = new List<Producto>() };
        }
    }

    public async Task IniciarCarrito()
    {
        try
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("carrito", new CompraDto());

            if (response.IsSuccessStatusCode)
            {
                Compra = await response.Content.ReadFromJsonAsync<CompraPendienteDto>();
            }

        }
        catch (System.Exception ex)
        {
            Console.WriteLine($"Error al obtener datos: {ex.Message}");

        }
    }
}

public class DatosRespuesta<T>
{
    public string Message { get; set; }
    public T Response { get; set; }
}
