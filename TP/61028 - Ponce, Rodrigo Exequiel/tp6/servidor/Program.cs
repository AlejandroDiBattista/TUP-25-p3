using Microsoft.EntityFrameworkCore;
using servidor.Modelos;

var builder = WebApplication.CreateBuilder(args);

// Conexión con SQLite
builder.Services.AddDbContext<TiendaContext>(options =>
    options.UseSqlite("Data Source=tienda.db"));

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClientApp", policy =>
    {
        policy.WithOrigins("http://localhost:5177")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseCors("AllowClientApp");

// ENDPOINTS PRODUCTOS

app.MapGet("/productos", async (TiendaContext db) =>
    await db.Productos.ToListAsync()
);

app.MapGet("/productos/{id}", async (int id, TiendaContext db) =>
    await db.Productos.FindAsync(id) is Producto producto
        ? Results.Ok(producto)
        : Results.NotFound()
);

app.MapPost("/productos", async (Producto producto, TiendaContext db) =>
{
    db.Productos.Add(producto);
    await db.SaveChangesAsync();
    return Results.Created($"/productos/{producto.Id}", producto);
});

app.MapPut("/productos/{id}", async (int id, Producto datosProducto, TiendaContext db) =>
{
    var producto = await db.Productos.FindAsync(id);
    if (producto is null) return Results.NotFound();

    producto.Nombre = datosProducto.Nombre;
    producto.Precio = datosProducto.Precio;
    producto.Stock = datosProducto.Stock;
    await db.SaveChangesAsync();
    return Results.Ok(producto);
});

app.MapDelete("/productos/{id}", async (int id, TiendaContext db) =>
{
    var producto = await db.Productos.FindAsync(id);
    if (producto is null) return Results.NotFound();

    db.Productos.Remove(producto);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// ENDPOINTS COMPRAS (CARRITO)

app.MapPost("/compras", async (Compra compra, TiendaContext db) =>
{
    foreach (var item in compra.Items)
    {
        var producto = await db.Productos.FindAsync(item.ProductoId);
        if (producto == null)
            return Results.BadRequest($"Producto con ID {item.ProductoId} no existe.");

        if (producto.Stock < item.Cantidad)
            return Results.BadRequest($"No hay suficiente stock para el producto {producto.Nombre}.");
    }

    decimal total = 0;
    foreach (var item in compra.Items)
    {
        var producto = await db.Productos.FindAsync(item.ProductoId);
        producto.Stock -= item.Cantidad;
        total += producto.Precio * item.Cantidad;
    }
    compra.Total = total;
    compra.Fecha = DateTime.Now;

    db.Compras.Add(compra);
    await db.SaveChangesAsync();
    return Results.Created($"/compras/{compra.Id}", compra);
});

app.MapGet("/compras", async (TiendaContext db) =>
    await db.Compras.Include(c => c.Items).ToListAsync()
);

app.MapGet("/compras/{id}", async (int id, TiendaContext db) =>
    await db.Compras.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == id) is Compra compra
        ? Results.Ok(compra)
        : Results.NotFound()
);

app.MapGet("/", () => "Servidor API está en funcionamiento");

app.Run();
