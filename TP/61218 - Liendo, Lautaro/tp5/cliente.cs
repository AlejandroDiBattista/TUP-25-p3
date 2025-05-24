using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

var baseUrl = "http://localhost:5000";
var http = new HttpClient();
var jsonOpt = new JsonSerializerOptions {
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true
};

async Task<List<Producto>> TraerTodosAsync() =>
    JsonSerializer.Deserialize<List<Producto>>(await http.GetStringAsync($"{baseUrl}/productos"), jsonOpt)!;

async Task<List<Producto>> TraerFaltantesAsync() =>
    JsonSerializer.Deserialize<List<Producto>>(await http.GetStringAsync($"{baseUrl}/productos/faltantes"), jsonOpt)!;

async Task ModificarStockAsync(int id, int cantidad, bool agregar) {
    var url = $"{baseUrl}/productos/{id}/" + (agregar ? "agregar" : "quitar") + $"?cantidad={cantidad}";
    var resp = await http.PostAsync(url, null);
    if (!resp.IsSuccessStatusCode)
        Console.WriteLine($"❌ Error: {(int)resp.StatusCode} - {await resp.Content.ReadAsStringAsync()}");
    else
        Console.WriteLine("✅ Operación realizada con éxito.");
}

while (true) {
    Console.WriteLine("""
    === Menú ===
    1. Listar productos
    2. Listar productos a reponer (stock < 3)
    3. Agregar stock
    4. Quitar stock
    0. Salir
    """);

    Console.Write("Opción: ");
    var op = Console.ReadLine();
    if (op == "0") break;

    switch (op) {
        case "1":
            var todos = await TraerTodosAsync();
            foreach (var p in todos)
                Console.WriteLine($"{p.Id} {p.Nombre,-20} {p.Precio,10:c} {p.Stock,5} u.");
            break;
        case "2":
            var faltantes = await TraerFaltantesAsync();
            foreach (var p in faltantes)
                Console.WriteLine($"{p.Id} {p.Nombre,-20} {p.Precio,10:c} {p.Stock,5} u.");
            break;
        case "3":
        case "4":
            Console.Write("ID del producto: ");
            if (!int.TryParse(Console.ReadLine(), out int id)) {
                Console.WriteLine("ID inválido.");
                break;
            }
            Console.Write("Cantidad: ");
            if (!int.TryParse(Console.ReadLine(), out int cant)) {
                Console.WriteLine("Cantidad inválida.");
                break;
            }
            await ModificarStockAsync(id, cant, op == "3");
            break;
        default:
            Console.WriteLine("Opción inválida.");
            break;
    }
}

class Producto {
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}
