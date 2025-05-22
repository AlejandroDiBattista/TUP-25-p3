using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;

string ApiKey   = "AIzaSyDNb3IWwI5tjnGXj4EFAsih0HJww1HgN7M";
string Modelo   = "gemini-2.5-flash-preview-04-17";
string endpoint = $"https://generativelanguage.googleapis.com/v1beta/models/{Modelo}:generateContent?key={ApiKey}";

// Clases para manejar la comunicación con la API de Gemini
public record MessageResponse(List<Candidate> Candidates);
public record ContentRequest(List<Content> Contents);
public record Candidate(Content Content);
public record Content(List<Part> Parts, string Role);
public record Part(string Text);

// Crear un historial vacío para la conversación
var historialChat = new List<Content>();

var opciones = new JsonSerializerOptions { 
    PropertyNameCaseInsensitive = true,  // Para la deserialización 
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase  // Para la serialización (convierte a camelCase)
};

// Función que envía el historial completo y obtiene la respuesta de Gemini
async Task<string> CompletarChat(List<Content> historial) {
    // El cargaUtil incluye todo el historial de la conversación
    var cargaUtil = new ContentRequest(historial);
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

// Añadir mensaje del usuario al historial
void AgregarMensajeUsuario(string mensaje) {
    historialChat.Add(new Content(new List<Part> { new Part(mensaje) }, "user"));
}

// Añadir respuesta de la IA al historial
void AgregarRespuestaIA(string respuesta) {
    historialChat.Add(new Content(new List<Part> { new Part(respuesta) }, "model"));
}

// Establecer un mensaje inicial para dar contexto a la conversación
AgregarMensajeUsuario("""
    Compórtate como un profesor que está enseñando Sharp a un grupo de alumnos principiantes
    Explicaciones deben ser breves claras y apuntada que se entienda como se consigue esta soluciones en los próximos casos que use

    Si te pide que desarrollen un algoritmo hacer la versión más sencilla de entender no le pongas comentario pero si explica la lógica del algoritmo en forma breve al final
    
""");

// Obtener y guardar la respuesta inicial
var respuestaInicial = await CompletarChat(historialChat);
AgregarRespuestaIA(respuestaInicial);

// Mostrar información inicial para el usuario
WriteLine("=== Chat con Gemini ===");
WriteLine("(Escribe 'salir' para terminar la conversación)");
WriteLine();
WriteLine($"IA: {respuestaInicial}");

// Bucle principal del chat
while (true) {
    WriteLine();
    Write("Tú: ");
    string entradaUsuario = ReadLine() ?? "";
    
    // Verificar si el usuario quiere salir
    if (entradaUsuario == "salir"){
        WriteLine("Finalizando el chat. ¡Hasta pronto!");
        break;
    }
    
    // Agregar la entrada del usuario al historial
    AgregarMensajeUsuario(entradaUsuario);
    
    // Obtener la respuesta de la IA
    var respuestaIA = await CompletarChat(historialChat);
    if (respuestaIA.Trim().StartsWith("```")) respuestaIA = "\n" + respuestaIA.Trim();
    
    // Agregar la respuesta al historial
    AgregarRespuestaIA(respuestaIA);
    
    // Mostrar la respuesta
    WriteLine($"\nIA:\n {respuestaIA}");

    // Opcional: guardar toda la conversación en un archivo después de cada interacción
    File.WriteAllText("17.2.respuesta.md", ConvertirHistorialAMarkdown(historialChat));
}

// Función para convertir el historial a formato Markdown para guardar
string ConvertirHistorialAMarkdown(List<Content> historial) {
    var sb = new StringBuilder();
    sb.AppendLine("## Conversación con Gemini");
    sb.AppendLine();
    
    foreach (var mensaje in historial.Skip(1)) { // Saltar el primer mensaje (instrucciones)
        string autor = mensaje.Role.Equals("user", StringComparison.OrdinalIgnoreCase) ? "**Tú**" : "**IA**";
        sb.AppendLine($"{autor}: {mensaje.Parts[0].Text}");
        sb.AppendLine();
    }
    
    return sb.ToString();
}
