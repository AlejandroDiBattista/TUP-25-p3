using System.Net.Http.Json;
using System.Text.Json;
using System.Text;
using System.Net.Http;


namespace cliente.Services;

public class ComprobanteService
{
    private readonly HttpClient _httpClient;


    public ComprobanteService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Compra?> ObtenerComprobantesAsync(int compraoId)
    {
        var response = await _httpClient.GetFromJsonAsync<Compra>($"compras/{compraoId}");
        return response;
    }

    
    public class Compra
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public decimal Total { get; set; }
    public string NombreCliente { get; set; } = "";
    public string ApellidoCliente { get; set; } = "";
    public string EmailCliente { get; set; } = "";

    public List<ItemCompra> Items { get; set; } = new();
}

public class ItemCompra
{
    public int Id { get; set; }
    public int ProductoId { get; set; }
    public Producto? Producto { get; set; }
    public int CompraId { get; set; }
    public Compra? Compra { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
 public string ProductoNombre { get; set; }

}
}
