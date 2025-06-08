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

while (true)
{
    Console.WriteLine(@"
1. Listar productos
2. Listar productos a reponer
3. Agregar stock a un producto
4. Quitar stock a un producto
0. Salir
");
    Console.Write("Opción: ");
    var op = Console.ReadLine();
    if (op == "0") break;

    switch (op)
    {
        case "1":
            var productos = await TraerAsync();
            Console.WriteLine("=== Productos ===");
            foreach (var p in productos)
                Console.WriteLine($"{p.Id} {p.Nombre,-20} {p.Precio,10:c} Stock: {p.Stock}");
            break;
        case "2":
            var reponer = await TraerReponerAsync();
            Console.WriteLine("=== Productos a reponer ===");
            foreach (var p in reponer)
                Console.WriteLine($"{p.Id} {p.Nombre,-20} {p.Precio,10:c} Stock: {p.Stock}");
            break;
        case "3":
            Console.Write("ID producto: ");
            int idA = int.Parse(Console.ReadLine()!);
            Console.Write("Cantidad a agregar: ");
            int cantA = int.Parse(Console.ReadLine()!);
            await AgregarStockAsync(idA, cantA);
            // No hace falta Console.WriteLine aquí, ya lo muestra el método
            break;
        case "4":
            Console.Write("ID producto: ");
            int idQ = int.Parse(Console.ReadLine()!);
            Console.Write("Cantidad a quitar: ");
            int cantQ = int.Parse(Console.ReadLine()!);
            await QuitarStockAsync(idQ, cantQ);
            // No hace falta Console.WriteLine aquí, ya lo muestra el método
            break;
    }
}

async Task<List<Producto>> TraerAsync(){
    var json = await http.GetStringAsync($"{baseUrl}/productos");
    return JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt)!;
}

async Task<List<Producto>> TraerReponerAsync(){
    var json = await http.GetStringAsync($"{baseUrl}/productos/reponer");
    return JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt)!;
}

async Task AgregarStockAsync(int id, int cantidad){
    var body = JsonSerializer.Serialize(new { cantidad });
    var content = new StringContent(body, System.Text.Encoding.UTF8, "application/json");
    var resp = await http.PostAsync($"{baseUrl}/productos/{id}/agregar", content);
    if (resp.IsSuccessStatusCode) {
        var json = await resp.Content.ReadAsStringAsync();
        var prod = JsonSerializer.Deserialize<Producto>(json, jsonOpt);
        Console.WriteLine($"Nuevo stock de {prod.Nombre}: {prod.Stock}");
    } else {
        Console.WriteLine("No se pudo agregar stock.");
    }
}

async Task<bool> QuitarStockAsync(int id, int cantidad){
    var body = JsonSerializer.Serialize(new { cantidad });
    var content = new StringContent(body, System.Text.Encoding.UTF8, "application/json");
    var resp = await http.PostAsync($"{baseUrl}/productos/{id}/quitar", content);
    if (resp.IsSuccessStatusCode) {
        var json = await resp.Content.ReadAsStringAsync();
        var prod = JsonSerializer.Deserialize<Producto>(json, jsonOpt);
        Console.WriteLine($"Nuevo stock de {prod.Nombre}: {prod.Stock}");
        return true;
    } else {
        Console.WriteLine("No se pudo quitar stock (stock insuficiente).");
        return false;
    }
}

class Producto {
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public decimal Precio { get; set; }
    public int Stock { get; set; } // NUEVO
}

// Fin del ejemplo