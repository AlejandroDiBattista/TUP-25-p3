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

// Codigo de ejemplo: Reemplazar por la implementacion real 

async Task<List<Producto>> TraerAsync(){
    var json = await http.GetStringAsync($"{baseUrl}/productos");
    return JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt)!;
}

Console.WriteLine("=== Productos ===");
foreach (var p in await TraerAsync())
{
    Console.WriteLine($"{p.Id} {p.Nombre,-20} {p.Precio,15:c}");
}

bool salir = false;
while (!salir)
{ Console.Clear();
    Console.WriteLine("\n--- MENÚ ---");
    Console.WriteLine("1. Listar productos");
    Console.WriteLine("2. Listar productos a reponer");
    Console.WriteLine("3. Agregar stock a un producto");
    Console.WriteLine("4. Quitar stock a un producto");
    Console.WriteLine("5. Agregar un producto");
    Console.WriteLine("6. Salir");
    Console.Write("Seleccione una opción: ");
    var opcion = Console.ReadLine();

    switch (opcion)
    {
        case "1":
            var json = await http.GetStringAsync($"{baseUrl}/productos");
            var productos = JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt)!;
            Console.WriteLine("\nPRODUCTOS DISPONIBLES:");
            foreach (var p in productos.OrderBy(p => p.Nombre))
            {
                Console.WriteLine($"ID:{p.Id}. {p.Nombre,-20} - Precio: {p.Precio:c} - Stock: {p.Cantidad,-5}");
            }
            Console.WriteLine("\nPresione una tecla para continuar...");
            Console.ReadKey();
            break;

        case "2":
            var reponerJson = await http.GetStringAsync($"{baseUrl}/productos/reponer");
            var reponer = JsonSerializer.Deserialize<List<Producto>>(reponerJson, jsonOpt)!;
            Console.WriteLine("\nPRODUCTOS A REPONER:");

            foreach (var p in reponer.OrderBy(p => p.Nombre))
            {
                Console.WriteLine($"ID:{p.Id}. {p.Nombre,-20} - Precio: {p.Precio:c} - Stock: {p.Cantidad,-5}");
            }
            Console.WriteLine("\nPresione una tecla para continuar...");
            Console.ReadKey();
            break;

        case "3":
            Console.Write("ID producto: ");
            var idAgregar = Console.ReadLine();
            Console.Write("Cantidad a agregar: ");
            var cantidadAgregar = Console.ReadLine();
            var resAgregar = await http.PostAsync($"{baseUrl}/productos/{idAgregar}/agregar/{cantidadAgregar}", null);
            Console.WriteLine(resAgregar.IsSuccessStatusCode ? "Stock agregado." : "Error.");
            var Cambio = await http.GetStringAsync($"{baseUrl}/productos/porID/{idAgregar}");
            var CambioStock = JsonSerializer.Deserialize<Producto>(Cambio, jsonOpt)!;
            Console.WriteLine($"El nuevo stock del producto {CambioStock.Nombre} es {CambioStock.Cantidad}");
            Console.WriteLine("\nPresione una tecla para continuar...");
            Console.ReadKey();
            break;

        case "4":
            Console.Write("ID producto: ");
            var idQuitar = Console.ReadLine();
            Console.Write("Cantidad a quitar: ");
            var cantidadQuitar = Console.ReadLine();
            var resQuitar = await http.PostAsync($"{baseUrl}/productos/{idQuitar}/quitar/{cantidadQuitar}", null);
            Console.WriteLine(resQuitar.IsSuccessStatusCode ? "Stock quitado." : "Error.");
            var CambioQuitar = await http.GetStringAsync($"{baseUrl}/productos/porID/{idQuitar}");
            var CambioStockQuitar = JsonSerializer.Deserialize<Producto>(CambioQuitar, jsonOpt)!;
            Console.WriteLine($"El nuevo stock del producto {CambioStockQuitar.Nombre} es {CambioStockQuitar.Cantidad}");
            Console.WriteLine("\nPresione una tecla para continuar...");
            Console.ReadKey();
            break;
        case "5":
            Console.Write("Nombre del producto: ");
            var NombreP = Console.ReadLine();
            Console.Write("Precio del Producto: ");
            var PrecioP = Console.ReadLine();
            Console.Write("Stock del producto: ");
            var StockP = Console.ReadLine();
            var nuevoProducto = new Producto
            {
                Nombre = NombreP,
                Precio = decimal.Parse(PrecioP),
                Cantidad = int.Parse(StockP)
            };
            var nuevo = await http.PostAsync($"{baseUrl}/productos/agregarNuevo", 
                new StringContent(JsonSerializer.Serialize(nuevoProducto, jsonOpt), System.Text.Encoding.UTF8, "application/json"));
            Console.WriteLine(nuevo.IsSuccessStatusCode ? "Producto agregado." : "Error.");

             Console.WriteLine("\nPresione una tecla para continuar...");
            Console.ReadKey();   
            //var nuevoJson = await http.GetStringAsync($"{baseUrl}/productos/porID/{nuevoProducto.Id}");
            //var nuevoProductoAgregado = JsonSerializer.Deserialize<Producto>(nuevoJson, jsonOpt)!;
            //Console.WriteLine($"El nuevo producto agregado es {nuevoProductoAgregado.Nombre} con un precio de {nuevoProductoAgregado.Precio:c} y un stock de {nuevoProductoAgregado.Stock}");
            break;

        case "6":
            salir = true;
            break;

        default:
            Console.WriteLine("Opción inválida.");
            break;
    }
}

class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public decimal Precio { get; set; }
    public int Cantidad { get; set; }
}

// Fin del ejemplo