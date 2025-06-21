
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

while(true){
    Console.Clear();
    Console.WriteLine("\n=== alta tienda ===");
    Console.WriteLine("1. productos disponibles");
    Console.WriteLine("2. Productos que debemos reponer");
    Console.WriteLine("3. Agregar stock de un producto");
    Console.WriteLine("4. Quitar stock de un producto");
    Console.WriteLine("5. Salir");
    Console.Write("Ingrese una opcion: ");
    var op = Console.ReadLine();

    switch (op) {
        case "1":
            await productosdisponibles();
            break;
        case "2":
            await productosreponer();
            break;
        case "3":
            await modificarstock("agregar");
            break;
        case "4":
            await modificarstock("quitar");
            break;
        case "5":
            Console.WriteLine("Saliendo...");
            return;
        default:    
            Console.WriteLine("Opcion no valida");
            break;
    }
    Console.WriteLine("\nPresione una tecla para continuar...");
    Console.ReadKey();
}
async Task productosdisponibles(){
    var res = await http.GetAsync($"{baseUrl}/productos");
    if (res.IsSuccessStatusCode) {
        var json = await res.Content.ReadAsStringAsync();
        var productos = JsonSerializer.Deserialize<Producto[]>(json, jsonOpt);
        Console.WriteLine("=== Productos disponibles ===");
        foreach (var p in productos) {
            Console.WriteLine($"Id: {p.Id}, Nombre: {p.Nombre}, Precio: {p.Precio}, Stock: {p.Stock}");
        }
    } else {
        Console.WriteLine("Error al obtener los productos");
    }
}

async Task productosreponer(){
    var res = await http.GetAsync($"{baseUrl}/productos");
    if (res.IsSuccessStatusCode) {
        var json = await res.Content.ReadAsStringAsync();
        var productos = JsonSerializer.Deserialize<Producto[]>(json, jsonOpt);
        Console.WriteLine("=== Productos que debemos reponer ===");
        if (productos == null || productos.Length == 0) {
            Console.WriteLine("No hay productos para reponer");
            return;
        }
        foreach (var p in productos.Where(p => p.Stock < 3)) {
            Console.WriteLine($"Id: {p.Id}, Nombre: {p.Nombre}, Precio: {p.Precio}, Stock: {p.Stock}");
        }
    } else {
        Console.WriteLine("Error al obtener los productos");
    }
}

async Task modificarstock(string op){
    Console.Write("Ingrese el id del producto: ");
    var id = int.Parse(Console.ReadLine());
    Console.Write("Ingrese la cantidad a modificar: ");
    var cantidad = int.Parse(Console.ReadLine());
    var res = await http.GetAsync($"{baseUrl}/productos/{id}");
    if (res.IsSuccessStatusCode) {
        var json = await res.Content.ReadAsStringAsync();
        var producto = JsonSerializer.Deserialize<Producto>(json, jsonOpt);
        if (op == "agregar") {
            producto.Stock += cantidad;
        } else if (op == "quitar") {
            if (producto.Stock - cantidad < 0) {
                Console.WriteLine("No se puede quitar mas stock del que hay");
                return;
            }
            producto.Stock -= cantidad;
        }
        var jsonProducto = JsonSerializer.Serialize(producto, jsonOpt);
        var content = new StringContent(jsonProducto, System.Text.Encoding.UTF8, "application/json");
        res = await http.PutAsync($"{baseUrl}/productos/{id}", content);
        if (res.IsSuccessStatusCode) {
            Console.WriteLine("Stock modificado correctamente");
        } else {
            Console.WriteLine("Error al modificar el stock");
        }
    } else {
        Console.WriteLine("Error al obtener el producto");
    }
}
class Producto{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}