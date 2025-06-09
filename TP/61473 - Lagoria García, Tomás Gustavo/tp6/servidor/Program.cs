using System.Text.Json;                     
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.DependencyInjection;

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

app.Run();

//MODELOS DE DATOS
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
            new Producto { Id = 1, Nombre = "Celular Samsung A14", Descripcion = "Pantalla 6.6”, 128GB", Precio = 450, Stock = 20, ImagenUrl = "https://via.placeholder.com/200" },
            new Producto { Id = 2, Nombre = "Auriculares Bluetooth", Descripcion = "Cancelación de ruido", Precio = 60, Stock = 50, ImagenUrl = "https://via.placeholder.com/200" },
            new Producto { Id = 3, Nombre = "Smart TV 43” LG", Descripcion = "Full HD, WebOS", Precio = 310, Stock = 10, ImagenUrl = "https://via.placeholder.com/200" },
            new Producto { Id = 4, Nombre = "Gaseosa Cola 2L", Descripcion = "Pack de 6 unidades", Precio = 9, Stock = 100, ImagenUrl = "https://via.placeholder.com/200" },
            new Producto { Id = 5, Nombre = "Notebook Lenovo i5", Descripcion = "8GB RAM, 512GB SSD", Precio = 700, Stock = 15, ImagenUrl = "https://via.placeholder.com/200" },
            new Producto { Id = 6, Nombre = "Mouse Gamer RGB", Descripcion = "7 botones programables", Precio = 25, Stock = 40, ImagenUrl = "https://via.placeholder.com/200" },
            new Producto { Id = 7, Nombre = "Parlante Bluetooth", Descripcion = "5W, portátil", Precio = 20, Stock = 30, ImagenUrl = "https://via.placeholder.com/200" },
            new Producto { Id = 8, Nombre = "Powerbank 10.000mAh", Descripcion = "Carga rápida USB-C", Precio = 35, Stock = 35, ImagenUrl = "https://via.placeholder.com/200" },
            new Producto { Id = 9, Nombre = "Tablet 10”", Descripcion = "Android 13, 64GB", Precio = 220, Stock = 12, ImagenUrl = "https://via.placeholder.com/200" },
            new Producto { Id = 10, Nombre = "Teclado Inalámbrico", Descripcion = "Compacto, multimedia", Precio = 18, Stock = 25, ImagenUrl = "https://via.placeholder.com/200" }
        );
    }
}