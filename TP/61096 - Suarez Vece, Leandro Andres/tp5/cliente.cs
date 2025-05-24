using System;                     // Console, etc.
using System.Linq;                // OrderBy
using System.Net.Http;            // HttpClient, StringContent ✔
using System.Text.Json;           // JsonSerializer, JsonNamingPolicy
using System.Threading.Tasks;     // Task

var baseUrl = "http://localhost:5000";
var http = new HttpClient();
var jsonOpt = new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true
};

// Codigo de ejemplo: Reemplazar por la implementacion real 

async Task<List<Producto>> TraerAsync()
{
    var json = await http.GetStringAsync($"{baseUrl}/productos");
    return JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt)!;
}

async Task<List<Producto>> TraerListaReponerAsync(int stock)
{
    var json = await http.GetStringAsync($"{baseUrl}/productos/{stock}");
    return JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt)!;
}

async Task<List<Producto>> AgregarStockAsync(int id, int stock)
{
    var response = await http.PutAsync($"{baseUrl}/productos/agregar/{id}/{stock}", null);
    if (!response.IsSuccessStatusCode)
    {
        throw new Exception($"Error: {response.StatusCode}");
    }

    var json = await response.Content.ReadAsStringAsync();
    return JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt)!;
}
async Task<List<Producto>> QuitarStockAsync(int id, int stock)
{
    var response = await http.PutAsync($"{baseUrl}/productos/quitar/{id}/{stock}", null);
    if (!response.IsSuccessStatusCode)
    {
        throw new Exception($"Error: {response.StatusCode}");
    }

    var json = await response.Content.ReadAsStringAsync();
    return JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt)!;
}

static int LeerEntero(string mensaje)
{
    int valor;
    while (true)
    {
        Console.Write(mensaje);
        if (int.TryParse(Console.ReadLine(), out valor))
            return valor;

        Console.WriteLine("Por favor ingresa un número válido.");
    }
}

int opcion = 0;
int idPorduct = 0;
int cantidad = 0;
do
{
    Console.Clear();
    Console.WriteLine("===== MENÚ =====");
    Console.WriteLine("1. Listar todos los productos");
    Console.WriteLine("2. Listar los productos con poco Stock");
    Console.WriteLine("3. Agregar Stock a un producto");
    Console.WriteLine("4. Quitar Stock a un producto");
    Console.WriteLine("0. Salir");
    Console.Write("Elige una opción: ");

    opcion = LeerEntero("ingrese una opcion: ");

    switch (opcion)
    {
        case 1:
            Console.WriteLine("=== Productos ===");
            foreach (var p in await TraerAsync())
            {
                Console.WriteLine($"{p.Id} {p.Nombre,-5} {p.Stock,5} {p.Precio,15:c} ");
            }
            break;
        case 2:
            Console.WriteLine("=== Productos Poco Stock===");
            foreach (var p in await TraerListaReponerAsync(3))
            {
                Console.WriteLine($"{p.Id} {p.Nombre,-5} {p.Stock,5} {p.Precio,15:c} ");
            }
            break;
        case 3:
            idPorduct = LeerEntero("ingrese el id del producto que desea agregar stock: ");
            cantidad = LeerEntero("ingrese la cantidad a modificar: ");
            AgregarStockAsync(idPorduct, cantidad);
            break;
        case 4:
            idPorduct = LeerEntero("ingrese el id del producto que desea quitar stock: ");
            cantidad = LeerEntero("ingrese la cantidad a modificar: ");
            var res = QuitarStockAsync(idPorduct, cantidad);


            break;
        case 0:
            Console.WriteLine("Saliendo...");
            break;
        default:
            Console.WriteLine("Opción no válida, intenta de nuevo.");
            break;
    }

    if (opcion != 0)
    {
        Console.WriteLine("\nPresiona una tecla para continuar...");
        Console.ReadKey();
    }

} while (opcion != 0);



class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public decimal Precio { get; set; }
    public int Stock { get; set; }

}

// Fin del ejemplo