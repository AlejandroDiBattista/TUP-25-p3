using System;                     // Console, etc.
using System.Linq;                // OrderBy
using System.Net.Http;            // HttpClient, StringContent ✔
using System.Text.Json;           // JsonSerializer, JsonNamingPolicy
using System.Threading.Tasks;     // Task
using static System.Console;

var baseUrl = "http://localhost:5000";
var http = new HttpClient();
var jsonOpt = new JsonSerializerOptions {
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true
};
// Codigo de ejemplo: Reemplazar por la implementacion real 

async Task<List<Producto>> TraerProductosAsync()
{
    var json = await http.GetStringAsync($"{baseUrl}/productos");
    return JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt)!;
}
async Task<List<Producto>> TraerParaReponerAsync()
{
    var json = await http.GetStringAsync($"{baseUrl}/productos/reponer");
    return JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt)!;
}
async Task<bool> AgregarStockAsync(int id, int cantidad)
{
    var content = new StringContent(
        JsonSerializer.Serialize(new { cantidad }),
        System.Text.Encoding.UTF8, "application/json"
    );
    var resp = await http.PutAsync($"{baseUrl}/productos/{id}/agregar-stock", content);
    return resp.IsSuccessStatusCode;
}
async Task<bool> QuitarStockAsync(int id, int cantidad) {
    var content = new StringContent(
        JsonSerializer.Serialize(new { cantidad }),
        System.Text.Encoding.UTF8, "application/json"
    );
    var resp = await http.PutAsync($"{baseUrl}/productos/{id}/quitar-stock", content);
    return resp.IsSuccessStatusCode;
}
class Producto {
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}
while (true)
{
    Clear();
    WriteLine("\n=== Menu ===");
    WriteLine("1. Listar Productos");
    WriteLine("2. Listar Productos a reponer");
    WriteLine("3. Agregar Stock a un Producto");
    WriteLine("4. Quitar Stock a un Producto");
    WriteLine("5. Salir");
    Write("Seleccione una opción: ");
    var opcion = ReadLine();
    switch (opcion)
    {
        case "1":
            var productos = await TraerProductosAsync();
            WriteLine("\n=== Productos ===");
            foreach (var p in productos)
            {
                WriteLine($"{p.Id} {p.Nombre,-20} {p.Precio,10:c} Stock: {p.Stock}");
                WriteLine("\n--------------------------------------------------------------");
            }
            ReadKey();
            break;
        case "2":
            var BStock = await TraerParaReponerAsync();
            WriteLine("\n=== Productos con poco stock ===");
            foreach (var p in BStock) { 
            WriteLine($"{p.Id} {p.Nombre,-20} Stock: {p.Stock}"); }
            ReadKey();
            break;
        case "3":
                Clear();
                Write("Ingrese ID de producto: ");
            if (int.TryParse(ReadLine(), out int idAgregar))
            {
                Write("Cantidad a agregar: ");
                if (int.TryParse(ReadLine(), out int cantAgregar))
                {
                    if (await AgregarStockAsync(idAgregar, cantAgregar))
                    { 
                    WriteLine("Stock agregado correctamente.");
                    ReadKey();
                    }
                else
                    {
                    WriteLine("Error al agregar stock.");
                    ReadKey();
                    }
                }
            }
            break;
        case "4":
                Clear();
                Write("Ingrese ID de producto: ");
            if (int.TryParse(ReadLine(), out int idQuitar))
            {
                Write("Cantidad a quitar: ");
                if (int.TryParse(ReadLine(), out int cantQuitar))
                {
                    if (await QuitarStockAsync(idQuitar, cantQuitar))
                    {   WriteLine("Stock quitado correctamente.");
                    ReadKey();
                    }
                    else
                    {
                    WriteLine("Error al quitar stock.");
                    ReadKey();
                    }
                }
            }
            break;
        case "5":
            return;
        default:
            WriteLine("Opción no válida");
            ReadKey();
            //Clear();
            break;
    }
}
// Fin del ejemplo