using Microsoft.EntityFrameworkCore;
using servidor.Models;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios CORS para permitir solicitudes desde el cliente
builder.Services.AddCors(options => {
    options.AddPolicy("AllowClientApp", policy => {
        policy.WithOrigins("http://localhost:5177", "https://localhost:7221")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Agregar controladores si es necesario
builder.Services.AddControllers();


builder.Services.AddDbContext<ModelsTiendaContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("TiendaContext")));

var app = builder.Build();

// Configurar el pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage();
}

// Usar CORS con la política definida
app.UseCors("AllowClientApp");

// Mapear rutas básicas
app.MapGet("/", () => "Servidor API está en funcionamiento");

// Ejemplo de endpoint de API
app.MapGet("/api/datos", () => new { Mensaje = "Datos desde el servidor", Fecha = DateTime.Now });

// Obtener todos los productos
app.MapGet("/api/productos", async (ModelsTiendaContext db) =>
    await db.Productos.ToListAsync());

// Obtener un producto por ID
app.MapGet("/api/productos/{id}", async (int id, ModelsTiendaContext db) =>
    await db.Productos.FindAsync(id) is Producto producto
        ? Results.Ok(producto)
        : Results.NotFound());

// Crear un nuevo producto
app.MapPost("/api/productos", async (Producto prod, ModelsTiendaContext db) =>
{
    db.Productos.Add(prod);
    await db.SaveChangesAsync();
    return Results.Created($"/api/productos/{prod.Id}", prod);
});

// Actualizar un producto existente
app.MapPut("/api/productos/{id}", async (int id, Producto prodActualizado, ModelsTiendaContext db) =>
{
    var prodExistente = await db.Productos.FindAsync(id);
    if (prodExistente is null)
        return Results.NotFound();

    prodExistente.Nombre = prodActualizado.Nombre;
    prodExistente.Descripcion = prodActualizado.Descripcion;
    prodExistente.Precio = prodActualizado.Precio;
    prodExistente.Stock = prodActualizado.Stock;
    prodExistente.ImagenUrl = prodActualizado.ImagenUrl;

    await db.SaveChangesAsync();
    return Results.Ok(prodExistente);
});

// Eliminar un producto
app.MapDelete("/api/productos/{id}", async (int id, ModelsTiendaContext db) =>
{
    var prod = await db.Productos.FindAsync(id);
    if (prod is null)
        return Results.NotFound();

    db.Productos.Remove(prod);
    await db.SaveChangesAsync();
    return Results.NoContent();
});
// Obtener todas las compras
app.MapGet("/api/compras", async (ModelsTiendaContext db) =>
    await db.Compras.ToListAsync());

// Obtener una compra por ID
app.MapGet("/api/compras/{id}", async (int id, ModelsTiendaContext db) =>
{
    var compra = await db.Compras.FindAsync(id);
    return compra is not null ? Results.Ok(compra) : Results.NotFound();
});

// Crear una nueva compra
app.MapPost("/api/compras", async (Compra compra, ModelsTiendaContext db) =>
{
    db.Compras.Add(compra);
    await db.SaveChangesAsync();
    return Results.Created($"/api/compras/{compra.Id}", compra);
});
app.MapPut("/api/compras/{id}", async (int id, Compra compraActualizada, ModelsTiendaContext db) =>
{
    var compra = await db.Compras.FindAsync(id);
    if (compra is null) return Results.NotFound();

    compra.Fecha = compraActualizada.Fecha;
    compra.Total = compraActualizada.Total;
    compra.ProductoId = compraActualizada.ProductoId;

    await db.SaveChangesAsync();
    return Results.Ok(compra);
});
app.MapDelete("/api/compras/{id}", async (int id, ModelsTiendaContext db) =>
{
    var compra = await db.Compras.FindAsync(id);
    if (compra is null) return Results.NotFound();

    db.Compras.Remove(compra);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();
