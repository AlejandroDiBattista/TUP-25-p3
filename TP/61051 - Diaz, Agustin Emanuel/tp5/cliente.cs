using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

var http = new HttpClient();
var baseUrl = "http://localhost:5000";
var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, PropertyNameCaseInsensitive = true };

await InicializarProductosAsync();

while (true) {
    Console.Clear();
    Console.WriteLine("""
    ========= TIENDA =========
    1. Listar productos
    2. Productos a reponer
    3. Agregar stock
    4. Quitar stock
    0. Salir
    ==========================
    """);

    Console.Write("> ");
    var op = Console.ReadLine();

    Console.Clear();

    switch (op) {
        case "0":
            return;

        case "1":
            var productosJson = await http.GetStringAsync($"{baseUrl}/productos");
            var productos = JsonSerializer.Deserialize<List<Producto>>(productosJson, options)!;
            Mostrar(productos);
            break;

        case "2":
            var reponerJson = await http.GetStringAsync($"{baseUrl}/productos/reponer");
            var reponer = JsonSerializer.Deserialize<List<Producto>>(reponerJson, options)!;
            Mostrar(reponer);
            break;

        case "3":
            Console.Write("ID del producto: ");
            int idAdd = int.Parse(Console.ReadLine()!);
            Console.Write("Cantidad a agregar: ");
            int cantidadAdd = int.Parse(Console.ReadLine()!);
            await http.PostAsync($"{baseUrl}/productos/agregar", CrearContenido(new MovimientoStock { Id = idAdd, Cantidad = cantidadAdd }));
            Console.WriteLine("Stock actualizado.");
            break;

        case "4":
            Console.Write("ID del producto: ");
            int idDel = int.Parse(Console.ReadLine()!);
            Console.Write("Cantidad a quitar: ");
            int cantidadDel = int.Parse(Console.ReadLine()!);
            var resp = await http.PostAsync($"{baseUrl}/productos/quitar", CrearContenido(new MovimientoStock { Id = idDel, Cantidad = cantidadDel }));
            var msg = await resp.Content.ReadAsStringAsync();
            Console.WriteLine(resp.IsSuccessStatusCode ? "Stock actualizado." : $"Error: {msg}");
            break;

        default:
            Console.WriteLine("Opción no válida.");
            break;
    }

    Console.WriteLine("\nPresione ENTER para continuar...");
    Console.ReadLine();
}

static void Mostrar(List<Producto> lista) {
    Console.WriteLine("\n ID  | Producto               | Stock");
    Console.WriteLine("-----|------------------------|-------");
    foreach (var p in lista) {
        Console.WriteLine($"{p.Id,3}  | {p.Nombre,-22} | {p.Stock,5}");
    }
}

static StringContent CrearContenido(object data) =>
    new(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");

async Task InicializarProductosAsync() {
    try {
        var respuesta = await http.GetStringAsync($"{baseUrl}/productos");
        var existentes = JsonSerializer.Deserialize<List<Producto>>(respuesta, options)!;

        if (existentes.Count == 0) {
            Console.WriteLine("Inicializando productos...");
            for (int i = 1; i <= 10; i++) {
                var nuevo = new Producto { Nombre = $"Producto {i}", Stock = 10 };
                await http.PostAsync($"{baseUrl}/productos", CrearContenido(nuevo));
            }
            Console.WriteLine("Productos creados.");
            await Task.Delay(1000);
        }
    } catch {
        Console.WriteLine("Error al conectar con el servidor. ¿Está en ejecución?");
        Environment.Exit(1);
    }
}

class Producto {
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public int Stock { get; set; }
}

class MovimientoStock {
    public int Id { get; set; }
    public int Cantidad { get; set; }
}