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
        Console.WriteLine($"error al obtener productos: {ex.Message}");
        return new List<Producto>();
    }
}

async Task<List<Producto>> TraerReponerAsync() {
    try {
        var json = await http.GetStringAsync($"{baseUrl}/productos/reponer");
        return JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt)!;
    }
    catch (Exception ex) {
        Console.WriteLine($"error al obtener productos a reponer: {ex.Message}");
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
            return (false, $"error al agregar stock: {error}");
        }
        return (true, "stock agregado");
    }
    catch (Exception ex) {
        return (false, $"error de conexion: {ex.Message}");
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
            return (false, $"error al quitar stock: {error}");
        }
        return (true, "stock quitado");
    }
    catch (Exception ex) {
        return (false, $"error de conexion: {ex.Message}");
    }
}

async Task MostrarProductosActualizados() {
    var productos = await TraerProductosAsync();
    if (productos.Any()) {
        Console.WriteLine("\n=== LISTA ACTUALIZADA DE PRODUCTOS ===");
        productos.ForEach(p => Console.WriteLine($"{p.Id}: {p.Nombre} - Stock: {p.Stock}"));
    }
    else {
        Console.WriteLine("productos no encontrados");
    }
}

while (true) {
    Console.WriteLine("\n1. Listar productos | 2. Productos a reponer | 3. Agregar stock | 4. Quitar stock | 5. Salir");
    Console.Write("Seleccione una opcion: ");
    var opcion = Console.ReadLine();

    switch (opcion) {
        case "1":
            var productos = await TraerProductosAsync();
            if (productos.Any()) {
                Console.WriteLine("\n=== PRODUCTOS DISPONIBLES ===");
                productos.ForEach(p => Console.WriteLine($"{p.Id}: {p.Nombre} - Stock: {p.Stock}"));
            }
            break;
            
        case "2":
            var reponer = await TraerReponerAsync();
            if (reponer.Any()) {
                Console.WriteLine("\n=== PRODUCTOS A REPONER (Stock < 3) ===");
                reponer.ForEach(p => Console.WriteLine($"{p.Id}: {p.Nombre} - Stock: {p.Stock}"));
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
                }
            }
            break;
            
        case "5":
            return;
    }
}

class Producto {
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}