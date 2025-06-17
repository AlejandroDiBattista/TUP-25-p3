using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using servidor.Data;
using servidor.Models;

namespace servidor.Endpoints;

public static class ProductoEndpoints
{
  public static void MapProductoEndpoints(this WebApplication app)
    {
        
        app.MapGet("/productos", async (TiendaContext db, string? buscar) =>
        {
            var query = db.Productos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(buscar))
            {
                query = query.Where(p => p.Nombre.Contains(buscar));
            }

            var productos = await query.ToListAsync();
            return Results.Ok(productos);
        });
    }
}