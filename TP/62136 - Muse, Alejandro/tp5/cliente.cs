using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

var baseUrl = "http://localhost:5000";
var http = new HttpClient();
var jsonOpt = new JsonSerializerOptions 
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true
};

// FUNCIONES PRINCIPALES
async Task<List<Producto>> ObtenerProductosAsync()
{
    try 
    {
        Console.WriteLine("🔄 Obteniendo productos...");
        var json = await http.GetStringAsync($"{baseUrl}/productos");
        return JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt) ?? new List<Producto>();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error al obtener productos: {ex.Message}");
        Console.WriteLine("💡 Asegúrate de que el servidor esté ejecutándose");
        return new List<Producto>();
    }
}

async Task<List<Producto>> ObtenerProductosParaReponerAsync()
{
    try 
    {
        Console.WriteLine("🔄 Obteniendo productos para reponer...");
        var json = await http.GetStringAsync($"{baseUrl}/productos/reponer");
        return JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt) ?? new List<Producto>();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error al obtener productos para reponer: {ex.Message}");
        return new List<Producto>();
    }
}

async Task AgregarStockAsync(int id, int cantidad)
{
    try 
    {
        var request = new { cantidad = cantidad };
        var json = JsonSerializer.Serialize(request, jsonOpt);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var response = await http.PutAsync($"{baseUrl}/productos/{id}/agregar-stock", content);
        var responseContent = await response.Content.ReadAsStringAsync();
        
        if (response.IsSuccessStatusCode)
        {
            var result = JsonSerializer.Deserialize<JsonElement>(responseContent, jsonOpt);
            Console.WriteLine($"✅ {result.GetProperty("mensaje").GetString()}");
        }
        else
        {
            Console.WriteLine($"❌ Error: {responseContent}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error al agregar stock: {ex.Message}");
    }
}

async Task QuitarStockAsync(int id, int cantidad)
{
    try 
    {
        var request = new { cantidad = cantidad };
        var json = JsonSerializer.Serialize(request, jsonOpt);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var response = await http.PutAsync($"{baseUrl}/productos/{id}/quitar-stock", content);
        var responseContent = await response.Content.ReadAsStringAsync();
        
        if (response.IsSuccessStatusCode)
        {
            var result = JsonSerializer.Deserialize<JsonElement>(responseContent, jsonOpt);
            Console.WriteLine($"✅ {result.GetProperty("mensaje").GetString()}");
        }
        else
        {
            Console.WriteLine($"❌ Error: {responseContent}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error al quitar stock: {ex.Message}");
    }
}

void MostrarProductos(List<Producto> productos, string titulo)
{
    Console.WriteLine($"\n=== {titulo} ===");
    if (!productos.Any())
    {
        Console.WriteLine("No hay productos para mostrar.");
        return;
    }
    
    Console.WriteLine($"{"ID",-4} {"Nombre",-20} {"Precio",-15} {"Stock",-8}");
    Console.WriteLine(new string('-', 50));
    
    foreach (var p in productos)
    {
        var stockColor = p.Stock < 3 ? "⚠️" : "✅";
        Console.WriteLine($"{p.Id,-4} {p.Nombre,-20} {p.Precio,-15:C} {stockColor}{p.Stock,-8}");
    }
}

// MENÚ PRINCIPAL
Console.WriteLine("🏪 === SISTEMA DE GESTIÓN DE TIENDA ===");
Console.WriteLine("Conectando con el servidor...");

// Verificar conexión con el servidor
try 
{
    await http.GetStringAsync($"{baseUrl}/productos");
    Console.WriteLine("✅ Conexión exitosa con el servidor");
}
catch 
{
    Console.WriteLine("❌ No se puede conectar al servidor");
    Console.WriteLine("💡 Asegúrate de que el servidor esté ejecutándose en otra terminal");
    return;
}

bool continuar = true;
while (continuar)
{
    Console.WriteLine("\n📋 MENÚ PRINCIPAL:");
    Console.WriteLine("1. 📦 Listar todos los productos");
    Console.WriteLine("2. ⚠️  Productos que necesitan reposición");
    Console.WriteLine("3. ➕ Agregar stock a un producto");
    Console.WriteLine("4. ➖ Quitar stock a un producto");
    Console.WriteLine("5. 🚪 Salir");
    Console.Write("\n👉 Selecciona una opción (1-5): ");
    
    var opcion = Console.ReadLine();
    
    switch (opcion)
    {
        case "1":
            var productos = await ObtenerProductosAsync();
            MostrarProductos(productos, "TODOS LOS PRODUCTOS");
            break;
            
        case "2":
            var productosReponer = await ObtenerProductosParaReponerAsync();
            if (productosReponer.Any())
            {
                MostrarProductos(productosReponer, "PRODUCTOS PARA REPONER (Stock < 3)");
            }
            else
            {
                Console.WriteLine("\n✅ ¡Excelente! No hay productos que necesiten reposición");
            }
            break;
            
        case "3":
            // Mostrar productos primero
            var productosParaAgregar = await ObtenerProductosAsync();
            if (productosParaAgregar.Any())
            {
                MostrarProductos(productosParaAgregar, "PRODUCTOS DISPONIBLES");
                Console.Write("\n👉 Ingresa el ID del producto: ");
                if (int.TryParse(Console.ReadLine(), out int idAgregar))
                {
                    Console.Write("👉 Cantidad a agregar: ");
                    if (int.TryParse(Console.ReadLine(), out int cantidadAgregar) && cantidadAgregar > 0)
                    {
                        await AgregarStockAsync(idAgregar, cantidadAgregar);
                    }
                    else
                    {
                        Console.WriteLine("❌ Cantidad inválida (debe ser mayor a 0)");
                    }
                }
                else
                {
                    Console.WriteLine("❌ ID inválido");
                }
            }
            break;
            
        case "4":
            // Mostrar productos primero
            var productosParaQuitar = await ObtenerProductosAsync();
            if (productosParaQuitar.Any())
            {
                MostrarProductos(productosParaQuitar, "PRODUCTOS DISPONIBLES");
                Console.Write("\n👉 Ingresa el ID del producto: ");
                if (int.TryParse(Console.ReadLine(), out int idQuitar))
                {
                    Console.Write("👉 Cantidad a quitar: ");
                    if (int.TryParse(Console.ReadLine(), out int cantidadQuitar) && cantidadQuitar > 0)
                    {
                        await QuitarStockAsync(idQuitar, cantidadQuitar);
                    }
                    else
                    {
                        Console.WriteLine("❌ Cantidad inválida (debe ser mayor a 0)");
                    }
                }
                else
                {
                    Console.WriteLine("❌ ID inválido");
                }
            }
            break;
            
        case "5":
            continuar = false;
            Console.WriteLine("👋 ¡Hasta luego! Gracias por usar el sistema de gestión de tienda.");
            break;
            
        default:
            Console.WriteLine("❌ Opción inválida. Por favor selecciona una opción del 1 al 5.");
            break;
    }
    
    if (continuar)
    {
        Console.WriteLine("\n⏸️  Presiona cualquier tecla para continuar...");
        Console.ReadKey();
        Console.Clear();
    }
}

// MODELO
public class Producto 
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}