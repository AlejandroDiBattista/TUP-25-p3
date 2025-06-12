using servidor.Models;
using servidor.Data;
using Microsoft.EntityFrameworkCore;

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
  }
}