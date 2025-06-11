using Microsoft.AspNetCore.Mvc;
using.Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=tienda.db"));

builder.Services.AddCors(options => {
    options.AddPolicy("AllowClientApp", policy => {
        policy.WithOrigins("http://localhost:5177", "https://localhost:7221")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment()) {
  app.UseDeveloperExceptionPage();
}


app.UseCors("AllowClientApp");

app.MapGet("/", () => "Servidor API está en funcionamiento");

app.MapGet("/api/datos", () => new { Mensaje = "Datos desde el servidor", Fecha = DateTime.Now });

app.MapGet("/productos", async (AppDbContext db) =>
    await db.Productos.ToListAsync());

using (var scope = app.Services.CreateScope())
{
  var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
  db.Database.EnsureCreated();

  if (!db.Productos.Any())
  {
    db.Productos.AddRange(new[]
    {
            new Producto { Nombre = "Celular Samsung", Descripcion = "Galaxy A52", Precio = 600, Stock = 5, ImagenUrl = "samsung.jpg" },
            new Producto { Nombre = "Celular iPhone", Descripcion = "iPhone 12", Precio = 450, Stock = 3, ImagenUrl = "iphone.jpg" },
            new Producto { Nombre = "Notebook HP", Descripcion = "Intel i5 10° gen", Precio = 900, Stock = 4, ImagenUrl = "hp.jpg" },
            new Producto { Nombre = "Mouse Logitech", Descripcion = "Inalámbrico", Precio = 100, Stock = 10, ImagenUrl = "mouse.jpg" },
            new Producto { Nombre = "Auriculares JBL", Descripcion = "Bluetooth", Precio = 150, Stock = 8, ImagenUrl = "jbl.jpg" },
            new Producto { Nombre = "Monitor LG", Descripcion = "24 pulgadas Full HD", Precio = 160, Stock = 2, ImagenUrl = "monitor.jpg" },
            new Producto { Nombre = "Teclado Redragon", Descripcion = "Mecánico RGB", Precio = 100, Stock = 6, ImagenUrl = "teclado.jpg" },
            new Producto { Nombre = "Cargador portátil", Descripcion = "10000 mAh", Precio = 80, Stock = 9, ImagenUrl = "cargador.jpg" },
            new Producto { Nombre = "MacBook Pro M1", Descripcion = "Macbook M1 Pro 16 pulgadas", Precio = 1500, Stock = 6, ImagenUrl = "macbook.jpg" },
            new Producto { Nombre = "Silla gamer", Descripcion = "Silla gamer Redragon", Precio = 700, Stock = 15, ImagenUrl = "silla.jpg" }
        });

    db.SaveChanges();
  }
}

app.Run();