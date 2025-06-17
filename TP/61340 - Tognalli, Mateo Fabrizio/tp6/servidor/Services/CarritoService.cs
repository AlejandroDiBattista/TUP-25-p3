using System.Collections.Concurrent;
using Servidor.DTOs;
using Servidor.Models;
using Servidor.Data;
using Microsoft.EntityFrameworkCore;

namespace Servidor.Services;

public class CarritoService
{
    private readonly ConcurrentDictionary<string, List<ItemCarritoDto>> _carritos = new();
    private readonly IServiceProvider _serviceProvider;

    public CarritoService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }    public string CrearCarrito()
    {
        var carritoId = Guid.NewGuid().ToString();
        _carritos.TryAdd(carritoId, new List<ItemCarritoDto>());
        return carritoId;
    }    public CarritoDto ObtenerCarrito(string carritoId)
    {
        if (!_carritos.TryGetValue(carritoId, out var items))
            return null;

        return new CarritoDto
        {
            Id = carritoId,
            Items = items
        };
    }public async Task<bool> AgregarProducto(string carritoId, int productoId, int cantidad = 1)
    {
        // Verificar si existe el carrito
        if (!_carritos.TryGetValue(carritoId, out var items))
        {
            return false;
        }

        // Obtener el contexto de base de datos usando el service provider
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TiendaDbContext>();
        
        // Obtener producto de la base de datos
        var producto = await context.Productos.FindAsync(productoId);
        if (producto == null)
        {
            return false;
        }

        // Verificar stock disponible
        var cantidadEnCarrito = items.Where(i => i.ProductoId == productoId).Sum(i => i.Cantidad);
        var cantidadTotal = cantidadEnCarrito + cantidad;
        
        if (cantidadTotal > producto.Stock)
        {
            return false;
        }        // Buscar si el producto ya está en el carrito
        var itemExistente = items.FirstOrDefault(i => i.ProductoId == productoId);
        
        if (itemExistente != null)
        {
            itemExistente.Cantidad += cantidad;
        }
        else
        {
            items.Add(new ItemCarritoDto
            {
                ProductoId = producto.Id,
                NombreProducto = producto.Nombre,
                ImagenUrl = producto.ImagenUrl,
                Cantidad = cantidad,
                PrecioUnitario = producto.Precio
            });
        }

        return true;
    }public bool EliminarProducto(string carritoId, int productoId, int cantidad = 1)
    {
        if (!_carritos.TryGetValue(carritoId, out var items))
            return false;

        var item = items.FirstOrDefault(i => i.ProductoId == productoId);
        if (item == null)
            return false;

        item.Cantidad -= cantidad;
        
        if (item.Cantidad <= 0)
        {
            items.Remove(item);
        }

        return true;
    }

    public bool VaciarCarrito(string carritoId)
    {
        if (!_carritos.TryGetValue(carritoId, out var items))
            return false;

        items.Clear();
        return true;
    }    public async Task<bool> ConfirmarCompra(string carritoId, ConfirmarCompraDto datosCliente)
    {
        if (!_carritos.TryGetValue(carritoId, out var items) || !items.Any())
            return false;

        // Obtener el contexto de base de datos usando el service provider
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TiendaDbContext>();
        
        using var transaction = await context.Database.BeginTransactionAsync();
        
        try
        {
            // Verificar stock de todos los productos
            foreach (var item in items)
            {
                var producto = await context.Productos.FindAsync(item.ProductoId);
                if (producto == null || producto.Stock < item.Cantidad)
                {
                    await transaction.RollbackAsync();
                    return false;
                }
            }

            // Crear la compra
            var compra = new Compra
            {
                Fecha = DateTime.Now,
                Total = items.Sum(i => i.Importe),
                NombreCliente = datosCliente.NombreCliente,
                ApellidoCliente = datosCliente.ApellidoCliente,
                EmailCliente = datosCliente.EmailCliente
            };

            context.Compras.Add(compra);
            await context.SaveChangesAsync();

            // Crear los items de compra y actualizar stock
            foreach (var item in items)
            {
                var producto = await context.Productos.FindAsync(item.ProductoId);
                if (producto != null)
                {
                    // Actualizar stock
                    producto.Stock -= item.Cantidad;

                    // Crear item de compra
                    var itemCompra = new ItemCompra
                    {
                        ProductoId = item.ProductoId,
                        CompraId = compra.Id,
                        Cantidad = item.Cantidad,
                        PrecioUnitario = item.PrecioUnitario
                    };

                    context.ItemsCompra.Add(itemCompra);
                }
            }

            await context.SaveChangesAsync();
            await transaction.CommitAsync();

            // Limpiar el carrito
            VaciarCarrito(carritoId);

            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            return false;
        }
    }
}
