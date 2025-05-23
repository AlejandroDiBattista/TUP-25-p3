using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

var baseUrl = "http://localhost:5000";
var http = new HttpClient();
var jsonOpt = new JsonSerializerOptions {
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true
};

while (true) {
    Console.WriteLine("\n=== Menú ===");
    Console.WriteLine("1. Listar productos");
    Console.WriteLine("2. Listar productos a reponer");
    Console.WriteLine("3. Agregar stock a un producto");
    Console.WriteLine("4. Quitar stock a un producto");
    Console.WriteLine("0. Salir");
    Console.Write("Opción: ");
    var opcion = Console.ReadLine();

    switch (opcion) {
        case "1":
            var productos = await TraerAsync();
            Console.WriteLine("\n=== Productos ===");
            foreach (var p in productos)
                Console.WriteLine($"{p.Id} {p.Nombre,-20} {p.Precio,8:c}  Stock: {p.Stock}");
            break;

        case "2":
            var agotados = await TraerAgotadosAsync();
            Console.WriteLine("\n=== Productos a reponer ===");
            foreach (var p in agotados)
                Console.WriteLine($"{p.Id} {p.Nombre,-20} Stock: {p.Stock}");
            break;

        case "3":
            Console.Write("ID del producto: ");
            int idA = int.Parse(Console.ReadLine()!);
            Console.Write("Cantidad a agregar: ");
            int cantA = int.Parse(Console.ReadLine()!);
            await AgregarStockAsync(idA, cantA);
            break;

        case "4":
            Console.Write("ID del producto: ");
            int idQ = int.Parse(Console.ReadLine()!);
            Console.Write("Cantidad a quitar: ");
            int cantQ = int.Parse(Console.ReadLine()!);
            await QuitarStockAsync(idQ, cantQ);
            break;

        case "0":
            return;
    }
}

async Task<List<Producto>> TraerAsync() {
    var json = await http.GetStringAsync($"{baseUrl}/productos");
    return JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt)!;
}

async Task<List<Producto>> TraerAgotadosAsync() {
    var json = await http.GetStringAsync($"{baseUrl}/productos/agotados");
    return JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt)!;
}

async Task AgregarStockAsync(int id, int cantidad) {
    var content = new StringContent("", Encoding.UTF8, "application/json");
    var resp = await http.PostAsync($"{baseUrl}/productos/{id}/agregar?cantidad={cantidad}", content);
    Console.WriteLine(await resp.Content.ReadAsStringAsync());
}

async Task QuitarStockAsync(int id, int cantidad) {
    var content = new StringContent("", Encoding.UTF8, "application/json");
    var resp = await http.PostAsync($"{baseUrl}/productos/{id}/quitar?cantidad={cantidad}", content);
    Console.WriteLine(await resp.Content.ReadAsStringAsync());
}

class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}
