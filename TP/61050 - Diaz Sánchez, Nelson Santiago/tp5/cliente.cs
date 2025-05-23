using System;                     // Console, etc.
using System.Linq;                // OrderBy
using System.Net.Http;            // HttpClient, StringContent ✔
using System.Text.Json;           // JsonSerializer, JsonNamingPolicy
using System.Threading.Tasks;     // Task
using System.Globalization;


CultureInfo.CurrentCulture = new CultureInfo("es-AR");

var baseUrl = "http://localhost:5000";
var http = new HttpClient();
var jsonOpt = new JsonSerializerOptions {
    PropertyNamingPolicy        = JsonNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true
};



async Task<List<Producto>> TraerAsync()
{
    var json = await http.GetStringAsync($"{baseUrl}/productos");
    return JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt)!;
}

async Task<List<Producto>> TraerFaltantesAsync() {
    var json = await http.GetStringAsync($"{baseUrl}/productos/faltantes");
    return JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt)!;
}

async Task SumarStock(int id) {
    await http.PostAsync($"{baseUrl}/productos/{id}/sumar", null);
}

async Task RestarStock(int id) {
    var res = await http.PostAsync($"{baseUrl}/productos/{id}/restar", null);
    if (!res.IsSuccessStatusCode) {
        Console.WriteLine("❌ No se pudo quitar stock (quizás ya está en 0)");
    }
}

Console.WriteLine("=== Productos ===");
foreach (var p in await TraerAsync()) {
    Console.WriteLine($"{p.Id} {p.Nombre,-20} {p.Precio,15:c} {p.Stock,5}u");
}
class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}

while (true) {
    Console.WriteLine("\n1. Listar productos");
    Console.WriteLine("2. Listar productos faltantes (stock < 3)");
    Console.WriteLine("3. Sumar stock a un producto");
    Console.WriteLine("4. Restar stock a un producto");
    Console.WriteLine("0. Salir");

    Console.Write("Opción: ");
    Console.WriteLine("");
    var op = Console.ReadLine();

    if (op == "0") break;

    switch (op) {
        case "1":
            foreach (var p in await TraerAsync())
                Console.WriteLine($"{p.Id} {p.Nombre,-20} {p.Precio,10:c} {p.Stock,5}u");
            break;

        case "2":
            foreach (var p in await TraerFaltantesAsync())
                Console.WriteLine($"{p.Id} {p.Nombre,-20} {p.Stock,5}u");
            break;

        case "3":
            Console.Write("ID del producto: ");
            int idSumar = int.Parse(Console.ReadLine()!);
            await SumarStock(idSumar);
            Console.WriteLine("✅ Stock sumado");
            break;

        case "4":
            Console.Write("ID del producto: ");
            int idRestar = int.Parse(Console.ReadLine()!);
            await RestarStock(idRestar);
            Console.WriteLine("✅ Stock restado (si fue posible)");
            break;
    }
}