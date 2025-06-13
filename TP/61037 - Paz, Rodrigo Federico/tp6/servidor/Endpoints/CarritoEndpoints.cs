using servidor.Models;
using servidor.Data;
using Microsoft.EntityFrameworkCore;
using servidor.Endpoints.ModelosResponse;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using servidor.Endpoints.ModelosRequest;

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


    app.MapPut("/carritos/{carritoId}/{productoId}", async (Guid carritoId, int productoId, CantidadRequest datos, TiendaContext db) =>
  {
    var carrito = await db.Carritos
      .Include(c => c.Items)
      .FirstOrDefaultAsync(c => c.Id == carritoId);

    if (carrito is null)
      return Results.NotFound("Carrito no encontrado.");

    var producto = await db.Productos.FindAsync(productoId);
    if (producto is null)
      return Results.NotFound("Producto no encontrado.");

    var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);

    if (item != null)
    {
      item.Cantidad += datos.Cantidad;
    }
    else
    {
      carrito.Items.Add(new ItemCarrito
      {
        ProductoId = productoId,
        Cantidad = datos.Cantidad
      });
    }

    await db.SaveChangesAsync();

    return Results.Ok();
  });
  }
}