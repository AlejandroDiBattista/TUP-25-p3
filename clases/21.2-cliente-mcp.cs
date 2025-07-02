#!/usr/bin/env dotnet script
#r "sdk:Microsoft.NET.Sdk.Web"                  // top-level await  [oai_citation:13‡Stack Overflow](https://stackoverflow.com/questions/65426934/does-dotnet-script-csx-support-asp-net-core-app-development?utm_source=chatgpt.com)
#r "nuget: ModelContextProtocol, 0.2.0-preview.1"       // (o la versión que uses)

using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModelContextProtocol.Client;
using System.Linq;
using System.Text.Json;

// Lanza el servidor MCP definido en 21.servidor-mcp.csx mediante dotnet-script.
var clientTransport = new StdioClientTransport(
    new StdioClientTransportOptions {
        Name      = "MiMCP",
        Command   = "dotnet",
        Arguments = new[] { "script", "./21.1-servidor-mcp.cs" }
    });

var cliente = await McpClientFactory.CreateAsync(clientTransport);

Console.WriteLine("=== Herramientas disponibles ===");
foreach (var tool in await cliente.ListToolsAsync()) {
    var doc = tool.ProtocolTool.InputSchema;
    string parametros = "";
    if (doc.TryGetProperty("properties", out var properties)) {
        var lista = properties.EnumerateObject().Select(p => $"{p.Value.GetProperty("type")} {p.Name}");
        parametros = string.Join(", ", lista);
    }
    Console.WriteLine($"✧ {tool.Name}({parametros}) \n  '{tool.Description}'\n");
}

async Task<long> Factorial(int numero) {
    var result = await cliente.CallToolAsync("Factorial", new Dictionary<string, object> { ["numero"] = numero });
    var text   = result.Content.ElementAt(0).Text;
    return long.Parse(text);
}

async Task<List<int>> PrimerosPrimos(int n) {
    var result = await cliente.CallToolAsync("PrimerosPrimos", new Dictionary<string, object> { ["n"] = n });
    var text   = result.Content.ElementAt(0).Text;
    return JsonSerializer.Deserialize<List<int>>(text);
}

Console.WriteLine($"Resultado: Factorial(10)      = {await Factorial(10)}");
Console.WriteLine($"Resultado: PrimerosPrimos(10) = {string.Join(", ", await PrimerosPrimos(10))}");