using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

var baseUrl = "http://localhost:5000";
var http = new HttpClient();
var jsonOpt = new JsonSerializerOptions {
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true
};

while (true) {
    Console.WriteLine("\n=== Menú ===");
    Console.WriteLine("1. Listar productos");
    Console.WriteLine("2. Listar productos con bajo stock");
    Console.WriteLine("3. Agregar stock a producto");
    Console.WriteLine("4. Quitar stock a producto");
    Console.WriteLine("0. Salir");
    Console.Write("Opción: ");
    var opcion = Console.ReadLine();

    switch (opcion) {
        case "1":
            var productos = await TraerProductosAsync();
            Console.WriteLine("\n=== Productos ===");
            foreach (var p in productos)
                Console.WriteLine($"{p.Id} {p.Nombre,-20} {p.Precio,10:C}  Stock: {p.Stock}");
            break;

        case "2":
            var bajos = await TraerBajoStockAsync();
            Console.WriteLine("\n=== Bajo Stock (<3) ===");
            foreach (var p in bajos)
                Console.WriteLine($"{p.Id} {p.Nombre,-20} Stock: {p.Stock}");
            break;

        case "3":
            Console.Write("ID del producto: ");
            int idAdd = int.Parse(Console.ReadLine()!);
            Console.Write("Cantidad a agregar: ");
            int cantAdd = int.Parse(Console.ReadLine()!);
            await CambiarStock("agregar-stock", idAdd, cantAdd);
            break;

        case "4":
            Console.Write("ID del producto: ");
            int idQuitar = int.Parse(Console.ReadLine()!);
            Console.Write("Cantidad a quitar: ");
            int cantQuitar = int.Parse(Console.ReadLine()!);
            await CambiarStock("quitar-stock", idQuitar, cantQuitar);
            break;

        case "0":
            return;
    }
}

async Task<List<Producto>> TraerProductosAsync() {
    var json = await http.GetStringAsync($"{baseUrl}/productos");
    return JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt)!;
}

async Task<List<Producto>> TraerBajoStockAsync() {
    var json = await http.GetStringAsync($"{baseUrl}/productos/bajo-stock");
    return JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt)!;
}

async Task CambiarStock(string accion, int id, int cantidad) {
    var obj = new { productoId = id, cantidad = cantidad };
    var json = JsonSerializer.Serialize(obj, jsonOpt);
    var resp = await http.PostAsync($"{baseUrl}/productos/{accion}",
        new StringContent(json, Encoding.UTF8, "application/json"));
    var msg = await resp.Content.ReadAsStringAsync();
    Console.WriteLine(resp.IsSuccessStatusCode ? "✔️ Éxito" : $"❌ Error: {msg}");
}

class Producto {
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}
