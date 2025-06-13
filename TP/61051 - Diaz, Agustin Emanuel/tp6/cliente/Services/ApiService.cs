using System.Net.Http.Json;
using System.Text.Json;
using cliente;

namespace cliente.Services
{
  public class ApiService
  {
    private readonly HttpClient _httpClient;
    private Guid? _carritoId;

    public event Action? OnCarritoActualizado;

    public ApiService(HttpClient httpClient)
    {
      _httpClient = httpClient;
    }

    #region Productos

    public async Task<List<Producto>> ObtenerProductosAsync(string? query = null)
    {
      try
      {
        var url = "/productos";
        if (!string.IsNullOrWhiteSpace(query))
        {
          url += $"?query={Uri.EscapeDataString(query)}";
        }

        var productos = await _httpClient.GetFromJsonAsync<List<Producto>>(url);
        return productos ?? new List<Producto>();
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error al obtener productos: {ex.Message}");
        return new List<Producto>();
      }
    }

    #endregion

    #region Carrito

    public async Task<Guid> ObtenerOCrearCarritoIdAsync()
    {
      if (_carritoId == null)
      {
        try
        {
          var response = await _httpClient.PostAsync("/carrito", null);
          if (response.IsSuccessStatusCode)
          {
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JsonElement>(content);
            _carritoId = Guid.Parse(result.GetProperty("carritoId").GetString()!);
          }
          else
          {
            _carritoId = Guid.NewGuid();
          }
        }
        catch
        {
          _carritoId = Guid.NewGuid();
        }
      }
      return _carritoId.Value;
    }

    public async Task<Carrito?> ObtenerCarritoAsync()
    {
      try
      {
        var carritoId = await ObtenerOCrearCarritoIdAsync();
        var carrito = await _httpClient.GetFromJsonAsync<Carrito>($"/carrito/{carritoId}");
        return carrito;
      }
      catch
      {
        return new Carrito { Id = await ObtenerOCrearCarritoIdAsync() };
      }
    }

    public async Task<Carrito?> ObtenerCarritoConProductosAsync()
    {
      try
      {
        var carrito = await ObtenerCarritoAsync();
        if (carrito == null || !carrito.Items.Any())
          return carrito;

        var productos = await ObtenerProductosAsync();

        foreach (var item in carrito.Items)
        {
          item.Producto = productos.FirstOrDefault(p => p.Id == item.ProductoId);
        }

        return carrito;
      }
      catch
      {
        return new Carrito { Id = await ObtenerOCrearCarritoIdAsync() };
      }
    }

    public async Task<bool> AgregarProductoAsync(int productoId, int cantidad = 1)
    {
      try
      {
        var carritoId = await ObtenerOCrearCarritoIdAsync();
        var response = await _httpClient.PutAsJsonAsync($"/carrito/{carritoId}/{productoId}", cantidad);

        if (response.IsSuccessStatusCode)
        {
          OnCarritoActualizado?.Invoke();
          return true;
        }
        return false;
      }
      catch
      {
        return false;
      }
    }

    public async Task<bool> EliminarProductoAsync(int productoId)
    {
      try
      {
        var carritoId = await ObtenerOCrearCarritoIdAsync();
        var response = await _httpClient.DeleteAsync($"/carrito/{carritoId}/{productoId}");

        if (response.IsSuccessStatusCode)
        {
          OnCarritoActualizado?.Invoke();
          return true;
        }
        return false;
      }
      catch
      {
        return false;
      }
    }

    public async Task<bool> VaciarCarritoAsync()
    {
      try
      {
        var carritoId = await ObtenerOCrearCarritoIdAsync();
        var response = await _httpClient.PostAsync($"/carrito/vaciar?id={carritoId}", null);

        if (response.IsSuccessStatusCode)
        {
          OnCarritoActualizado?.Invoke();
          return true;
        }
        return false;
      }
      catch
      {
        return false;
      }
    }

    public async Task<bool> EliminarCarritoAsync()
    {
      try
      {
        var carritoId = await ObtenerOCrearCarritoIdAsync();
        var response = await _httpClient.DeleteAsync($"/carrito?id={carritoId}");

        if (response.IsSuccessStatusCode)
        {
          _carritoId = null;
          OnCarritoActualizado?.Invoke();
          return true;
        }
        return false;
      }
      catch
      {
        return false;
      }
    }

    public async Task<int> ObtenerCantidadItemsAsync()
    {
      var carrito = await ObtenerCarritoAsync();
      return carrito?.TotalItems ?? 0;
    }

    #endregion

    #region Compras

    public async Task<bool> ConfirmarCompraAsync(Compra datosCompra)
    {
      try
      {
        var carritoId = await ObtenerOCrearCarritoIdAsync();
        var response = await _httpClient.PutAsJsonAsync($"/carrito/{carritoId}/confirmar", datosCompra);

        if (response.IsSuccessStatusCode)
        {
          _carritoId = null; 
          OnCarritoActualizado?.Invoke();
          return true;
        }
        return false;
      }
      catch
      {
        return false;
      }
    }

    public async Task<List<CompraResumen>> ObtenerComprasAsync()
    {
      try
      {
        var compras = await _httpClient.GetFromJsonAsync<List<CompraResumen>>("/compras");
        return compras ?? new List<CompraResumen>();
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error al obtener compras: {ex.Message}");
        return new List<CompraResumen>();
      }
    }

    public async Task<CompraDetalle?> ObtenerCompraAsync(int compraId)
    {
      try
      {
        var compra = await _httpClient.GetFromJsonAsync<CompraDetalle>($"/compras/{compraId}");
        return compra;
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error al obtener compra {compraId}: {ex.Message}");
        return null;
      }
    }

    #endregion

    #region Utilidades

    public async Task<bool> VerificarConexionAsync()
    {
      try
      {
        var response = await _httpClient.GetAsync("/");
        return response.IsSuccessStatusCode;
      }
      catch
      {
        return false;
      }
    }

    public async Task<ApiDatos?> ObtenerDatosApiAsync()
    {
      try
      {
        var datos = await _httpClient.GetFromJsonAsync<ApiDatos>("/api/datos");
        return datos;
      }
      catch
      {
        return null;
      }
    }

    #endregion
  }

  public class CompraResumen
  {
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public decimal Total { get; set; }
    public ClienteInfo Cliente { get; set; } = new();
    public List<ProductoCompra> Productos { get; set; } = new();
  }

  public class CompraDetalle
  {
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public decimal Total { get; set; }
    public ClienteInfo Cliente { get; set; } = new();
    public List<ProductoCompra> Productos { get; set; } = new();
  }

  public class ClienteInfo
  {
    public string NombreCliente { get; set; } = string.Empty;
    public string ApellidoCliente { get; set; } = string.Empty;
    public string EmailCliente { get; set; } = string.Empty;
  }

  public class ProductoCompra
  {
    public string Producto { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal { get; set; }
  }

  public class ApiDatos
  {
    public string Mensaje { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
  }
    
  public class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public string ImagenUrl { get; set; } = string.Empty;
}

public class Carrito
{
    public Guid Id { get; set; }
    public List<CarritoItem> Items { get; set; } = new();
    public decimal Total => Items.Sum(i => i.Subtotal);
    public int TotalItems => Items.Sum(i => i.Cantidad);
}

public class CarritoItem
{
    public int ProductoId { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal => Cantidad * PrecioUnitario;
    public Producto? Producto { get; set; }
}

public class Compra
{
    public string NombreCliente { get; set; } = string.Empty;
    public string ApellidoCliente { get; set; } = string.Empty;
    public string EmailCliente { get; set; } = string.Empty;
}
}