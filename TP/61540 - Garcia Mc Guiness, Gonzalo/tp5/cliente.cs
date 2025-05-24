using System;                     // Console, etc.
using System.Linq;                // OrderBy
using System.Net.Http;            // HttpClient, StringContent ✔
using System.Text.Json;           // JsonSerializer, JsonNamingPolicy
using System.Threading.Tasks;     // Task
using static System.Console;           // WriteLine, ReadLine

var baseUrl = "http://localhost:5000";
var http = new HttpClient();
var jsonOpt = new JsonSerializerOptions {
    PropertyNamingPolicy        = JsonNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true
};

// Codigo de ejemplo: Reemplazar por la implementacion real 
//TRAE PRODUCTOS
async Task<List<Producto>> TraerAsync()
{
    var json = await http.GetStringAsync($"{baseUrl}/productos");
    return JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt)!;
}
//TRAER PRODUCTOS CON STOCK BAJO
async Task<List<Producto>> TraerBajoStockAsync()
{
    var json = await http.GetStringAsync($"{baseUrl}/productos/bajo-stock");
    return JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt)!;
}
//AGREGAR STOCK
async Task<bool> AgregarStockAsync(int id, int cantidad)
{
    var url = $"{baseUrl}/productos/{id}/agregar/{cantidad}";
    var response = await http.PostAsync(url, null);
    return response.IsSuccessStatusCode;
}
//QUITAR STOCK
async Task<bool> QuitarStockAsync(int id, int cantidad)
{
    var url = $"{baseUrl}/productos/{id}/quitar/{cantidad}";
    var response = await http.PostAsync(url, null);
    return response.IsSuccessStatusCode;
}
class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}

while (true)
{
    WriteLine("=== Menú ===");
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
            {
                WriteLine("No hay productos con stock bajo.");
            }
            else
            {
                foreach (var p in bajoStock)
                    WriteLine($"{p.Id} {p.Nombre,-20} {p.Precio,15:c} Stock {p.Stock}");
            }
            break;

        case "3":
            Write("ID del producto: ");
            if (!int.TryParse(ReadLine(), out int idAgregar))
            {
                WriteLine("ID invalido");
                break;
            }
            Write("Cantidad a agregar: ");
            if (!int.TryParse(ReadLine(), out int cantidadAgregar) || cantidadAgregar <= 0)
            {
                WriteLine("Cantidad invalida");
                break;
            }
            var okAgregar = await AgregarStockAsync(idAgregar, cantidadAgregar);
            if (okAgregar)
            {
                WriteLine("Stock agregado correctamente.");
            }
            else
            {
                WriteLine("Error al agregar stock.");
            }
            break;

        case "4":
            Write("ID del producto: ");
            if (!int.TryParse(ReadLine(), out int idQuitar))
            {
                WriteLine("ID invalido");
                break;
            }
            Write("Cantidad a quitar: ");
            if (!int.TryParse(ReadLine(), out int cantidadQuitar) || cantidadQuitar <= 0)
            {
                WriteLine("Cantidad invalida");
                break;
            }
            var okQuitar = await QuitarStockAsync(idQuitar, cantidadQuitar);
            if (okQuitar)
                WriteLine("Stock quitado correctamente.");
            else
                WriteLine("Error al quitar stock");
            break;

        case "5":
            WriteLine("Saliendo...");
            return;

        default:
            WriteLine("Opción no válida.");
            break;
    }
}
// Fin del ejemplo