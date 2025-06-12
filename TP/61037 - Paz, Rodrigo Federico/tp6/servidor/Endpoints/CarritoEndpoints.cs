using servidor.Models;
using servidor.Data;
using Microsoft.EntityFrameworkCore;
using servidor.Endpoints.ModelosResponse;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

public static class CarritoEndpoints
{
  public static void MapCarritoEndpoints(this WebApplication app)
  {

    app.MapPost("/carritos", async (TiendaContext db) =>
    {
      var carrito = new Carrito();
      db.Carritos.Add(carrito);
      await db.SaveChangesAsync();

      return Results.Ok(new { carrito.Id });
    });

    app.MapGet("/carritos/{carritoId}", async (Guid carritoId, TiendaContext db) =>
{
    var carrito = await db.Carritos
        .Include(c => c.Items)
        .ThenInclude(i => i.Producto)
        .FirstOrDefaultAsync(c => c.Id == carritoId);

    if (carrito == null)
        return Results.NotFound("Carrito no encontrado.");

    var response = carrito.Items.Select(i => new ItemCarritoResponse
    {
        ProductoId = i.ProductoId,
        Nombre = i.Producto.Nombre,
        Precio = i.Producto.Precio,
        Cantidad = i.Cantidad
    }).ToList();

    return Results.Ok(response);
   });

  }
}