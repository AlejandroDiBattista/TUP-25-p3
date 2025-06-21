using System;                     
using System.Linq;                
using System.Net.Http;            
using System.Text.Json;           
using System.Threading.Tasks;     

var baseUrl = "http://localhost:5000";
var http = new HttpClient();
var jsonOpt = new JsonSerializerOptions {
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true
};

async Task MostrarMenu() {
    Console.Clear();
    Console.WriteLine("=== MENU TIENDA ===");
    Console.WriteLine("1. Listar productos");
    Console.WriteLine("2. Listar productos a reponer (stock < 3)");
    Console.WriteLine("3. Agregar stock a producto");
    Console.WriteLine("4. Quitar stock a producto");
    Console.WriteLine("5. Salir");
    Console.Write("Seleccione una opción: ");
}

async Task<List<Producto>> ListarProductos() {
    var json = await http.GetStringAsync($"{baseUrl}/productos");
    return JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt)!;
}

async Task<List<Producto>> ListarProductosReponer() {
    var json = await http.GetStringAsync($"{baseUrl}/productos-reponer");
    return JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt)!;
}

async Task MostrarProductos(List<Producto> productos) {
    Console.WriteLine("ID  Nombre               Precio     Stock");
    Console.WriteLine("-----------------------------------------");
    foreach (var p in productos) {
        Console.WriteLine($"{p.Id,-3} {p.Nombre,-20} {p.Precio,8:c} {p.Stock,8}");
    }
    Console.WriteLine();
}

async Task AgregarStock() {
    Console.Write("ID del producto: ");
    if (!int.TryParse(Console.ReadLine(), out int id)) {
        Console.WriteLine("ID inválido");
        return;
    }
    
    Console.Write("Cantidad a agregar: ");
    if (!int.TryParse(Console.ReadLine(), out int cantidad) || cantidad <= 0) {
        Console.WriteLine("Cantidad inválida");
        return;
    }
    
    var response = await http.PostAsync($"{baseUrl}/agregar-stock/{id}/{cantidad}", null);
    if (response.IsSuccessStatusCode) {
        Console.WriteLine("Stock actualizado correctamente");
    } else {
        Console.WriteLine("Error: " + await response.Content.ReadAsStringAsync());
    }
}

async Task QuitarStock() {
    Console.Write("ID del producto: ");
    if (!int.TryParse(Console.ReadLine(), out int id)) {
        Console.WriteLine("ID inválido");
        return;
    }
    
    Console.Write("Cantidad a quitar: ");
    if (!int.TryParse(Console.ReadLine(), out int cantidad) || cantidad <= 0) {
        Console.WriteLine("Cantidad inválida");
        return;
    }
    
    var response = await http.PostAsync($"{baseUrl}/quitar-stock/{id}/{cantidad}", null);
    if (response.IsSuccessStatusCode) {
        Console.WriteLine("Stock actualizado correctamente");
    } else {
        Console.WriteLine("Error: " + await response.Content.ReadAsStringAsync());
    }
}

// Menú principal
bool salir = false;
while (!salir) {
    await MostrarMenu();
    var opcion = Console.ReadLine();
    
    switch (opcion) {
        case "1":
            await MostrarProductos(await ListarProductos());
            break;
        case "2":
            await MostrarProductos(await ListarProductosReponer());
            break;
        case "3":
            await AgregarStock();
            break;
        case "4":
            await QuitarStock();
            break;
        case "5":
            salir = true;
            break;
        default:
            Console.WriteLine("Opción inválida");
            break;
    }
    
    if (!salir) {
        Console.WriteLine("Presione cualquier tecla para continuar...");
        Console.ReadKey();
    }
}

class Producto {
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}