using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

var baseUrl = "http://localhost:5000";
var http = new HttpClient();
var jsonOpt = new JsonSerializerOptions {
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true
};

async Task<List<Producto>> TraerTodosAsync() {
    var json = await http.GetStringAsync($"{baseUrl}/productos");
    return JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt)!;
}

async Task<List<Producto>> TraerReponerAsync() {
    var json = await http.GetStringAsync($"{baseUrl}/productos/reponer");
    return JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt)!;
}

async Task AgregarStock(int id, int cantidad) {
    var res = await http.PutAsync($"{baseUrl}/productos/{id}/agregar/{cantidad}", null);
    Console.WriteLine(res.IsSuccessStatusCode ? "✔ Stock agregado." : "❌ Error al agregar stock.");
}

async Task QuitarStock(int id, int cantidad) {
    var res = await http.PutAsync($"{baseUrl}/productos/{id}/quitar/{cantidad}", null);
    var txt = await res.Content.ReadAsStringAsync();
    Console.WriteLine(res.IsSuccessStatusCode ? "✔ Stock quitado." : $"❌ Error: {txt}");
}

int Opcion(string prompt, int min, int max) {
    int op;
    do {
        Console.Write(prompt);
    } while (!int.TryParse(Console.ReadLine(), out op) || op < min || op > max);
    return op;
}

while (true) {
    Console.WriteLine("\n=== Menú ===");
    Console.WriteLine("1. Listar productos");
    Console.WriteLine("2. Listar productos con stock bajo (<3)");
    Console.WriteLine("3. Agregar stock");
    Console.WriteLine("4. Quitar stock");
    Console.WriteLine("0. Salir");

    var op = Opcion("Elija una opción: ", 0, 4);
    if (op == 0) break;

    if (op == 1) {
        foreach (var p in await TraerTodosAsync())
            Console.WriteLine($"{p.Id} {p.Nombre,-20} {p.Stock,3} u. {p.Precio,8:c}");
    } else if (op == 2) {
        foreach (var p in await TraerReponerAsync())
            Console.WriteLine($"{p.Id} {p.Nombre,-20} {p.Stock,3} u. {p.Precio,8:c}");
    } else if (op == 3) {
        Console.Write("ID producto: "); int id = int.Parse(Console.ReadLine()!);
        Console.Write("Cantidad a agregar: "); int cant = int.Parse(Console.ReadLine()!);
        await AgregarStock(id, cant);
    } else if (op == 4) {
        Console.Write("ID producto: "); int id = int.Parse(Console.ReadLine()!);
        Console.Write("Cantidad a quitar: "); int cant = int.Parse(Console.ReadLine()!);
        await QuitarStock(id, cant);
    }
}

class Producto {
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}
