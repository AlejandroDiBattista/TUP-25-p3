using Xunit;
using Cliente.Services;
using Cliente.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text;
using System.Threading;
using System.Linq.Expressions;

namespace Cliente.Tests;

public class CarritoServiceTests
{
    private readonly Mock<ILogger<CarritoService>> _mockLogger;
    private readonly Mock<HttpMessageHandler> _mockHttpHandler;
    private readonly HttpClient _httpClient;
    private readonly CarritoService _carritoService;

    public CarritoServiceTests()
    {
        _mockLogger = new Mock<ILogger<CarritoService>>();
        _mockHttpHandler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockHttpHandler.Object)
        {
            BaseAddress = new Uri("http://localhost:5184")
        };
        _carritoService = new CarritoService(_httpClient, _mockLogger.Object);
    }

    [Fact]
    public async Task ObtenerCarritoIdAsync_DebeCrearNuevoCarrito_CuandoNoExiste()
    {

        var expectedCarritoId = 123;
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(expectedCarritoId.ToString(), Encoding.UTF8, "application/json")
        };        _mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                It.IsAny<HttpRequestMessage>(),
                It.IsAny<CancellationToken>())
            .ReturnsAsync(response);


        var resultado = await _carritoService.ObtenerCarritoIdAsync();


        Assert.Equal(expectedCarritoId, resultado);
    }

    [Theory]
    [InlineData(1, 5, true)]
    [InlineData(0, 5, false)]
    [InlineData(-1, 5, false)]
    public void ValidarStock_DebeRetornarResultadoEsperado(int cantidad, int stock, bool esperado)
    {

        var resultado = ValidationService.Producto.ValidarStock(cantidad, stock);


        Assert.Equal(esperado, resultado);
    }

    [Theory]
    [InlineData("juan@email.com", null)]
    [InlineData("", "El email es requerido")]
    [InlineData("email-invalido", "Formato de email inválido")]
    public void ValidarEmail_DebeRetornarMensajeEsperado(string email, string? mensajeEsperado)
    {

        var resultado = ValidationService.Cliente.ValidarEmail(email);


        Assert.Equal(mensajeEsperado, resultado);
    }

    [Fact]
    public async Task AgregarProductoAsync_DebeRetornarTrue_CuandoEsExitoso()
    {

        var productoId = 1;
        var cantidad = 2;
        

        var carritoResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("123", Encoding.UTF8, "application/json")
        };


        var addResponse = new HttpResponseMessage(HttpStatusCode.OK);        _mockHttpHandler
            .Protected()
            .SetupSequence<Task<HttpResponseMessage>>(
                "SendAsync",
                It.IsAny<HttpRequestMessage>(),
                It.IsAny<CancellationToken>())
            .ReturnsAsync(carritoResponse)
            .ReturnsAsync(addResponse);


        var resultado = await _carritoService.AgregarProductoAsync(productoId, cantidad);


        Assert.True(resultado);
    }
}

public class ValidationServiceTests
{
    [Theory]
    [InlineData("Juan", null)]
    [InlineData("", "El nombre es requerido")]
    [InlineData("J", "El nombre debe tener al menos 2 caracteres")]
    [InlineData("NombreMuyLargoQueSuperaElLimiteDeCaracteresPeeeeeeeermitidos", "El nombre no puede tener más de 50 caracteres")]
    public void ValidarNombre_DebeRetornarMensajeEsperado(string nombre, string? mensajeEsperado)
    {

        var resultado = ValidationService.Cliente.ValidarNombre(nombre);


        Assert.Equal(mensajeEsperado, resultado);
    }

    [Theory]
    [InlineData(100.50, null)]
    [InlineData(0, "El precio debe ser mayor a cero")]
    [InlineData(-10, "El precio debe ser mayor a cero")]
    [InlineData(1000000, "El precio es demasiado alto")]
    public void ValidarPrecio_DebeRetornarMensajeEsperado(decimal precio, string? mensajeEsperado)
    {

        var resultado = ValidationService.Producto.ValidarPrecio(precio);


        Assert.Equal(mensajeEsperado, resultado);
    }
}
