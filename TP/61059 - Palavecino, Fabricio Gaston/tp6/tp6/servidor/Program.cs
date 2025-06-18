using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClientApp", policy =>
    {
        policy.WithOrigins("http://localhost:5184")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});


builder.Services.AddDbContext<TiendaContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddControllers();

var app = builder.Build();


if (app.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage();
}


app.UseCors("AllowClientApp");
app.UseStaticFiles();


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TiendaContext>();
    db.Database.Migrate();
    if (!db.Productos.Any())
    {
        db.Productos.AddRange(new[]
        {
            new Producto { Id = 1, Nombre = "Celular A1", Descripcion = "Celular básico", Precio = 120000, Stock = 10, ImagenUrl = "wwwroot/imagenes/celular.jpg" },
            new Producto { Id = 2, Nombre = "Notebook B2", Descripcion = "Notebook liviana", Precio = 250000, Stock = 10, ImagenUrl = "wwwroot/imagenes/x515ea-ej1626wpi_iafbso0001_1.jpg" },
            new Producto { Id = 3, Nombre = "Auriculares C3", Descripcion = "Auriculares Bluetooth", Precio = 45000, Stock = 20, ImagenUrl = "wwwroot/imagenes/auriculares.jpg" },
            new Producto { Id = 4, Nombre = "Mouse Gamer", Descripcion = "Mouse con luces RGB", Precio = 5000, Stock = 26, ImagenUrl = "wwwroot/imagenes/mousegamer.jpg" },
            new Producto { Id = 5, Nombre = "Teclado Mecánico", Descripcion = "Teclado mecánico retroiluminado", Precio = 15000, Stock = 18, ImagenUrl = "wwwroot/imagenes/teecladomecanico.jpg"},
            new Producto { Id = 6, Nombre = "Monitor 24''", Descripcion = "Full HD 1080p", Precio = 80000, Stock = 2, ImagenUrl = "wwwroot/imagenes/monitor.jpg" },
            new Producto { Id = 7, Nombre = "Notebook Lenovo", Descripcion = "Intel i5, 8GB RAM", Precio = 350000, Stock = 9, ImagenUrl = "wwwroot/imagenes/notebooklenovo.jpg" },
            new Producto { Id = 8, Nombre = "Silla Gamer", Descripcion = "Ergonómica y cómoda", Precio = 60000, Stock = 7, ImagenUrl = "wwwroot/imagenes/sillagamer.jpg" },
            new Producto { Id = 9, Nombre = "iphone 16", Descripcion = "Android 12, 128GB", Precio = 180000, Stock = 10, ImagenUrl = "wwwroot/imagenes/iphone16.jpg" },
            new Producto { Id = 10, Nombre = "Tablet 10''", Descripcion = "Ideal para estudiar", Precio = 90000, Stock = 5, ImagenUrl = "wwwroot/imagenes/tablet.jpg" },
            new Producto { Id = 11, Nombre = "Webcam Full HD", Descripcion = "Con micrófono incluido", Precio = 10000, Stock = 30, ImagenUrl = "wwwroot/imagenes/webcam.jpg" },
            new Producto { Id = 12, Nombre = "Disco SSD 1TB", Descripcion = "Alta velocidad", Precio = 55000, Stock = 15, ImagenUrl = "wwwroot/imagenes/discossd1t.jpg" }
        });
        db.SaveChanges();
    }
}


app.MapGet("/", () => "Servidor API está en funcionamiento");


app.MapGet("/productos", async ([FromServices] TiendaContext db, [FromQuery] string q) =>
{
    var query = db.Productos.AsQueryable();
    if (!string.IsNullOrWhiteSpace(q))
        query = query.Where(p => p.Nombre.Contains(q));
    return await query.ToListAsync();
});


app.MapPut("/carritos/{carrito}/confirmar", async ([FromServices] TiendaContext db, string carrito, [FromBody] CompraConfirmacionDto dto) =>
{
    foreach (var item in dto.Items)
    {
        var prod = await db.Productos.FindAsync(item.ProductoId);
        if (prod == null || prod.Stock < item.Cantidad)
            return Results.BadRequest($"Stock insuficiente para {prod?.Nombre ?? "producto desconocido"}");
        prod.Stock -= item.Cantidad;
    }
    var compra = new Compra
    {
        Fecha = DateTime.Now,
        Total = dto.Items.Sum(i => i.PrecioUnitario * i.Cantidad),
        NombreCliente = dto.Nombre,
        ApellidoCliente = dto.Apellido,
        EmailCliente = dto.Email,
        Items = dto.Items.Select(i => new Item
        {
            ProductoId = i.ProductoId,
            Cantidad = i.Cantidad,
            PrecioUnitario = i.PrecioUnitario
        }).ToList()
    };
    db.Compras.Add(compra);
    await db.SaveChangesAsync();
    return Results.Ok();
});

app.Run();

public class CompraConfirmacionDto
{
    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public string Email { get; set; }
    public List<ItemCompraDto> Items { get; set; }
}

public class ItemCompraDto
{
    public int ProductoId { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
}