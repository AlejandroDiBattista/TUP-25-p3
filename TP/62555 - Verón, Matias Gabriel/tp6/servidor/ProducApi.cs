using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Servidor.Data;
using Servidor.Models;
using Microsoft.AspNetCore.Mvc;

namespace Servidor.Endpoints;

public static class ProductoApi
{
    public static async Task MapProductoEndpoints(this WebApplication app)
    {

        app.MapGet("/api/productos", async (TiendaContext db) =>
        {
            var productos = await db.Productos.ToListAsync();
            return Results.Ok(productos);
        });
       
        app.MapGet("/api/compras", async (TiendaContext db) =>
        {
            var compras = await db.Compras
                .Include(c => c.Detalles)
                .ThenInclude(d => d.Producto)
                .OrderByDescending(c => c.Fecha)
                .ToListAsync();

            return compras.Select(compra => new
            {
                compra.Id,
                compra.Fecha,
                compra.Total,
                compra.NombreCliente,
                compra.ApellidoCliente,
                compra.EmailCliente,
                Detalles = compra.Detalles.Select(d => new
                {
                    d.ProductoId,
                    NombreProducto = d.Producto != null ? d.Producto.Nombre : "",
                    d.Cantidad,
                    d.PrecioUnitario
                }).ToList()
            });
        });

        
            app.MapPut("/api/carritos/{carritoId}/confirmar", async (int carritoId, CompraDTO compraDto, TiendaContext db) =>
            {
                var carrito = await db.Carritos
                    .Include(c => c.Items)
                    .ThenInclude(i => i.Producto)
                    .FirstOrDefaultAsync(c => c.Id == carritoId);

                if (carrito == null || carrito.Items.Count == 0)
                    return Results.BadRequest("Carrito no encontrado o vacío.");

                
                foreach (var item in carrito.Items)
                {
                    if (item.Producto.Stock >= item.Cantidad)
                    {
                        item.Producto.Stock -= item.Cantidad;
                    }
                    else
                    {
                        return Results.BadRequest($"No hay suficiente stock para el producto {item.Producto.Nombre}.");
                    }
                }

                var compra = new Compra
                {
                    Fecha = DateTime.Now,
                    Total = carrito.Items.Sum(i => i.Cantidad * i.Producto.Precio),
                    NombreCliente = compraDto.NombreCliente,
                    ApellidoCliente = compraDto.ApellidoCliente,
                    EmailCliente = compraDto.EmailCliente,
                    Detalles = carrito.Items.Select(i => new DetalleCompra
                    {
                        ProductoId = i.ProductoId,
                        Cantidad = i.Cantidad,
                        PrecioUnitario = i.Producto.Precio
                    }).ToList()
                };

                db.Compras.Add(compra);
                db.Carritos.Remove(carrito);
                await db.SaveChangesAsync();

              
                return Results.Ok(new
                {
                    compra.Id,
                    compra.Fecha,
                    compra.Total,
                    compra.NombreCliente,
                    compra.ApellidoCliente,
                    compra.EmailCliente,
                    Detalles = compra.Detalles.Select(d => new
                    {
                        d.ProductoId,
                        d.Cantidad,
                        d.PrecioUnitario
                    }).ToList()
                });
            });
      
        app.MapPost("/api/carritos", async (TiendaContext db) =>
        {
            var carrito = new Carrito();
            db.Carritos.Add(carrito);
            await db.SaveChangesAsync();
            return Results.Ok(carrito); 
        });
       
     // Agregar producto al carrito sumandola la cantidad
        app.MapPut("/api/carritos/{carritoId:int}/{productoId:int}", async (
            int carritoId,
            int productoId,
            [FromBody] int cantidad,
            TiendaContext db) =>
        {
            if (cantidad < 1)
                return Results.BadRequest("Cantidad inválida.");

            var carrito = await db.Carritos.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == carritoId);
            if (carrito == null)
                return Results.NotFound("Carrito no encontrado.");

            var producto = await db.Productos.FindAsync(productoId);
            if (producto == null)
                return Results.NotFound("Producto no encontrado.");

            if (producto.Stock < cantidad)
                return Results.BadRequest("No hay suficiente stock.");

            var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
            if (item == null)
            {
                carrito.Items.Add(new ItemCarrito { ProductoId = productoId, Cantidad = cantidad });
            }
            else
            {
                item.Cantidad += cantidad;
            }

            await db.SaveChangesAsync();
            return Results.Ok();
        });
        // Endpoint para setear la cantidad absoluta de un producto en el carrito (sin modificar stock real)
        app.MapPut("/api/carritos/{carritoId:int}/{productoId:int}/cantidad", async (
            int carritoId,
            int productoId,
            [FromBody] int cantidad,
            TiendaContext db) =>
        {
            if (cantidad < 1)
                return Results.BadRequest("Cantidad inválida.");

            var carrito = await db.Carritos.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == carritoId);
            if (carrito == null)
                return Results.NotFound("Carrito no encontrado.");

            var producto = await db.Productos.FindAsync(productoId);
            if (producto == null)
                return Results.NotFound("Producto no encontrado.");

            var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
            if (item == null)
            {
                carrito.Items.Add(new ItemCarrito { ProductoId = productoId, Cantidad = cantidad });
            }
            else
            {
                item.Cantidad = cantidad;
            }

            await db.SaveChangesAsync();
            return Results.Ok();
        });
    

        app.MapGet("/api/carritos/{carritoId:int}", async (int carritoId, TiendaContext db) =>
        {
            var carrito = await db.Carritos
                .Include(c => c.Items)
                .ThenInclude(i => i.Producto)
                .FirstOrDefaultAsync(c => c.Id == carritoId);

            if (carrito == null)
                return Results.NotFound("Carrito no encontrado.");

            return Results.Ok(carrito);
        });

        app.MapDelete("/api/carritos/{carritoId}", async (int carritoId, TiendaContext db) =>
        {
            var carrito = await db.Carritos.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == carritoId);
            if (carrito == null)
                return Results.NotFound("Carrito no encontrado.");

            db.ItemsCarrito.RemoveRange(carrito.Items); 
            carrito.Items.Clear();
            await db.SaveChangesAsync();

            return Results.Ok();
        });
        
            app.MapDelete("/api/carritos/{carritoId}/{productoId}", async (int carritoId, int productoId, TiendaContext db) =>
            {
                var carrito = await db.Carritos.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == carritoId);
                if (carrito == null)
                    return Results.NotFound("Carrito no encontrado.");

                var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
                if (item == null)
                    return Results.NotFound("Producto no encontrado en el carrito.");

                db.ItemsCarrito.Remove(item);
                carrito.Items.Remove(item);

                await db.SaveChangesAsync();
                return Results.Ok();
            });
    }
    
}

