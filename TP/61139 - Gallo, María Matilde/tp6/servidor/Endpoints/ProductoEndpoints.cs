using Microsoft.EntityFrameworkCore;
using Servidor.Data;
using Servidor.Models;

namespace Servidor.Endpoints;

public static class ProductosEndpoints
{
    public static void MapProductos(this WebApplication app)
    {
        // GET 
        app.MapGet("/productos", async (TiendaContext db) =>
            await db.Productos.ToListAsync());

        //  mismo GET pero con /api/productos
        app.MapGet("/api/productos", async (TiendaContext db) =>
            await db.Productos.ToListAsync());

        // GET por id sin prefijo
        app.MapGet("/productos/{id}", async (int id, TiendaContext db) =>
            await db.Productos.FindAsync(id) is Producto p ? Results.Ok(p) : Results.NotFound());

        //  GET por id con /api/productos/{id}
        app.MapGet("/api/productos/{id}", async (int id, TiendaContext db) =>
            await db.Productos.FindAsync(id) is Producto p2 ? Results.Ok(p2) : Results.NotFound());

        // POST compras 
        app.MapPost("/compras", async (Compra compra, TiendaContext db) =>
        {
            compra.Fecha = DateTime.Now;
            db.Compras.Add(compra);
            await db.SaveChangesAsync();
            return Results.Ok(compra);
        });
    }
}

