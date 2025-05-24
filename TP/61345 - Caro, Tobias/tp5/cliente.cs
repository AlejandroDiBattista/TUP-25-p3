#r "nuget: System.Net.Http.Json, 7.0.0"
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http.Json;

var baseUrl = "http://localhost:5000";
var http = new HttpClient();
var jsonOpt = new JsonSerializerOptions {
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true
};

async Task<List<Producto>> TraerProductosAsync() {
    try {
        var json = await http.GetStringAsync($"{baseUrl}/productos");
        return JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt)!;
    }
    catch (Exception ex) {
        Console.WriteLine($"Error al obtener productos: {ex.Message}");
        return new List<Producto>();
    }
}

async Task<List<Producto>> TraerReponerAsync() {
    try {
        var json = await http.GetStringAsync($"{baseUrl}/productos/reponer");
        return JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt)!;
    }
    catch (Exception ex) {
        Console.WriteLine($"Error al obtener productos a reponer: {ex.Message}");
        return new List<Producto>();
    }
}

async Task<(bool, string)> AgregarStockAsync(int id, int cantidad) {
    try {
        var requestData = new { cantidad };
        var response = await http.PostAsJsonAsync(
            $"{baseUrl}/productos/{id}/agregar", 
            requestData, 
            jsonOpt
        );
        
        if (!response.IsSuccessStatusCode) {
            var error = await response.Content.ReadAsStringAsync();
            return (false, $"Error al agregar stock: {error}");
        }
        return (true, "Stock agregado correctamente.");
    }
    catch (Exception ex) {
        return (false, $"Error de conexión: {ex.Message}");
    }
}

async Task<(bool, string)> QuitarStockAsync(int id, int cantidad) {
    try {
        var requestData = new { cantidad };
        var response = await http.PostAsJsonAsync(
            $"{baseUrl}/productos/{id}/quitar", 
            requestData, 
            jsonOpt
        );
        
        if (!response.IsSuccessStatusCode) {
            var error = await response.Content.ReadAsStringAsync();
            return (false, $"Error al quitar stock: {error}");
        }
        return (true, "Stock quitado correctamente.");
    }
    catch (Exception ex) {
        return (false, $"Error de conexión: {ex.Message}");
    }
}

async Task MostrarProductosActualizados() {
    var productos = await TraerProductosAsync();
    if (productos.Any()) {
        Console.WriteLine("\n=== LISTA ACTUALIZADA DE PRODUCTOS ===");
        Console.WriteLine($"{"ID",-5} {"Nombre",-20} {"Precio",-12} {"Stock",-8}");
        Console.WriteLine(new string('-', 50));
        productos.ForEach(p => Console.WriteLine($"{p.Id,-5} {p.Nombre,-20} ${p.Precio,-11:F2} {p.Stock,-8}"));
    }
    else {
        Console.WriteLine("No se encontraron productos");
    }
}

Console.WriteLine("=== SISTEMA DE GESTIÓN DE STOCK ===");
Console.WriteLine("Conectando al servidor...\n");

while (true) {
    Console.WriteLine("\n=== MENÚ PRINCIPAL ===");
    Console.WriteLine("1. Listar productos | 2. Productos a reponer | 3. Agregar stock | 4. Quitar stock | 5. Salir");
    Console.Write("Seleccione una opción: ");
    var opcion = Console.ReadLine();

    switch (opcion) {
        case "1":
            Console.WriteLine("\n=== PRODUCTOS DISPONIBLES ===");
            var productos = await TraerProductosAsync();
            if (productos.Any()) {
                Console.WriteLine($"{"ID",-5} {"Nombre",-20} {"Precio",-12} {"Stock",-8}");
                Console.WriteLine(new string('-', 50));
                productos.ForEach(p => Console.WriteLine($"{p.Id,-5} {p.Nombre,-20} ${p.Precio,-11:F2} {p.Stock,-8}"));
            }
            else {
                Console.WriteLine("No hay productos disponibles.");
            }
            break;
            
        case "2":
            Console.WriteLine("\n=== PRODUCTOS A REPONER (Stock < 3) ===");
            var reponer = await TraerReponerAsync();
            if (reponer.Any()) {
                Console.WriteLine($"{"ID",-5} {"Nombre",-20} {"Precio",-12} {"Stock",-8}");
                Console.WriteLine(new string('-', 50));
                reponer.ForEach(p => Console.WriteLine($"{p.Id,-5} {p.Nombre,-20} ${p.Precio,-11:F2} {p.Stock,-8}"));
            }
            else {
                Console.WriteLine("¡Excelente! No hay productos que necesiten reposición.");
            }
            break;
            
        case "3":
            Console.Write("\nID del producto y cantidad a agregar (ej: 1 5): ");
            var inputAgregar = Console.ReadLine()?.Trim();
            
            if (!string.IsNullOrEmpty(inputAgregar)) {
                var datosAgregar = inputAgregar.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                
                if (datosAgregar.Length == 2 && 
                    int.TryParse(datosAgregar[0], out int idAgregar) && 
                    int.TryParse(datosAgregar[1], out int cantAgregar)) {
                    
                    if (cantAgregar > 0) {
                        var (success, message) = await AgregarStockAsync(idAgregar, cantAgregar);
                        Console.WriteLine($"\n{(success ? "✅" : "❌")} {message}");
                        if (success) await MostrarProductosActualizados();
                    }
                    else {
                        Console.WriteLine("\n❌ La cantidad debe ser mayor a 0.");
                    }
                }
                else {
                    Console.WriteLine("\n❌ Formato inválido. Use: ID CANTIDAD (ej: 1 5)");
                }
            }
            break;
            
        case "4":
            Console.Write("\nID del producto y cantidad a quitar (ej: 1 5): ");
            var inputQuitar = Console.ReadLine()?.Trim();
            
            if (!string.IsNullOrEmpty(inputQuitar)) {
                var datosQuitar = inputQuitar.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                
                if (datosQuitar.Length == 2 && 
                    int.TryParse(datosQuitar[0], out int idQuitar) && 
                    int.TryParse(datosQuitar[1], out int cantQuitar)) {
                    
                    if (cantQuitar > 0) {
                        var (success, message) = await QuitarStockAsync(idQuitar, cantQuitar);
                        Console.WriteLine($"\n{(success ? "✅" : "❌")} {message}");
                        if (success) await MostrarProductosActualizados();
                    }
                    else {
                        Console.WriteLine("\n❌ La cantidad debe ser mayor a 0.");
                    }
                }
                else {
                    Console.WriteLine("\n❌ Formato inválido. Use: ID CANTIDAD (ej: 1 5)");
                }
            }
            break;
            
        case "5":
            Console.WriteLine("\n¡Hasta luego!");
            http.Dispose();
            return;
            
        default:
            Console.WriteLine("\n❌ Opción inválida. Intente nuevamente.");
            break;
    }
    
    Console.WriteLine("\nPresione cualquier tecla para continuar...");
    Console.ReadKey();
    Console.Clear();
}

class Producto {
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}