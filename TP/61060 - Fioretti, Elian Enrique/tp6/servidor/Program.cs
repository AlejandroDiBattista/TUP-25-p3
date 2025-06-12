using servidor.Modulos;
using Microsoft.EntityFrameworkCore;

Dictionary<string, List<ItemCompra>> carritos = new();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options => {
    options.AddPolicy("AllowClientApp", policy => {
        policy.WithOrigins("http://localhost:5177", "https://localhost:7221")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddDbContext<TiendaContexto>(options =>
    options.UseSqlite("Data Source=tienda.baseDeDatos"));

var app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage();
}

app.UseCors("AllowClientApp");

app.UseStaticFiles();

app.MapGet("/", () => "Servidor API está en funcionamiento");

app.MapGet("/api/datos", () => new { Mensaje = "Datos desde el servidor", Fecha = DateTime.Now });

app.MapGet("/api/productos", async (TiendaContexto baseDeDatos, string? busqueda) =>
{
    var consulta = baseDeDatos.Productos.AsQueryable();
    if (!string.IsNullOrWhiteSpace(busqueda))
    {
        consulta = consulta.Where(p => p.Nombre.Contains(busqueda) || p.Descripcion.Contains(busqueda));
    }
    return await consulta.ToListAsync();
});

var baseUrl = "http://localhost:5184";

using (var scope = app.Services.CreateScope())
{
    var baseDeDatos = scope.ServiceProvider.GetRequiredService<TiendaContexto>();
    baseDeDatos.Database.EnsureCreated();
    if (!baseDeDatos.Productos.Any())
    {
        baseDeDatos.Productos.AddRange(new List<Producto>
        {
            new Producto { Nombre = "Brazo Biónico Avanzado", Descripcion = "Prótesis de brazo con fuerza aumentada y sensores táctiles.", Precio = 250000, Stock = 5, ImagenUrl = $"{baseUrl}/img/productos/brazo-bionico-avanzado.jpg" },
            new Producto { Nombre = "Pierna Biónica Deportiva", Descripcion = "Pierna biónica para alto rendimiento deportivo.", Precio = 300000, Stock = 3, ImagenUrl = $"{baseUrl}/img/productos/pierna-bionica-deportiva.jpg" },
            new Producto { Nombre = "Mano Biónica Multifunción", Descripcion = "Mano con dedos articulados y control por app.", Precio = 180000, Stock = 7, ImagenUrl = $"{baseUrl}/img/productos/mano-bionica-multifuncion.jpg" },
            new Producto { Nombre = "Ojo Biónico HD", Descripcion = "Implante ocular con visión nocturna y zoom digital.", Precio = 220000, Stock = 4, ImagenUrl = $"{baseUrl}/img/productos/ojo-bionico-hd.jpg" },
            new Producto { Nombre = "Exoesqueleto de Asistencia", Descripcion = "Exoesqueleto para soporte y movilidad asistida.", Precio = 500000, Stock = 2, ImagenUrl = $"{baseUrl}/img/productos/exoesqueleto-de-asistencia.jpg" },
            new Producto { Nombre = "Corazón Artificial Inteligente", Descripcion = "Corazón biónico con monitoreo remoto.", Precio = 800000, Stock = 1, ImagenUrl = $"{baseUrl}/img/productos/corazon-artificial-inteligente.jpg" },
            new Producto { Nombre = "Audífono Biónico 360", Descripcion = "Audífono con reducción de ruido y conexión Bluetooth.", Precio = 95000, Stock = 10, ImagenUrl = $"{baseUrl}/img/productos/audifono-bionico-360.jpg" },
            new Producto { Nombre = "Columna Vertebral Biónica", Descripcion = "Implante para soporte y corrección postural.", Precio = 400000, Stock = 2, ImagenUrl = $"{baseUrl}/img/productos/columna-vertebral-bionica.jpg" },
            new Producto { Nombre = "Piel Sintética Sensible", Descripcion = "Cubreprótesis con sensores de temperatura y presión.", Precio = 120000, Stock = 8, ImagenUrl = $"{baseUrl}/img/productos/piel-sintetica-sensible.jpg" },
            new Producto { Nombre = "Pulmón Artificial Compacto", Descripcion = "Dispositivo portátil para asistencia respiratoria.", Precio = 350000, Stock = 3, ImagenUrl = $"{baseUrl}/img/productos/pulmon-artificial-compacto.jpg" }
        });
        baseDeDatos.SaveChanges();
    }
}

app.Run();
