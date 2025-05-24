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

// Menú de opciones
System.Globalization.CultureInfo.DefaultThreadCurrentCulture = new System.Globalization.CultureInfo("es-AR");
while (true)
{
    Console.Clear();
    Console.WriteLine("=== Menú ===");
    Console.WriteLine("1. Listar productos");
    Console.WriteLine("2. Listar productos con stock bajo");
    Console.WriteLine("3. Agregar stock a un producto");
    Console.WriteLine("4. Quitar stock a un producto");
    Console.WriteLine("0. Salir");
    Console.WriteLine("--------------------------------------");
    Console.Write("Seleccione una opción: ");
    var opcion = Console.ReadLine();

    if (opcion == "0") break;

    switch (opcion)
    {
        case "1":
            {
                Console.Clear();
                // For para mostrar los productos
                Console.WriteLine("=== Productos ===");
                var productos = await TraerAsync();

                Console.WriteLine($"{"ID",5} {"Nombre",-20} {"Precio",15} {"Stock",10}");

                foreach (var p in productos)
                {
                    Console.WriteLine($"{p.Id,5} {p.Nombre,-20} {p.Precio,15:c} {p.Stock,10}");
                }
                Console.WriteLine("--------------------------------------");
                Console.WriteLine("Presione una tecla para continuar...");
                Console.ReadKey();
            }
            break;
        case "2":
            {
                Console.Clear();
                // For para mostrar los productos con stock bajo
                Console.WriteLine("=== Productos con stock bajo ===");
                var productos = await TraerStockBajoAsync();

                Console.WriteLine($"{"ID",5} {"Nombre",-20} {"Precio",15} {"Stock",10}");

                foreach (var p in productos)
                {
                    Console.WriteLine($"{p.Id,5} {p.Nombre,-20} {p.Precio,15:c} {p.Stock,10}");
                }
                Console.WriteLine("--------------------------------------");
                Console.WriteLine("Presione una tecla para continuar...");
                Console.ReadKey();
            }
            break;
        case "3":
            await AgregarStockAsync();
            break;
        case "4":
            await QuitarStockAsync();
            break;
        default:
            Console.WriteLine("--------------------------------------");
            Console.WriteLine("Opción no válida.");
            Console.WriteLine("--------------------------------------");
            Console.WriteLine("Presione una tecla para continuar...");
            Console.ReadKey();
            break;
    }
}

// Codigo de ejemplo: Reemplazar por la implementacion real 

async Task<List<Producto>> TraerAsync()
{
    var json = await http.GetStringAsync($"{baseUrl}/productos");
    return JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt)!;
}

// Función para obtener los productos con stock bajo
async Task<List<Producto>> TraerStockBajoAsync()
{
    var json = await http.GetStringAsync($"{baseUrl}/productos/stock-bajo");
    return JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt)!;
}

// Funcion para agregar stock a un producto
async Task AgregarStockAsync()
{
    Console.Clear();
    Console.Write("Ingresá el ID del producto: ");
    if (!int.TryParse(Console.ReadLine(), out var id))
    {
        Console.WriteLine("--------------------------------------");
        Console.WriteLine("ID inválido. Presione una tecla para continuar...");
        Console.ReadKey();
        return;
    }

    Console.WriteLine("--------------------------------------");

    Console.Write("Ingresá la cantidad a agregar: ");
    if (!int.TryParse(Console.ReadLine(), out var cantidad))
    {
        Console.WriteLine("--------------------------------------");
        Console.WriteLine("Cantidad inválida. Presione una tecla para continuar...");
        Console.ReadKey();
        return;
    }

    Console.WriteLine("--------------------------------------");

    var stock = new ModificarStock { Cantidad = cantidad };
    var json = JsonSerializer.Serialize(stock, jsonOpt);
    var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

    var response = await http.PutAsync($"{baseUrl}/productos/{id}/agregar-stock", content);

    if (response.IsSuccessStatusCode)
    {
        var body = await response.Content.ReadAsStringAsync();
        var productoActualizado = JsonSerializer.Deserialize<Producto>(body, jsonOpt);

        Console.WriteLine($"Stock agregado correctamente. Nuevo stock: {productoActualizado!.Stock}");
    }
    else
    {
        var errorMsg = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Error al actualizar el stock: {response.StatusCode}");
        Console.WriteLine("--------------------------------------");    
        Console.WriteLine($"Mensaje de error: {errorMsg}");
    }
    Console.WriteLine("--------------------------------------");
    Console.WriteLine("Presione una tecla para continuar...");
    Console.ReadKey();
}

// Funcion para quitar stock a un producto
async Task QuitarStockAsync()
{
    Console.Clear();
    Console.Write("Ingresá el ID del producto: ");
    if (!int.TryParse(Console.ReadLine(), out var id))
    {
        Console.WriteLine("--------------------------------------");
        Console.WriteLine("ID inválido. Presione una tecla para continuar...");
        Console.ReadKey();
        return;
    }

    Console.WriteLine("--------------------------------------");

    Console.Write("Ingresá la cantidad a quitar: ");
    if (!int.TryParse(Console.ReadLine(), out var cantidad))
    {
        Console.WriteLine("--------------------------------------");
        Console.WriteLine("Cantidad inválida. Presione una tecla para continuar...");
        Console.ReadKey();
        return;
    }

    Console.WriteLine("--------------------------------------");

    var stock = new ModificarStock { Cantidad = cantidad };
    var json = JsonSerializer.Serialize(stock, jsonOpt);
    var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

    var response = await http.PutAsync($"{baseUrl}/productos/{id}/quitar", content);

    if (response.IsSuccessStatusCode)
    {
        var body = await response.Content.ReadAsStringAsync();
        var productoActualizado = JsonSerializer.Deserialize<Producto>(body, jsonOpt);

        Console.WriteLine($"Stock quitado correctamente. Nuevo stock: {productoActualizado!.Stock}");
    }
    else
    {
        var errorMsg = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Error al actualizar el stock: {response.StatusCode}");
        Console.WriteLine("--------------------------------------");    
        Console.WriteLine($"Mensaje de error: {errorMsg}");
    }
    Console.WriteLine("--------------------------------------");
    Console.WriteLine("Presione una tecla para continuar...");
    Console.ReadKey();
}

class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}

class ModificarStock
{
    public int Cantidad { get; set; }
}

// Fin del ejemplo