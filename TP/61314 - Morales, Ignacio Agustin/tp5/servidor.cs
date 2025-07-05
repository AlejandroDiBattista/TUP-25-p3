#r "sdk:Microsoft.NET.Sdk.Web"
#r "nuget: Microsoft.EntityFrameworkCore, 9.0.4"
#r "nuget: Microsoft.EntityFrameworkCore.Sqlite, 9.0.4"

using System.Text.Json;                     
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.DependencyInjection;

//  CONFIGURACIÓN
var builder = WebApplication.CreateBuilder();
builder.Services.AddDbContext<AppDb>(opt => opt.UseSqlite("Data Source=./tienda.db")); // agregar servicios : Instalar EF Core y SQLite
builder.Services.Configure<JsonOptions>(opt => opt.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

var app = builder.Build();

app.MapGet("/productos", async (AppDb db) => await db.Productos.ToListAsync());

var db = app.Services.GetRequiredService<AppDb>();
db.Database.EnsureCreated(); // crear BD si no existe
// Agregar productos de ejemplo al crear la base de datos

app.Run("http://localhost:5000"); 
// NOTA: Si falla la primera vez, corralo nuevamente.



// Modelo de datos
class AppDb : DbContext {
    public AppDb(DbContextOptions<AppDb> options) : base(options) { }
    public DbSet<Producto> Productos => Set<Producto>();
}

while (true)
        {
            Console.WriteLine("\n--- MENÚ ---");
            Console.WriteLine("1. Listar productos");
            Console.WriteLine("2. Listar productos a reponer");
            Console.WriteLine("3. Agregar stock");
            Console.WriteLine("4. Quitar stock");
            Console.WriteLine("0. Salir");
            Console.Write("Elija una opción: ");
            string? opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1":
                    var productos = await client.GetFromJsonAsync<List<Producto_cliente>>("/productos");
                    foreach (var p in productos!) Console.WriteLine($"{p.Id} - {p.Nombre} (Stock: {p.Stock})");
                    break;

                case "2":
                    var reponer = await client.GetFromJsonAsync<List<Producto_cliente>>("/productos/reponer");
                    foreach (var p in reponer!) Console.WriteLine($"{p.Id} - {p.Nombre} (Stock: {p.Stock})");
                    break;

                case "3":
                    Console.Write("ID del producto: ");
                    int idAdd = int.Parse(Console.ReadLine()!);
                    Console.Write("Cantidad a agregar: ");
                    int cantAdd = int.Parse(Console.ReadLine()!);
                    await client.PostAsync($"/productos/agregar/{idAdd}/{cantAdd}", null);
                    break;

                case "4":
                    Console.Write("ID del producto: ");
                    int idRemove = int.Parse(Console.ReadLine()!);
                    Console.Write("Cantidad a quitar: ");
                    int cantRemove = int.Parse(Console.ReadLine()!);
                    var response = await client.PostAsync($"/productos/quitar/{idRemove}/{cantRemove}", null);
                    Console.WriteLine(response.IsSuccessStatusCode ? "Stock actualizado." : "Error: No se pudo quitar.");
                    break;

                case "0":
                    return;
            }
        }

class Producto{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public decimal Precio { get; set; }
}