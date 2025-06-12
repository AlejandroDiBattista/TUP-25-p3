using System.Text.Json;                     
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);


// Configuración EF Core con SQLite
builder.Services.AddDbContext<TiendaDbContext>(options =>
    options.UseSqlite("Data Source=tienda.db"));

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
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configurar el pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<TiendaDbContext>();
    context.Database.EnsureCreated(); // 🔸 Acá, no usa migraciones, pero crea la base si no existe.
}

// Usar CORS con la política definida
app.UseCors("AllowClientApp");

// Mapear rutas básicas
app.MapGet("/", () => "Servidor API está en funcionamiento");
app.MapGet("/productos", async (
    string? q,
    decimal? precioMin,
    decimal? precioMax,
    bool? enStock,
    string? ordenarPor,
    TiendaDbContext db) =>
{
    var query = db.Productos.AsQueryable();

    if (!string.IsNullOrWhiteSpace(q))
        query = query.Where(p => p.Nombre.ToLower().Contains(qLower) || p.Descripcion.ToLower().Contains(qLower));

    if (precioMin.HasValue)
        query = query.Where(p => p.Precio >= precioMin.Value);

    if (precioMax.HasValue)
        query = query.Where(p => p.Precio <= precioMax.Value);

    if (enStock == true)
        query = query.Where(p => p.Stock > 0);

    // Ordenamiento
    query = ordenarPor switch
    {
        "precio_asc" => query.OrderBy(p => p.Precio),
        "precio_desc" => query.OrderByDescending(p => p.Precio),
        "nombre" => query.OrderBy(p => p.Nombre),
        _ => query.OrderBy(p => p.Id) // default
    };

    var productos = await query.ToListAsync();
    return Results.Ok(productos);
});

// Diccionario temporal para carritos
var carritos = new Dictionary<Guid, List<ItemCarrito>>();

// POST /carritos → crea un nuevo carrito

app.MapPost("/carritos", async (AgregarCarritoRequest req, TiendaDbContext db) =>
{
    var producto = await db.Productos.FindAsync(req.ProductoId);
    if (producto == null)
        return Results.NotFound("Producto no encontrado");

    if (producto.Stock < req.Cantidad)
        return Results.BadRequest("Stock insuficiente");

    var id = Guid.NewGuid();

    carritos[id] = new List<ItemCarrito>()
    {
        new ItemCarrito
        {
            ProductoId = producto.Id,
            Nombre = producto.Nombre,
            PrecioUnitario = producto.Precio,
            Cantidad = req.Cantidad
        }
    };
    return Results.Ok(id);
});


// PUT /carritos/{carritoId}/{productoId}/aumentar → aumenta la cantidad de un producto en el carrito
app.MapPut("/carritos/{carritoId}/{productoId}/aumentar", async (Guid carritoId, int productoId, TiendaDbContext db) =>
{
    if (!carritos.ContainsKey(carritoId))
        return Results.NotFound("Carrito no encontrado");

    var producto = await db.Productos.FindAsync(productoId);
    if (producto == null)
        return Results.NotFound("Producto no encontrado");

    if (producto.Stock <= 0)
        return Results.BadRequest("Stock insuficiente");

    var carrito = carritos[carritoId];
    var item = carrito.FirstOrDefault(i => i.ProductoId == productoId);

    if (item == null)
    {
        carrito.Add(new ItemCarrito
        {
            ProductoId = producto.Id,
            Nombre = producto.Nombre,
            PrecioUnitario = producto.Precio,
            Cantidad = 1
        });
    }
    else
    {
        item.Cantidad++;
    }

    producto.Stock--;
    await db.SaveChangesAsync();

    return Results.Ok(carrito);
});

// GET /carritos/{carritoId}
app.MapGet("/carritos/{carritoId}", (Guid carritoId) =>
{
    if (!carritos.ContainsKey(carritoId))
        return Results.NotFound("Carrito no encontrado");

    return Results.Ok(carritos[carritoId]);
});
// DELETE /carritos/{carritoId}
app.MapDelete("/carritos/{carritoId}", async (Guid carritoId, TiendaDbContext db) =>
{
    if (!carritos.ContainsKey(carritoId))
        return Results.NotFound("Carrito no encontrado");

    var carrito = carritos[carritoId];
    foreach (var item in carrito)
    {
        var producto = await db.Productos.FindAsync(item.ProductoId);
        if (producto != null)
            producto.Stock += item.Cantidad;
    }

    await db.SaveChangesAsync();
    carritos.Remove(carritoId);

    return Results.NoContent();
});



// Ejemplo de endpoint de API
app.MapGet("/api/datos", () => new { Mensaje = "Datos desde el servidor", Fecha = DateTime.Now });

app.Run();

//MODELOS DE DATOS
record AgregarCarritoRequest(int ProductoId, int Cantidad);

record ItemCarrito
{
    public int ProductoId { get; set; }
    public string Nombre { get; set; } = "";
    public decimal PrecioUnitario { get; set; }
    public int Cantidad { get; set; }
}
public class Compra
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public decimal Total { get; set; }
    public string NombreCliente { get; set; } = "";
    public string ApellidoCliente { get; set; } = "";
    public string EmailCliente { get; set; } = "";

    public List<ItemCompra> Items { get; set; } = new();
}

public class ItemCompra
{
    public int Id { get; set; }
    public int ProductoId { get; set; }
    public Producto? Producto { get; set; }
    public int CompraId { get; set; }
    public Compra? Compra { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }


}

 public class Producto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public string ImagenURL { get; set; }
        
    }

public class TiendaDbContext : DbContext
{
    public TiendaDbContext(DbContextOptions<TiendaDbContext> options) : base(options) { }

    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Compra> Compras => Set<Compra>();
    public DbSet<ItemCompra> ItemsCompra => Set<ItemCompra>();
    
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Producto>().HasData(
            new Producto { Id = 1, Nombre = "Celular Samsung A14", Descripcion = "Pantalla 6.6”, 128GB", Precio = 450, Stock = 20, ImagenURL = "https://images.samsung.com/is/image/samsung/p6pim/ar/sm-a145mzsearo/gallery/ar-galaxy-a14-sm-a145-sm-a145mzsearo-535983519?$684_547_PNG$" }
            /*new Producto { Id = 2, Nombre = "Auriculares Bluetooth", Descripcion = "Cancelación de ruido", Precio = 60, Stock = 50, ImagenURL = "https://via.placeholder.com/200" },
            new Producto { Id = 3, Nombre = "Smart TV 43” LG", Descripcion = "Full HD, WebOS", Precio = 310, Stock = 10, ImagenURL = "https://via.placeholder.com/200" },
            new Producto { Id = 4, Nombre = "Gaseosa Cola 2L", Descripcion = "Pack de 6 unidades", Precio = 9, Stock = 100, ImagenURL = "https://via.placeholder.com/200" },
            new Producto { Id = 5, Nombre = "Notebook Lenovo i5", Descripcion = "8GB RAM, 512GB SSD", Precio = 700, Stock = 15, ImagenURL = "https://via.placeholder.com/200" },
            new Producto { Id = 6, Nombre = "Mouse Gamer RGB", Descripcion = "7 botones programables", Precio = 25, Stock = 40, ImagenURL = "https://via.placeholder.com/200" },
            new Producto { Id = 7, Nombre = "Parlante Bluetooth", Descripcion = "5W, portátil", Precio = 20, Stock = 30, ImagenURL = "https://via.placeholder.com/200" },
            new Producto { Id = 8, Nombre = "Powerbank 10.000mAh", Descripcion = "Carga rápida USB-C", Precio = 35, Stock = 35, ImagenURL = "https://via.placeholder.com/200" },
            new Producto { Id = 9, Nombre = "Tablet 10”", Descripcion = "Android 13, 64GB", Precio = 220, Stock = 12, ImagenURL = "https://via.placeholder.com/200" },
            new Producto { Id = 10, Nombre = "Teclado Inalámbrico", Descripcion = "Compacto, multimedia", Precio = 18, Stock = 25, ImagenURL = "https://via.placeholder.com/200" }
        */
        );
    }
}