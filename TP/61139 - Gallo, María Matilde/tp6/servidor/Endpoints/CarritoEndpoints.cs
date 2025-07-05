using Microsoft.EntityFrameworkCore;
using Servidor.Data;
using Servidor.Models;

namespace Servidor.Endpoints
{
    public static class CarritoEndpoints
    {
        public static void MapCarrito(this WebApplication app)
        {
            // Endpoint para crear una compra
            app.MapPost("/api/compra", async (Compra compra, TiendaContext db) =>
            {
                db.Compras.Add(compra);
                await db.SaveChangesAsync();
                return Results.Ok(compra.Id);
            });

            // Agregar un item al carrito
            app.MapPost("/api/carrito/items", async (ItemCarrito item, TiendaContext db) =>
            {
                db.ItemsCarrito.Add(item);
                await db.SaveChangesAsync();
                return Results.Created($"/api/carrito/items/{item.Id}", item);
            });

            // Obtener todos los items del carrito
            app.MapGet("/api/carrito/items", async (TiendaContext db) =>
                await db.ItemsCarrito.Include(i => i.Producto).ToListAsync());

            // Eliminar un item del carrito
            app.MapDelete("/api/carrito/items/{id}", async (int id, TiendaContext db) =>
            {
                var item = await db.ItemsCarrito.FindAsync(id);
                if (item == null) return Results.NotFound();

                db.ItemsCarrito.Remove(item);
                await db.SaveChangesAsync();

                return Results.NoContent();
            });
        }
    }
}
