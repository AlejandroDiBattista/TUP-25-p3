using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

var baseUrl = "http://localhost:5000";
var http = new HttpClient();
var jsonOpt = new JsonSerializerOptions {
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true
};


var nombresProductos = new Dictionary<int, string> {
    {1, "Martillo"},
    {2, "Taladro"},
    {3, "Destornillador"},
    {4, "Sierra"},
    {5, "Llave inglesa"},
    {6, "Cinta métrica"},
    {7, "Alicate"},
    {8, "Nivel"},
    {9, "Pistola de pegamento"},
    {10, "Lijadora"}
};

// Diccionario para precios personalizados por Id
var preciosProductos = new Dictionary<int, decimal> {
    {1, 150.50m},
    {2, 230.00m},
    {3, 99.99m},
    {4, 180.75m},
    {5, 120.00m},
    {6, 45.30m},
    {7, 78.80m},
    {8, 60.40m},
    {9, 200.00m},
    {10, 340.90m}
};

bool salir = false;
while (!salir)
{
    Console.WriteLine("\n=== MENÚ ===");
    Console.WriteLine("1. Listar productos");
    Console.WriteLine("2. Listar productos a reponer");
    Console.WriteLine("3. Agregar stock");
    Console.WriteLine("4. Quitar stock");
    Console.WriteLine("0. Salir");
    Console.Write("Opción: ");
    var opcion = Console.ReadLine();

    switch (opcion)
    {
        case "1":
            await Listar("/productos");
            break;
        case "2":
            await Listar("/reposicion");
            break;
        case "3":
            await ModificarStock("agregar-stock");
            break;
        case "4":
            await ModificarStock("quitar-stock");
            break;
        case "0":
            salir = true;
            break;
        default:
            Console.WriteLine("Opción inválida");
            break;
    }
}

async Task Listar(string ruta)
{
    try {
        var productos = await http.GetFromJsonAsync<List<Producto>>($"{baseUrl}{ruta}", jsonOpt);

        productos = productos.OrderBy(p => p.Id).ToList();

        Console.WriteLine("\nID  Nombre                Precio    Stock");
        foreach (var p in productos) {
            var nombreModificado = nombresProductos.ContainsKey(p.Id) ? nombresProductos[p.Id] : p.Nombre;
            var precioModificado = preciosProductos.ContainsKey(p.Id) ? preciosProductos[p.Id] : p.Precio;

            Console.WriteLine($"{p.Id,2}  {nombreModificado,-20} {precioModificado,8:C} {p.Stock,6}");
        }
    } catch {
        Console.WriteLine("Error al obtener los productos.");
    }
}

async Task ModificarStock(string accion)
{
    Console.Write("Ingrese ID de producto: ");
    if (!int.TryParse(Console.ReadLine(), out int id)) return;
    Console.Write("Ingrese cantidad: ");
    if (!int.TryParse(Console.ReadLine(), out int cantidad)) return;

    var url = $"{baseUrl}/{accion}?id={id}&cantidad={cantidad}";
    var response = await http.PostAsync(url, null);

    if (response.IsSuccessStatusCode) {
        var producto = await response.Content.ReadFromJsonAsync<Producto>(jsonOpt);
        Console.WriteLine($"Nuevo stock de '{producto!.Nombre}': {producto.Stock}");
    } else {
        var error = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Error: {error}");
    }
}

class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}
