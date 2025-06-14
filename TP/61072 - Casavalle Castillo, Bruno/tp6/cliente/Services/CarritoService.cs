using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using servidor.Models;

public class ItemCarrito
{
    public int ProductoId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public int Cantidad { get; set; }
    public string ImagenUrl { get; set; } = string.Empty;

    public decimal Subtotal => Precio * Cantidad;

    public ItemCarrito(Producto producto, int cantidad)
    {
        ProductoId = producto.Id;
        Nombre = producto.Nombre;
        Precio = producto.Precio;
        ImagenUrl = producto.ImagenUrl;
        Cantidad = cantidad;
    }

    public ItemCarrito() { }
}

public class CarritoService
{
    public event Action? OnChange;

    private readonly List<ItemCarrito> carrito = new();
    private readonly ProductoService productoService;

    public CarritoService(ProductoService productoService)
    {
        this.productoService = productoService;
    }

    public IReadOnlyList<ItemCarrito> ObtenerItems() => carrito;

    public void AgregarProducto(Producto producto, int cantidad)
    {
        var existente = carrito.FirstOrDefault(i => i.ProductoId == producto.Id);
        if (existente != null)
        {
            if (existente.Cantidad + cantidad <= producto.Stock)
            {
                existente.Cantidad += cantidad;
            }
            else
            {
                existente.Cantidad = producto.Stock;
            }
        }
        else
        {
            int cantidadAAgregar = Math.Min(cantidad, producto.Stock);
            carrito.Add(new ItemCarrito(producto, cantidadAAgregar));
        }

        NotificarCambio();
    }

    public void EliminarProducto(int productoId)
    {
        var item = carrito.FirstOrDefault(i => i.ProductoId == productoId);
        if (item != null)
        {
            carrito.Remove(item);
            NotificarCambio();
        }
    }

    public void VaciarCarrito()
    {
        carrito.Clear();
        NotificarCambio();
    }

    public decimal CalcularTotal() => carrito.Sum(i => i.Subtotal);

    public async Task IncrementarCantidad(int productoId)
    {
        var item = carrito.FirstOrDefault(i => i.ProductoId == productoId);
        if (item != null)
        {
            var producto = await ObtenerProductoPorId(productoId);
            if (producto != null && item.Cantidad < producto.Stock)
            {
                item.Cantidad++;
                NotificarCambio();
            }
        }
    }

    public void DisminuirCantidad(int productoId)
    {
        var item = carrito.FirstOrDefault(i => i.ProductoId == productoId);
        if (item != null)
        {
            item.Cantidad--;
            if (item.Cantidad <= 0)
            {
                carrito.Remove(item);
            }
            NotificarCambio();
        }
    }

    private void NotificarCambio() => OnChange?.Invoke();

    private async Task<Producto?> ObtenerProductoPorId(int id)
    {
        return await productoService.ObtenerPorId(id);
    }
}