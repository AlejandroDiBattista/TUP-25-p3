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

while (true) {
    Console.WriteLine("\n=== MENU ===");
    Console.WriteLine("1. Listar productos");
    Console.WriteLine("2. Listar productos a reponer");
    Console.WriteLine("3. Agregar stock a un producto");
    Console.WriteLine("4. Quitar stock a un producto");
    Console.WriteLine("0. Salir");
    Console.Write("Opción: ");
    var op = Console.ReadLine();
    if (op == "0") break;
    if (op == "1") {
        var productos = await TraerProductos();
        Mostrar(productos);
    } else if (op == "2") {
        var productos = await TraerProductosReponer();
        Mostrar(productos);
    } else if (op == "3") {
        Console.Write("ID producto: ");
        int.TryParse(Console.ReadLine(), out var id);
        Console.Write("Cantidad a agregar: ");
        int.TryParse(Console.ReadLine(), out var cant);
        await AgregarStock(id, cant);
    } else if (op == "4") {
        Console.Write("ID producto: ");
        int.TryParse(Console.ReadLine(), out var id);
        Console.Write("Cantidad a quitar: ");
        int.TryParse(Console.ReadLine(), out var cant);
        await QuitarStock(id, cant);
    }
}

async Task<List<Producto>> TraerProductos() {
    var json = await http.GetStringAsync($"{baseUrl}/productos");
    return JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt)!;
}

async Task<List<Producto>> TraerProductosReponer() {
    var json = await http.GetStringAsync($"{baseUrl}/productos/reponer");
    return JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt)!;
}

void Mostrar(List<Producto> productos) {
    Console.WriteLine($"{"ID",2} {"Nombre",-20} {"Precio",8} {"Stock",6}");
    foreach (var p in productos)
        Console.WriteLine($"{p.Id,2} {p.Nombre,-20} {p.Precio,8:c} {p.Stock,6}");
}

async Task AgregarStock(int id, int cantidad) {
    var resp = await http.PostAsync($"{baseUrl}/productos/{id}/agregar?cantidad={cantidad}", null);
    if (resp.IsSuccessStatusCode) Console.WriteLine("Stock agregado.");
    else Console.WriteLine("Error: " + await resp.Content.ReadAsStringAsync());
}

async Task QuitarStock(int id, int cantidad) {
    var resp = await http.PostAsync($"{baseUrl}/productos/{id}/quitar?cantidad={cantidad}", null);
    if (resp.IsSuccessStatusCode) Console.WriteLine("Stock quitado.");
    else Console.WriteLine("Error: " + await resp.Content.ReadAsStringAsync());
}

class Producto {
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}

// Fin del ejemplo