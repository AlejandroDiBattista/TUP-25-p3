using static System.Console;      // Console, etc.
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

while (true)
{
    Clear();
    WriteLine("|----------------------------------|");
    WriteLine("|               Menu               |");
    WriteLine("|----------------------------------|");
    WriteLine("| 1 - Listar productos             |");
    WriteLine("| 2 - Listar productos a reponer   |");
    WriteLine("| 3 - Agregar stock a un producto  |");
    WriteLine("| 4 - Quitar stock a un producto   |");
    WriteLine("| 0 - Salir                        |");
    WriteLine("|----------------------------------|");
    Write("Seleccione una opcion: ");
    var opcion = ReadLine();

    if (opcion == "1")
    {
        var productos = await Get<List<Producto>>("/productos");
        MostrarProductos(productos);
    }
    else if (opcion == "2")
    {
        var aReponer = await Get<List<Producto>>("/productos/reponer");
        MostrarProductos(aReponer);
    }
    else if (opcion == "3")
    {
        Write("ID del producto: ");
        var idA = int.Parse(ReadLine()!);
        Write("Cantidad a agregar: ");
        var cantA = int.Parse(ReadLine()!);
        await Post($"/productos/{idA}/agregar?cantidad={cantA}");
    }
    else if (opcion == "4")
    {
        Write("ID del producto: ");
        var idQ = int.Parse(ReadLine()!);
        Write("Cantidad a quitar: ");
        var cantQ = int.Parse(ReadLine()!);
        await Post($"/productos/{idQ}/quitar?cantidad={cantQ}");
    }
    else if (opcion == "0")
    {
        return;
    }
    else
    {
        WriteLine("Opcion no valida");
    }
}

async Task<T> Get<T>(string url)
{
    var json = await http.GetStringAsync(baseUrl + url);
    return JsonSerializer.Deserialize<T>(json, jsonOpt)!;
}

async Task Post(string url)
{
    var response = await http.PostAsync(baseUrl + url, new StringContent(""));
    if (response.IsSuccessStatusCode)
    {
        WriteLine("Operacion realizada con exito");
        ReadKey();
    }
    else
    {
        var error = await response.Content.ReadAsStringAsync();
        WriteLine($"Error: {response.StatusCode} - {error}");
    }
}

void MostrarProductos(IEnumerable<Producto> productos)
{
    Clear();
    WriteLine("|----|----------------------|------------|-------|");
    WriteLine("| ID | Nombre               | Precio     | Stock |");
    WriteLine("|----|----------------------|------------|-------|");
    foreach (var p in productos)
    {
        WriteLine($"| {p.Id,2} | {p.Nombre,-20} | {p.Precio,10:c} | {p.Stock,5} |");
    }
    WriteLine("|----|----------------------|------------|-------|");
    ReadKey();
}

class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}