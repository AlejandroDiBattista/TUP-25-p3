using Microsoft.EntityFrameworkCore;
using servidor.Data;
using servidor.Models;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors(options => {
    options.AddPolicy("AllowClientApp", policy => {
        policy.WithOrigins("http://localhost:5177", "https://localhost:7221")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=tienda.db"));


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();


var app = builder.Build();


if (app.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage();
    
}

app.UseStaticFiles(); 
app.UseCors("AllowClientApp");
app.UseRouting();

app.UseAuthorization();


app.MapGet("/", () => "Servidor API está en funcionamiento");
app.MapGet("/api/datos", () => new { Mensaje = "Datos desde el servidor", Fecha = DateTime.Now });


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();

    if (!db.Productos.Any())
    {
        db.Productos.AddRange(
            new Producto { Nombre = "Auriculares Bluetooth", Descripcion = "Inalámbricos con cancelación de ruido", Imagen = "auriculares.jpg", Precio = 12000, Stock = 15 },
            new Producto { Nombre = "Mouse Gamer", Descripcion = "RGB y alta precisión", Imagen = "mouse.jpg", Precio = 8500, Stock = 25 },
            new Producto { Nombre = "Teclado Mecánico", Descripcion = "Retroiluminado y duradero", Imagen = "teclado.jpg", Precio = 18000, Stock = 10 }
        );
        db.SaveChanges();
    }
}




app.MapGet("/productos", async (AppDbContext db) =>
{
    return await db.Productos.ToListAsync();
});


app.MapGet("/compras", async (AppDbContext db) =>
{
    return await db.Compras
        .Include(c => c.Items)
        .ThenInclude(i => i.Producto)
        .ToListAsync();
});


app.MapPost("/compras", async (AppDbContext db, Compra compra) =>
{
    decimal total = 0;

    foreach (var item in compra.Items)
    {
        var producto = await db.Productos.FindAsync(item.ProductoId);

        if (producto == null || producto.Stock < item.Cantidad)
        {
            return Results.BadRequest("Producto no válido o sin stock.");
        }

        producto.Stock -= item.Cantidad;
        item.PrecioUnitario = producto.Precio;
        total += item.PrecioUnitario * item.Cantidad;
    }

    compra.Total = total;
    compra.Fecha = DateTime.Now;

    db.Compras.Add(compra);
    await db.SaveChangesAsync();

    return Results.Created($"/compras/{compra.Id}", compra);
});

app.Run();
