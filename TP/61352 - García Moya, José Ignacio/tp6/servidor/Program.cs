using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);



// Agregar servicios CORS para permitir solicitudes desde el cliente
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClientApp", policy =>
    {
        policy.WithOrigins("http://localhost:5184")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Configuración de serialización camelCase
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

// Configuración de EF Core con SQLite
builder.Services.AddDbContext<TiendaContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Agregar controladores si es necesario
builder.Services.AddControllers();

var app = builder.Build();

// Configurar el pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage();
}

// Usar CORS con la política definida
app.UseCors("AllowClientApp");

// Mapear rutas básicas
app.MapGet("/", () => "Servidor API está en funcionamiento");

// Endpoint para obtener productos
app.MapGet("/productos", async ([FromServices] TiendaContext db, [FromQuery] string? q) =>
{
    var query = db.Productos.AsQueryable();
    if (!string.IsNullOrWhiteSpace(q))
        query = query.Where(p => p.Nombre.Contains(q));
    return await query.ToListAsync();
});

// Endpoint para confirmar compra
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

// DTOs para la confirmación de compra
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
