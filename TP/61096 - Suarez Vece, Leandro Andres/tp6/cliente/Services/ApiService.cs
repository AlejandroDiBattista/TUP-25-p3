using System.Net.Http.Json;
using System.Collections.Generic;
using System;
using cliente.Models;

namespace cliente.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;

    public ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public event Action<string>? OnBuscar;
    public void Buscar(string texto)
    {
        StringBusqueda = texto;
        OnBuscar?.Invoke(texto);
    }

    public string? StringBusqueda { get; set; }


    public event Action OnChange;
    public int Count => ListaProductos?.Count ?? 0;
    private List<ItemCompraGtDto> _listaProductos = new List<ItemCompraGtDto>();
    public List<ItemCompraGtDto> ListaProductos
    {
        get => _listaProductos;
        set
        {
            _listaProductos = value;
            NotifyStateChanged(); // Notificar el cambio
        }
    }


    private CompraPendienteDto _compra;
    public CompraPendienteDto Compra
    {
        get => _compra;
        set
        {
            _compra = value;
            NotifyStateChanged();
        }
    }

    private void NotifyStateChanged() => OnChange?.Invoke();

    public async Task ConfirmarCompra(ConfirmarCompraDto dto)
    {
        try
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"carrito/{Compra.Id_compra}/confirmar", dto);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Compra confirmada");
                EliminarTodoProductMemoria();
                OnChange?.Invoke();
            }

        }
        catch (System.Exception ex)
        {
            Console.WriteLine($"Error al obtener datos: {ex.Message}");
        }

    }

    public async Task VaciarCarrito()
    {
        try
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"carrito/{Compra.Id_compra}");
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Producto Eliminado");
                EliminarTodoProductMemoria();
                OnChange?.Invoke();
            }

        }
        catch (System.Exception ex)
        {
            Console.WriteLine($"Error al obtener datos: {ex.Message}");
        }
    }
    public async Task ElimarDelCarrito(int id)
    {
        try
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"carrito/{Compra.Id_compra}/{id}");
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Producto Eliminado");
                EliminarProductoMemoria(id);
                OnChange?.Invoke();
            }

        }
        catch (System.Exception ex)
        {
            Console.WriteLine($"Error al obtener datos: {ex.Message}");
        }
    }
    public async Task AgregarProductoAlCarrito(ItemCompraDto dto, string producto, int stock)
    {
        try
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"carrito/{Compra.Id_compra}", dto);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Producto agregado o modificado");
                AgregarProductoMemoria(dto, producto, stock);
                OnChange?.Invoke();
            }

        }
        catch (System.Exception ex)
        {
            Console.WriteLine($"Error al obtener datos: {ex.Message}");
        }
    }

    public async Task EliminarProductoMemoria(int id)
    {
        var productoExistente = ListaProductos.FirstOrDefault(p => p.ProductoId == id);
        _listaProductos.Remove(productoExistente);
        NotifyStateChanged();
    }
    public async Task EliminarTodoProductMemoria()
    {

        _listaProductos.Clear();
        NotifyStateChanged();
    }


    public void AgregarProductoMemoria(ItemCompraDto dto, string nombre, int stock)
    {
        var productoExistente = ListaProductos.FirstOrDefault(p => p.ProductoId == dto.ProductoId);

        if (productoExistente != null)
        {
            productoExistente.Cantidad += dto.Cantidad;
        }
        else
        {
            var producto = new ItemCompraGtDto
            {
                Cantidad = dto.Cantidad,
                ProductoId = dto.ProductoId,
                CompraId = Compra.Id_compra,
                NombreProducto = nombre,
                PrecioProducto = dto.PrecioUnitario,
                Stock = stock
            };

            ListaProductos.Add(producto);
        }

        NotifyStateChanged();
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


    public async Task<List<Producto>> ObtenerProductos()
    {

        try
        {
            var res = await _httpClient.GetFromJsonAsync<List<Producto>>($"productos/{StringBusqueda}");
            return res;
        }
        catch (System.Exception ex)
        {
            Console.WriteLine($"Error al obtener datos: {ex.Message}");
            return new List<Producto>();
        }
    }

    public async Task<List<CompraGetDto>> ObtenerHistorial(Page page)
    {

        try
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("historial", page);

            if (response.IsSuccessStatusCode)
            {
                var res = await response.Content.ReadFromJsonAsync<List<CompraGetDto>>();
                return res ?? new List<CompraGetDto>();
            }

        }
        catch (System.Exception ex)
        {
            Console.WriteLine($"Error al obtener datos del historial de compra: {ex.Message}");
        }
        return new List<CompraGetDto>();
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
