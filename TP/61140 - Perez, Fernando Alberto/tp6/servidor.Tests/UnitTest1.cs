using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using servidor;

namespace servidor.Tests;

public class ProductosEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ProductosEndpointTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetProductos_ReturnsOkAndProductList()
    {
        // Act
        var response = await _client.GetAsync("/productos");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var productos = await response.Content.ReadFromJsonAsync<List<Producto>>();
        Assert.NotNull(productos);
        Assert.NotEmpty(productos);
    }

    private class Producto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public string ImagenUrl { get; set; } = string.Empty;
    }
}