#r "nuget: Spectre.Console, 0.47.0"

using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Spectre.Console;

var baseUrl = "http://localhost:5000";
var http = new HttpClient();
var jsonOpt = new JsonSerializerOptions {
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true
};

while (true) {
    var opcion = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("[green]¿Qué querés hacer?[/]")
            .AddChoices(new[] {
                "1. Ver todos los productos",
                "2. Ver productos a reponer",
                "3. Agregar stock",
                "4. Quitar stock",
                "0. Salir"
            })
    );

    switch (opcion[0]) {
        case '0': return;
        case '1': await MostrarProductos(); break;
        case '2': await MostrarProductos("/productos/reponer"); break;
        case '3': await CambiarStock("/productos/agregar"); break;
        case '4': await CambiarStock("/productos/quitar"); break;
    }
}

async Task MostrarProductos(string ruta = "/productos") {
    var json = await http.GetStringAsync(baseUrl + ruta);
    var lista = JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt);

    var tabla = new Table();
    tabla.AddColumn("ID");
    tabla.AddColumn("Nombre");
    tabla.AddColumn("Precio");
    tabla.AddColumn("Stock");

    foreach (var p in lista!) {
        tabla.AddRow(p.Id.ToString(), p.Nombre, $"{p.Precio:c}", p.Stock.ToString());
    }

    AnsiConsole.Write(tabla);
}

async Task CambiarStock(string ruta) {
    var id = AnsiConsole.Ask<int>("[yellow]ID del producto:[/]");
    var cantidad = AnsiConsole.Ask<int>("[yellow]Cantidad:[/]");

    var cambio = new { Id = id, Cantidad = cantidad };
    var json = JsonSerializer.Serialize(cambio, jsonOpt);
    var content = new StringContent(json, Encoding.UTF8, "application/json");

    var res = await http.PostAsync(baseUrl + ruta, content);
    if (res.IsSuccessStatusCode) {
        AnsiConsole.MarkupLine("[green]✔ Operación realizada correctamente.[/]");
    } else {
        var error = await res.Content.ReadAsStringAsync();
        AnsiConsole.MarkupLine($"[red]Error:[/] {error}");
    }
}

class Producto {
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}
