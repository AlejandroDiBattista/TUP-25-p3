using servidor.Models;
using servidor.Data;
using servidor.Endpoints.ModelosRequest;
using Microsoft.EntityFrameworkCore;
using servidor.Endpoints.ModelosResponse;

public static class CompraEndpoints
{
  public static void MapCompraEndpoints(this WebApplication app)
  {
    app.MapPost("/compras", async (CompraRequest request, TiendaContext db) =>
    {

      if (request == null || request.Items == null || !request.Items.Any())
      {
        return Results.BadRequest("Debe incluir al menos un producto.");
      }

      decimal total = 0;
      var items = new List<ItemCompra>();

      foreach (var itemDto in request.Items)
      {
        var producto = await db.Productos.FindAsync(itemDto.ProductoId);
        Console.WriteLine("Request recibido:");
        if (producto == null)
          return Results.NotFound($"Producto con ID {itemDto.ProductoId} no existe.");

        if (producto.Stock < itemDto.Cantidad)
          return Results.BadRequest($"No hay stock suficiente para {producto.Nombre}.");

        producto.Stock -= itemDto.Cantidad;

        items.Add(new ItemCompra
        {
          ProductoId = producto.Id,
          Cantidad = itemDto.Cantidad,
          PrecioUnitario = producto.Precio
        });

        total += producto.Precio * itemDto.Cantidad;
      }

      var compra = new Compra
      {
        Fecha = DateTime.Now,
        NombreCliente = request.NombreCliente,
        ApellidoCliente = request.ApellidoCliente,
        EmailCliente = request.EmailCliente,
        Total = total,
        Items = items
      };

      db.Compras.Add(compra);
      await db.SaveChangesAsync();

      return Results.Created($"/compras/{compra.Id}", compra);

    });

    app.MapGet("/compras", async (TiendaContext db) =>
      {
        var compras = await db.Compras
              .Include(c => c.Items)
              .ThenInclude(i => i.Producto)
              .ToListAsync();

        var response = compras.Select(c => new CompraResponse
        {
          Id = c.Id,
          NombreCliente = c.NombreCliente,
          ApellidoCliente = c.ApellidoCliente,
          EmailCliente = c.EmailCliente,
          Fecha = c.Fecha,
          Total = c.Total,
          Items = c.Items.Select(i => new CompraResponse.ItemCompraResponse
          {
            NombreProducto = i.Producto.Nombre,
            Cantidad = i.Cantidad,
            PrecioUnitario = i.PrecioUnitario
          }).ToList()
        }).ToList();

        return Results.Ok(response);
      });
        
      app.MapGet("/compras/{id}", async (int id, TiendaContext db) =>
{
    var compra = await db.Compras
        .Include(c => c.Items)
        .ThenInclude(i => i.Producto)
        .FirstOrDefaultAsync(c => c.Id == id);

    if (compra == null)
        return Results.NotFound($"No se encontró la compra con ID {id}");

    var response = new CompraResponse
    {
        Id = compra.Id,
        NombreCliente = compra.NombreCliente,
        ApellidoCliente = compra.ApellidoCliente,
        EmailCliente = compra.EmailCliente,
        Fecha = compra.Fecha,
        Total = compra.Total,
        Items = compra.Items.Select(i => new CompraResponse.ItemCompraResponse
        {
            NombreProducto = i.Producto.Nombre,
            Cantidad = i.Cantidad,
            PrecioUnitario = i.PrecioUnitario
        }).ToList()
    };

    return Results.Ok(response);
});  

    }
}