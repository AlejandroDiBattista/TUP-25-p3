using Microsoft.EntityFrameworkCore;
using servidor;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options => {
    options.AddPolicy("AllowClientApp", policy => {
        policy.WithOrigins("http://localhost:5177", "https://localhost:7221")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<TiendaDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options => 
{
    options.SerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
});

var app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseCors("AllowClientApp");


app.MapGet("/api/productos", async (TiendaDbContext db, string? q) => { var productosQuery = db.Productos.AsQueryable(); if (!string.IsNullOrWhiteSpace(q)) { productosQuery = productosQuery.Where(p => p.Nombre.ToLower().Contains(q.ToLower()) || p.Descripcion.ToLower().Contains(q.ToLower())); } var productos = await productosQuery.ToListAsync(); return Results.Ok(productos); });
app.MapPost("/api/carritos", async (TiendaDbContext db) => { var nuevoCarrito = new Carrito(); db.Carritos.Add(nuevoCarrito); await db.SaveChangesAsync(); return Results.Created($"/api/carritos/{nuevoCarrito.Id}", new { carritoId = nuevoCarrito.Id }); });
app.MapPut("/api/carritos/{carritoId:guid}/productos/{productoId:int}", async (Guid carritoId, int productoId, TiendaDbContext db) => { var carrito = await db.Carritos.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == carritoId); if (carrito is null) { return Results.NotFound("El carrito no existe."); } var producto = await db.Productos.FindAsync(productoId); if (producto is null) { return Results.NotFound("El producto no existe."); } var itemEnCarrito = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId); if (itemEnCarrito is not null) { if (producto.Stock > itemEnCarrito.Cantidad) { itemEnCarrito.Cantidad++; } else { return Results.BadRequest("No hay suficiente stock para agregar otra unidad de este producto."); } } else { if (producto.Stock > 0) { carrito.Items.Add(new CarritoItem { ProductoId = producto.Id, Cantidad = 1 }); } else { return Results.BadRequest("Producto sin stock."); } } await db.SaveChangesAsync(); return Results.Ok(carrito); });
app.MapDelete("/api/carritos/{carritoId:guid}/productos/{productoId:int}", async (Guid carritoId, int productoId, TiendaDbContext db) => { var carrito = await db.Carritos.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == carritoId); if (carrito is null) { return Results.NotFound("El carrito no existe."); } var itemEnCarrito = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId); if (itemEnCarrito is null) { return Results.NotFound("El producto no se encuentra en el carrito."); } if (itemEnCarrito.Cantidad > 1) { itemEnCarrito.Cantidad--; } else { db.CarritoItems.Remove(itemEnCarrito); } await db.SaveChangesAsync(); return Results.Ok(carrito); });
app.MapGet("/api/carritos/{carritoId:guid}", async (Guid carritoId, TiendaDbContext db) => { var carrito = await db.Carritos.Include(c => c.Items).ThenInclude(i => i.Producto).FirstOrDefaultAsync(c => c.Id == carritoId); if (carrito is null) { return Results.NotFound("El carrito no existe."); } return Results.Ok(carrito); });
app.MapDelete("/api/carritos/{carritoId:guid}", async (Guid carritoId, TiendaDbContext db) => { var carrito = await db.Carritos.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == carritoId); if (carrito is null) { return Results.NotFound("El carrito no existe."); } if (carrito.Items.Any()) { db.CarritoItems.RemoveRange(carrito.Items); await db.SaveChangesAsync(); } return Results.Ok(carrito); });
app.MapPut("/api/carritos/{carritoId:guid}/confirmar", async (Guid carritoId, DatosClienteDto datosCliente, TiendaDbContext db) => { using var transaction = await db.Database.BeginTransactionAsync(); try { var carrito = await db.Carritos.Include(c => c.Items).ThenInclude(i => i.Producto).FirstOrDefaultAsync(c => c.Id == carritoId); if (carrito is null || !carrito.Items.Any()) { return Results.NotFound("El carrito no existe o está vacío."); } foreach (var item in carrito.Items) { if (item.Producto is null) { await transaction.RollbackAsync(); return Results.Problem($"No se pudo encontrar el producto con ID {item.ProductoId} en el carrito.", statusCode: 500); } if (item.Producto.Stock < item.Cantidad) { await transaction.RollbackAsync(); return Results.BadRequest($"No hay suficiente stock para {item.Producto.Nombre}. Stock disponible: {item.Producto.Stock}"); } } var compra = new Compra { NombreCliente = datosCliente.NombreCliente, ApellidoCliente = datosCliente.ApellidoCliente, EmailCliente = datosCliente.EmailCliente, Fecha = DateTime.Now, Total = carrito.Items.Sum(i => i.Cantidad * i.Producto.Precio) }; foreach (var item in carrito.Items) { var itemCompra = new ItemCompra { ProductoId = item.ProductoId, Cantidad = item.Cantidad, PrecioUnitario = item.Producto.Precio, Compra = compra }; db.ItemsCompra.Add(itemCompra); item.Producto.Stock -= item.Cantidad; } db.CarritoItems.RemoveRange(carrito.Items); db.Carritos.Remove(carrito); await db.SaveChangesAsync(); await transaction.CommitAsync(); return Results.Created($"/api/compras/{compra.Id}", compra); } catch (Exception ex) { await transaction.RollbackAsync(); return Results.Problem($"Error: {ex.Message}", statusCode: 500); } });


app.Run();