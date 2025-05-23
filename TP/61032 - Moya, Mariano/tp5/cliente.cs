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

async Task<List<Producto>> TraerAReponerAsync(){
    var json = await http.GetStringAsync($"{baseUrl}/productos/reponer");
    return JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt)!;
}

async Task<bool> AgregarStockAsync(int id, int cantidad){
    var resp = await http.PostAsync(
        $"{baseUrl}/productos/{id}/agregar-stock",
        new StringContent(JsonSerializer.Serialize(new { cantidad }), System.Text.Encoding.UTF8, "application/json")
    );
    return resp.IsSuccessStatusCode;
}

async Task<bool> QuitarStockAsync(int id, int cantidad){
    var resp = await http.PostAsync(
        $"{baseUrl}/productos/{id}/quitar-stock",
        new StringContent(JsonSerializer.Serialize(new { cantidad }), System.Text.Encoding.UTF8, "application/json")
    );
    return resp.IsSuccessStatusCode;
}

async Task MainMenu()
{
    while (true)
    {
        Console.WriteLine("\n=== Menú Tienda ===");
        Console.WriteLine("1. Listar productos");
        Console.WriteLine("2. Listar productos a reponer");
        Console.WriteLine("3. Agregar stock a un producto");
        Console.WriteLine("4. Quitar stock a un producto");
        Console.WriteLine("0. Salir");
        Console.Write("Opción: ");
        var op = Console.ReadLine();
        switch (op)
        {
            case "1":
                var productos = await TraerAsync();
                Console.WriteLine("ID  Nombre                Precio        Stock");
                foreach (var p in productos)
                    Console.WriteLine($"{p.Id,-3} {p.Nombre,-20} {p.Precio,10:c} {p.Stock,8}");
                break;
            case "2":
                var reponer = await TraerAReponerAsync();
                Console.WriteLine("ID  Nombre                Precio        Stock");
                foreach (var p in reponer)
                    Console.WriteLine($"{p.Id,-3} {p.Nombre,-20} {p.Precio,10:c} {p.Stock,8}");
                break;
            case "3":
                Console.Write("ID del producto: ");
                if (!int.TryParse(Console.ReadLine(), out int idAdd)) break;
                Console.Write("Cantidad a agregar: ");
                if (!int.TryParse(Console.ReadLine(), out int cantAdd)) break;
                if (await AgregarStockAsync(idAdd, cantAdd))
                    Console.WriteLine("Stock agregado correctamente.");
                else
                    Console.WriteLine("Error al agregar stock.");
                break;
            case "4":
                Console.Write("ID del producto: ");
                if (!int.TryParse(Console.ReadLine(), out int idQuitar)) break;
                Console.Write("Cantidad a quitar: ");
                if (!int.TryParse(Console.ReadLine(), out int cantQuitar)) break;
                if (await QuitarStockAsync(idQuitar, cantQuitar))
                    Console.WriteLine("Stock quitado correctamente.");
                else
                    Console.WriteLine("Error al quitar stock (puede que no haya suficiente).");
                break;
            case "0":
                return;
            default:
                Console.WriteLine("Opción inválida.");
                break;
        }
    }
}

await MainMenu();

class Producto {
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}

// Fin del ejemplo