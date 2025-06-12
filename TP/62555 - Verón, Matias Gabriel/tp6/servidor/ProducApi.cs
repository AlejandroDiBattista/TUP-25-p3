using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Servidor.Data;
using Servidor.Models;

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
        app.MapPut("/api/carritos/{carritoId}/confirmar", async (int carritoId, CompraDTO compraDto, TiendaContext db) =>
        {
    
            var carrito = await db.Carritos.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == carritoId);
            if (carrito == null || carrito.Items.Count == 0)
                return Results.BadRequest("Carrito no encontrado o vacío.");

            
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
            return Results.Ok(compra.Id);
        });
    }
    
}

