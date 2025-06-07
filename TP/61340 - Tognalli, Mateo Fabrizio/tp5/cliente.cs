using System;                     // Console, etc.
using System.Linq;                // OrderBy
using System.Net.Http;            // HttpClient, StringContent
using System.Text.Json;           // JsonSerializer, JsonNamingPolicy
using System.Threading.Tasks;     // Task

var baseUrl = "http://localhost:5000";
var http = new HttpClient();
var jsonOpt = new JsonSerializerOptions {
    PropertyNamingPolicy        = JsonNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true
};

// Implementación de métodos para consumir la API
async Task<List<Producto>> ObtenerProductosAsync() {
    var json = await http.GetStringAsync($"{baseUrl}/productos");
    return JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt)!;
}

async Task<List<Producto>> ObtenerProductosAReponerAsync() {
    var json = await http.GetStringAsync($"{baseUrl}/productos/reponer");
    return JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt)!;
}

async Task<Producto> AgregarStockAsync(int id, int cantidad) {
    var response = await http.PutAsync(
        $"{baseUrl}/productos/{id}/agregar-stock?cantidad={cantidad}", 
        null);
        
    if (!response.IsSuccessStatusCode)
        return new Producto();
        
    var json = await response.Content.ReadAsStringAsync();
    return JsonSerializer.Deserialize<Producto>(json, jsonOpt) ?? new Producto();
}

async Task<Producto> QuitarStockAsync(int id, int cantidad) {
    var response = await http.PutAsync(
        $"{baseUrl}/productos/{id}/quitar-stock?cantidad={cantidad}", 
        null);
        
    if (!response.IsSuccessStatusCode) {
        Console.WriteLine($"Error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
        return new Producto();
    }
        
    var json = await response.Content.ReadAsStringAsync();
    return JsonSerializer.Deserialize<Producto>(json, jsonOpt) ?? new Producto();
}

// Menú principal
bool salir = false;
while (!salir) {
    Console.Clear();
    Console.WriteLine("=== SISTEMA DE GESTIÓN DE STOCK ===");
    Console.WriteLine("1. Listar todos los productos");
    Console.WriteLine("2. Listar productos a reponer (stock < 3)");
    Console.WriteLine("3. Agregar stock a un producto");
    Console.WriteLine("4. Quitar stock de un producto");
    Console.WriteLine("0. Salir");
    Console.Write("\nSeleccione una opción: ");
    
    string opcion = Console.ReadLine() ?? "";
    
    switch (opcion) {
        case "1":
            await ListarProductos();
            break;
        case "2":
            await ListarProductosAReponer();
            break;
        case "3":
            await AgregarStock();
            break;
        case "4":
            await QuitarStock();
            break;
        case "0":
            salir = true;
            break;
        default:
            Console.WriteLine("Opción inválida. Presione cualquier tecla para continuar...");
            Console.ReadKey();
            break;
    }
}

// Implementación de opciones del menú
async Task ListarProductos() {
    Console.Clear();
    Console.WriteLine("=== LISTADO DE PRODUCTOS ===");
    Console.WriteLine($"{"ID",-5}{"NOMBRE",-30}{"PRECIO",-15}{"STOCK",-10}");
    Console.WriteLine(new string('-', 60));
      try {
        var productos = await ObtenerProductosAsync();
        foreach (var p in productos) {
            Console.WriteLine($"{p.Id,-5}{p.Nombre,-30}$ {p.Precio,13}{p.Stock,10}");
        }
    }
    catch (Exception ex) {
        Console.WriteLine($"Error al obtener productos: {ex.Message}");
    }
    
    Console.WriteLine("\nPresione cualquier tecla para continuar...");
    Console.ReadKey();
}

async Task ListarProductosAReponer() {
    Console.Clear();
    Console.WriteLine("=== PRODUCTOS A REPONER (STOCK < 3) ===");
    Console.WriteLine($"{"ID",-5}{"NOMBRE",-30}{"PRECIO",-15}{"STOCK",-10}");
    Console.WriteLine(new string('-', 60));
    
    try {
        var productos = await ObtenerProductosAReponerAsync();
        if (productos.Count == 0) {
            Console.WriteLine("No hay productos que requieran reposición.");
        } else {
            foreach (var p in productos) {
                Console.WriteLine($"{p.Id,-5}{p.Nombre,-30}$ {p.Precio,13}{p.Stock,10}");
            }
        }
    }
    catch (Exception ex) {
        Console.WriteLine($"Error al obtener productos a reponer: {ex.Message}");
    }
    
    Console.WriteLine("\nPresione cualquier tecla para continuar...");
    Console.ReadKey();
}

async Task AgregarStock() {
    Console.Clear();
    Console.WriteLine("=== AGREGAR STOCK A PRODUCTO ===");
    
    try {
        // Mostrar productos
        var productos = await ObtenerProductosAsync();
        Console.WriteLine($"{"ID",-5}{"NOMBRE",-30}{"STOCK ACTUAL",-15}");
        Console.WriteLine(new string('-', 50));
        foreach (var p in productos) {
            Console.WriteLine($"{p.Id,-5}{p.Nombre,-30}{p.Stock,15}");
        }
        
        // Solicitar ID del producto
        Console.Write("\nIngrese ID del producto: ");
        if (!int.TryParse(Console.ReadLine(), out int id)) {
            Console.WriteLine("ID inválido");
            Console.ReadKey();
            return;
        }
        
        // Solicitar cantidad a agregar
        Console.Write("Ingrese cantidad a agregar: ");
        if (!int.TryParse(Console.ReadLine(), out int cantidad) || cantidad <= 0) {
            Console.WriteLine("Cantidad inválida. Debe ser un número positivo.");
            Console.ReadKey();
            return;
        }
        
        // Realizar la operación
        var resultado = await AgregarStockAsync(id, cantidad);
        if (resultado.Id == 0) {
            Console.WriteLine("Error al agregar stock. Verifique que el ID sea correcto.");
        } else {
            Console.WriteLine($"Stock actualizado correctamente. Nuevo stock: {resultado.Stock}");
        }
    }
    catch (Exception ex) {
        Console.WriteLine($"Error: {ex.Message}");
    }
    
    Console.WriteLine("\nPresione cualquier tecla para continuar...");
    Console.ReadKey();
}

async Task QuitarStock() {
    Console.Clear();
    Console.WriteLine("=== QUITAR STOCK DE PRODUCTO ===");
    
    try {
        // Mostrar productos
        var productos = await ObtenerProductosAsync();
        Console.WriteLine($"{"ID",-5}{"NOMBRE",-30}{"STOCK ACTUAL",-15}");
        Console.WriteLine(new string('-', 50));
        foreach (var p in productos) {
            Console.WriteLine($"{p.Id,-5}{p.Nombre,-30}{p.Stock,15}");
        }
        
        // Solicitar ID del producto
        Console.Write("\nIngrese ID del producto: ");
        if (!int.TryParse(Console.ReadLine(), out int id)) {
            Console.WriteLine("ID inválido");
            Console.ReadKey();
            return;
        }
        
        // Solicitar cantidad a quitar
        Console.Write("Ingrese cantidad a quitar: ");
        if (!int.TryParse(Console.ReadLine(), out int cantidad) || cantidad <= 0) {
            Console.WriteLine("Cantidad inválida. Debe ser un número positivo.");
            Console.ReadKey();
            return;
        }
        
        // Realizar la operación
        var resultado = await QuitarStockAsync(id, cantidad);
        if (resultado.Id == 0) {
            Console.WriteLine("Error al quitar stock. Verifique el ID y que haya suficiente stock disponible.");
        } else {
            Console.WriteLine($"Stock actualizado correctamente. Nuevo stock: {resultado.Stock}");
        }
    }
    catch (Exception ex) {
        Console.WriteLine($"Error: {ex.Message}");
    }
    
    Console.WriteLine("\nPresione cualquier tecla para continuar...");
    Console.ReadKey();
}

// Modelo de datos
class Producto {
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public int Precio { get; set; }
    public int Stock { get; set; }
}