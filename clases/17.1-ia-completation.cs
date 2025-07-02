using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;

// Clases para manejar la comunicación con la API de Gemini
public record MessageResponse(List<Candidate> Candidates);
public record ContentRequest(List<Content> Contents);
public record Candidate(Content Content);
public record Content(List<Part> Parts, string Role);
public record Part(string Text);

var opciones = new JsonSerializerOptions { 
    PropertyNameCaseInsensitive = true,                 // Para la deserialización 
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase   // Para la serialización (convierte a camelCase)
};

string ApiKey   = "AIzaSyDNb3IWwI5tjnGXj4EFAsih0HJww1HgN7M";
string Modelo   = "gemini-2.5-flash-preview-04-17";
string endpoint = $"https://generativelanguage.googleapis.com/v1beta/models/{Modelo}:generateContent?key={ApiKey}";

async Task<string> Completar(string indicacion) {

    // El carga debe tener la estructura esperada por la API
    var cargaUtil = new ContentRequest(
        new List<Content> {
            new Content( new List<Part> { 
                new Part(indicacion) }, 
                "user")
        }
    );

    using var httpClient = new HttpClient();
    
    try {
        // Usando PostAsJsonAsync para simplificar el envío de datos JSON
        var respuesta = await httpClient.PostAsJsonAsync(endpoint, cargaUtil, opciones);
        respuesta.EnsureSuccessStatusCode();
        
        // Deserializar directamente la respuesta JSON
        var resultado = await respuesta.Content.ReadFromJsonAsync<MessageResponse>(opciones);
        return resultado.Candidates[0].Content.Parts[0].Text;
    } catch (Exception e) {
        return $"Error: {e.Message}";
    }
}

var respuesta = await Completar("""
Creme una plantilla con el siguiente documento  

---
    Por la presente se certifica que el 
    Sr. Alejandro Di Battista ha realizado 
    un pago de $300.000 (trescientos mil pesos) 
    para cancelar su deuda correspondiente 
    a la compra de un televisor. 
    
    Fecha: 17 de abril de 2024. 

    --- 
    EN la platilla remplaza los datos por varieble entre <>
""");

Console.WriteLine(respuesta);
File.WriteAllText("17.1.respuesta.md", respuesta);
