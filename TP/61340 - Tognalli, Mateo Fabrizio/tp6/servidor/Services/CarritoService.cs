using System.Collections.Concurrent;
using Servidor.DTOs;
using Servidor.Models;
using Servidor.Data;
using Microsoft.EntityFrameworkCore;

namespace Servidor.Services;

public class ReservaStock
{
    public int ProductoId { get; set; }
    public int Cantidad { get; set; }
    public DateTime Expiracion { get; set; }
    public string CarritoId { get; set; }
}

public class CarritoService
{
    private readonly ConcurrentDictionary<string, List<ItemCarritoDto>> _carritos = new();
    private readonly ConcurrentDictionary<string, ReservaStock> _reservas = new();
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

        // Limpiar reservas expiradas
        LimpiarReservasExpiradas();

        // Obtener el contexto de base de datos usando el service provider
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TiendaDbContext>();
        
        // Obtener producto de la base de datos
        var producto = await context.Productos.FindAsync(productoId);
        if (producto == null)
        {
            return false;
        }

        // Calcular stock reservado por OTROS carritos (excluyendo el actual)
        var stockReservadoPorOtros = _reservas.Values
            .Where(r => r.ProductoId == productoId && r.Expiracion > DateTime.Now && r.CarritoId != carritoId)
            .Sum(r => r.Cantidad);
        
        // Stock disponible para este carrito
        var stockDisponible = producto.Stock - stockReservadoPorOtros;
        
        // Cantidad que ya tiene este carrito del producto
        var cantidadEnCarrito = items.Where(i => i.ProductoId == productoId).Sum(i => i.Cantidad);
        
        // Verificar si puede agregar la cantidad solicitada
        if (cantidadEnCarrito + cantidad > stockDisponible)
        {
            return false;
        }        // Crear o actualizar reserva temporal (1 minuto)
        var reservaKey = $"{carritoId}_{productoId}";
        var nuevaExpiracion = DateTime.Now.AddMinutes(1);
        var cantidadTotalReservada = cantidadEnCarrito + cantidad;
        
        _reservas.AddOrUpdate(reservaKey, 
            new ReservaStock 
            { 
                ProductoId = productoId, 
                Cantidad = cantidadTotalReservada, 
                Expiracion = nuevaExpiracion,
                CarritoId = carritoId
            },
            (key, existente) => 
            {
                existente.Cantidad = cantidadTotalReservada;
                existente.Expiracion = nuevaExpiracion;
                return existente;
            });

        // Buscar si el producto ya está en el carrito
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
            // Eliminar reserva completamente
            var reservaKey = $"{carritoId}_{productoId}";
            _reservas.TryRemove(reservaKey, out _);
        }
        else
        {
            // Actualizar reserva con la nueva cantidad
            var reservaKey = $"{carritoId}_{productoId}";
            if (_reservas.TryGetValue(reservaKey, out var reserva))            {
                reserva.Cantidad = item.Cantidad;
                reserva.Expiracion = DateTime.Now.AddMinutes(1); // Renovar expiración
            }
        }

        return true;
    }    public bool VaciarCarrito(string carritoId)
    {
        if (!_carritos.TryGetValue(carritoId, out var items))
            return false;

        // Liberar todas las reservas de este carrito
        var reservasCarrito = _reservas.Where(kvp => kvp.Value.CarritoId == carritoId).ToList();
        foreach (var reserva in reservasCarrito)
        {
            _reservas.TryRemove(reserva.Key, out _);
        }

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
            // Verificar stock de todos los productos (sin considerar reservas ya que este carrito las tiene)
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
                    // Actualizar stock definitivamente
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

            // Limpiar el carrito y liberar reservas
            VaciarCarrito(carritoId);

            return true;
        }
        catch
        {            await transaction.RollbackAsync();
            return false;
        }
    }    private void LimpiarReservasExpiradas()
    {
        var reservasExpiradas = _reservas
            .Where(kvp => kvp.Value.Expiracion <= DateTime.Now)
            .ToList();

        foreach (var reservaExpirada in reservasExpiradas)
        {
            _reservas.TryRemove(reservaExpirada.Key, out _);
        }
    }    public int ObtenerStockDisponible(int productoId, int stockReal)
    {
        // Limpiar reservas expiradas
        LimpiarReservasExpiradas();

        // Calcular stock reservado para este producto por TODOS los carritos
        var stockReservado = _reservas.Values
            .Where(r => r.ProductoId == productoId && r.Expiracion > DateTime.Now)
            .Sum(r => r.Cantidad);        return Math.Max(0, stockReal - stockReservado);
    }
}
