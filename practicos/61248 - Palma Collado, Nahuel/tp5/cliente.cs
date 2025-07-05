#nullable enable
using System;                     // Console, etc.
using System.Linq;                // OrderBy
using System.Net.Http;            // HttpClient, StringContent ✔
using System.Text.Json;           // JsonSerializer, JsonNamingPolicy
using System.Threading.Tasks;     // Task

var baseUrl = "http://localhost:5000";
var http = new HttpClient();
var jsonOpt = new JsonSerializerOptions {
    PropertyNamingPolicy        = JsonNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true
};

// Codigo de ejemplo: Reemplazar por la implementacion real 

async Task<List<Producto>> TraerAsync(){
    var json = await http.GetStringAsync($"{baseUrl}/productos");
    return JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt)!;
}

Console.WriteLine("=== Productos disponibles ===");
foreach (var p in await TraerAsync()) {
    Console.WriteLine($"{p.Id} {p.Nombre,-20} {p.Precio,15:c}");
}
class Producto {
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}

// Menú interactivo
while (true)
{
    Console.WriteLine("\n=== Menú Tienda ===");
    Console.WriteLine("1. Listar productos");
    Console.WriteLine("2. Listar productos a reponer");
    Console.WriteLine("3. Agregar stock a un producto");
    Console.WriteLine("4. Quitar stock a un producto");
    Console.WriteLine("0. Salir");
    Console.Write("Seleccione una opción: ");
    var op = Console.ReadLine();
    if (op == "0") break;
    switch (op)
    {
        case "1":
            var productos = await TraerAsync();
            Console.WriteLine("\n=== Productos ===");
            foreach (var p in productos)
                Console.WriteLine($"{p.Id} {p.Nombre,-20} {p.Precio,10:c} Stock: {p.Stock}");
            break;
        case "2":
            var reponer = await TraerReponerAsync();
            Console.WriteLine("\n=== Productos a reponer (stock < 3) ===");
            foreach (var p in reponer)
                Console.WriteLine($"{p.Id} {p.Nombre,-20} {p.Precio,10:c} Stock: {p.Stock}");
            break;
        case "3":
            Console.Write("Ingrese ID de producto para agregar stock: ");
            if (int.TryParse(Console.ReadLine(), out int idAdd))
            {
                var prod = await AgregarStockAsync(idAdd);
                if (prod != null)
                    Console.WriteLine($"Stock actualizado: {prod.Nombre} ahora tiene {prod.Stock}");
                else
                    Console.WriteLine("Producto no encontrado.");
            }
            break;
        case "4":
            Console.Write("Ingrese ID de producto para quitar stock: ");
            if (int.TryParse(Console.ReadLine(), out int idQuitar))
            {
                var prod = await QuitarStockAsync(idQuitar);
                if (prod != null)
                    Console.WriteLine($"Stock actualizado: {prod.Nombre} ahora tiene {prod.Stock}");
                else
                    Console.WriteLine("Producto no encontrado o stock insuficiente.");
            }
            break;
        default:
            Console.WriteLine("Opción inválida");
            break;
    }
}

// Métodos auxiliares
async Task<List<Producto>> TraerReponerAsync()
{
    var json = await http.GetStringAsync($"{baseUrl}/productos/reponer");
    return JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt)!;
}

async Task<Producto?> AgregarStockAsync(int id)
{
    var resp = await http.PostAsync($"{baseUrl}/productos/{id}/agregar", null);
    if (!resp.IsSuccessStatusCode) return null;
    var json = await resp.Content.ReadAsStringAsync();
    return JsonSerializer.Deserialize<Producto>(json, jsonOpt);
}

async Task<Producto?> QuitarStockAsync(int id)
{
    var resp = await http.PostAsync($"{baseUrl}/productos/{id}/quitar", null);
    if (!resp.IsSuccessStatusCode) return null;
    var json = await resp.Content.ReadAsStringAsync();
    return JsonSerializer.Deserialize<Producto>(json, jsonOpt);
}