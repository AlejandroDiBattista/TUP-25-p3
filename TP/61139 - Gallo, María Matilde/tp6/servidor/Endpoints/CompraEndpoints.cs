using Microsoft.EntityFrameworkCore;
using Servidor.Data;
using Servidor.Models;

namespace Servidor.Endpoints;

public static class CompraEndpoints
{
    public static void MapCompras(this WebApplication app)
    {
        app.MapPost("/api/compras", async (Compra compra, TiendaContext db) =>
        {
            if (compra.Items == null || !compra.Items.Any())
                return Results.BadRequest("La compra no tiene ítems.");

            db.Compras.Add(compra);
            await db.SaveChangesAsync();

            return Results.Ok(compra);
        });

        app.MapGet("/api/compras", async (TiendaContext db) =>
            await db.Compras
                .Include(c => c.Items)
                .ThenInclude(i => i.Producto)
                .ToListAsync()
        );
    }
}
