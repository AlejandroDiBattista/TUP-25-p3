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

async Task<List<Producto>> TraerProductosAsync() {
    var json = await http.GetStringAsync($"{baseUrl}/productos");
    return JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt)!;
}

async Task<List<Producto>> TraerBajoStockAsync() {
    var json = await http.GetStringAsync($"{baseUrl}/productos/bajo-stock");
    return JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt)!;
}

async Task AgregarStockAsync(int id) {
    var resp = await http.PostAsync($"{baseUrl}/productos/{id}/agregar", null);
    if (resp.IsSuccessStatusCode)
        Console.WriteLine("✔ Stock agregado.");
    else
        Console.WriteLine("❌ Error al agregar stock.");
}

async Task QuitarStockAsync(int id) {
    var resp = await http.PostAsync($"{baseUrl}/productos/{id}/quitar", null);
    if (resp.IsSuccessStatusCode)
        Console.WriteLine("✔ Stock quitado.");
    else {
        var error = await resp.Content.ReadAsStringAsync();
        Console.WriteLine($"❌ Error: {error}");
    }
}

int opcion;
do {
    Console.WriteLine("\n=== Menú ===");
    Console.WriteLine("1. Listar productos");
    Console.WriteLine("2. Listar productos con bajo stock");
    Console.WriteLine("3. Agregar stock a un producto");
    Console.WriteLine("4. Quitar stock a un producto");
    Console.WriteLine("0. Salir");
    Console.Write("Elegí una opción: ");
    opcion = int.Parse(Console.ReadLine()!);

    switch (opcion) {
        case 1:
            var productos = await TraerProductosAsync();
            foreach (var p in productos)
                Console.WriteLine($"{p.Id} - {p.Nombre} ({p.Stock})");
            break;
        case 2:
            var bajoStock = await TraerBajoStockAsync();
            foreach (var p in bajoStock)
                Console.WriteLine($"{p.Id} - {p.Nombre} ({p.Stock})");
            break;
        case 3:
            Console.Write("ID del producto a aumentar stock: ");
            int idAgregar = int.Parse(Console.ReadLine()!);
            await AgregarStockAsync(idAgregar);
            break;
        case 4:
            Console.Write("ID del producto a quitar stock: ");
            int idQuitar = int.Parse(Console.ReadLine()!);
            await QuitarStockAsync(idQuitar);
            break;
    }

} while (opcion != 0);

class Producto {
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public int Stock { get; set; }
}