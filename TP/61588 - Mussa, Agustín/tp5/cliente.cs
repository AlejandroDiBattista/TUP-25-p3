using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Console;

var baseUrl = "http://localhost:5000";
var http = new HttpClient();
var jsonOpt = new JsonSerializerOptions {
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true
};

// TRAE PRODUCTOS
async Task<List<Producto>> TraerAsync()
{
    var json = await http.GetStringAsync($"{baseUrl}/productos");
    return JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt)!;
}

// TRAE PRODUCTOS CON STOCK BAJO
async Task<List<Producto>> TraerBajoStockAsync()
{
    var json = await http.GetStringAsync($"{baseUrl}/productos/bajo-stock");
    return JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt)!;
}

// AGREGAR STOCK (POST con JSON)
async Task<bool> AgregarStockAsync(int id, int cantidad)
{
    var datos = new { id, cantidad };
    var content = new StringContent(JsonSerializer.Serialize(datos), Encoding.UTF8, "application/json");
    var response = await http.PostAsync($"{baseUrl}/productos/agregar-stock", content);
    return response.IsSuccessStatusCode;
}

// QUITAR STOCK (POST con JSON)
async Task<bool> QuitarStockAsync(int id, int cantidad)
{
    var datos = new { id, cantidad };
    var content = new StringContent(JsonSerializer.Serialize(datos), Encoding.UTF8, "application/json");
    var response = await http.PostAsync($"{baseUrl}/productos/quitar-stock", content);
    return response.IsSuccessStatusCode;
}

// CLASE LOCAL
class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}

// INTERFAZ DE CONSOLA
while (true)
{
    WriteLine("\n=== Menú ===");
    WriteLine("1. Ver todos los productos");
    WriteLine("2. Ver productos con stock bajo");
    WriteLine("3. Agregar stock a un producto");
    WriteLine("4. Quitar stock a un producto");
    WriteLine("5. Salir");

    Write("Seleccione una opción: ");
    var opcion = ReadLine();
    if (opcion == "5") break;

    switch (opcion)
    {
        case "1":
            var productos = await TraerAsync();
            WriteLine("\n=== Productos ===");
            foreach (var p in productos)
                WriteLine($"{p.Id} {p.Nombre,-20} {p.Precio,15:c} Stock {p.Stock}");
            break;

        case "2":
            var bajoStock = await TraerBajoStockAsync();
            WriteLine("\n=== Productos con stock bajo ===");
            if (bajoStock.Count == 0)
                WriteLine("No hay productos con stock bajo.");
            else
                foreach (var p in bajoStock)
                    WriteLine($"{p.Id} {p.Nombre,-20} {p.Precio,15:c} Stock {p.Stock}");
            break;

        case "3":
            Write("ID del producto: ");
            if (!int.TryParse(ReadLine(), out int idAgregar))
            {
                WriteLine("ID inválido");
                break;
            }
            Write("Cantidad a agregar: ");
            if (!int.TryParse(ReadLine(), out int cantidadAgregar) || cantidadAgregar <= 0)
            {
                WriteLine("Cantidad inválida");
                break;
            }
            var okAgregar = await AgregarStockAsync(idAgregar, cantidadAgregar);
            WriteLine(okAgregar ? "Stock agregado correctamente." : "Error al agregar stock.");
            break;

        case "4":
            Write("ID del producto: ");
            if (!int.TryParse(ReadLine(), out int idQuitar))
            {
                WriteLine("ID inválido");
                break;
            }
            Write("Cantidad a quitar: ");
            if (!int.TryParse(ReadLine(), out int cantidadQuitar) || cantidadQuitar <= 0)
            {
                WriteLine("Cantidad inválida");
                break;
            }
            var okQuitar = await QuitarStockAsync(idQuitar, cantidadQuitar);
            WriteLine(okQuitar ? "Stock quitado correctamente." : "Error al quitar stock.");
            break;

        default:
            WriteLine("Opción no válida.");
            break;
    }
}
