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

    // El payload incluye todo el historial de la conversación
    var payload = new ContentRequest(historial);
    using var httpClient = new HttpClient();
    
    try {
        // Usando PostAsJsonAsync para simplificar el envío de datos JSON
        var respuesta = await httpClient.PostAsJsonAsync(endpoint, payload, opciones);
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
    Actua como una secretaria que ayuda a llevar la agenda de contactos y citas.
    Eres muy cortes y eficas, siempre respondes a mis preguntas de manera clara y concisa.
    
    # Contactos 
        ID   Legajo Nombre y Apellido                    Teléfono        Asistencias
        01.  61203  Acevedo Costello, Juan Ignacio       (381) 388-7804   17  
        02.  61667  Acosta, Maira                        (381) 562-8162   20  
        03.  62055  Ahumada, Aiquén                      (381) 419-9202   19  
        04.  61118  Barrios, Santiago Alexis             (381) 526-8193   19  
        05.  61319  Carabajal, José Gabriel              (381) 562-7688   17  
        06.  61214  Collazos Cortez, Máximo Alberto      (381) 350-5275   17  
        07.  61141  Di Clemente, María Antonela          (381) 398-3935   15  
        08.  61730  Diaz, Antonio                        (381) 392-6461    3  
        09.  61626  Diaz Londero, Sergio Gonzalo         (381) 604-6547   16  
        10.  61271  Donelli, Gerardo Exequiel            (381) 514-3223   19  
        11.  61221  Duclós, Marcelo Ezequiel             (381) 551-4353   20  
        12.  61720  Fernández, Luciano                   (381) 586-7891    0  
        13.  62093  Frías Silva, Juan Segundo            (381) 415-8753   15  
        14.  61139  Gallo, María Matilde                 (381) 333-4836   12  
        15.  61352  García Moya, José Ignacio            (381) 638-9006   17  
        16.  61200  Gauna Serrano, Martín Javier         (381) 389-2631   19  
        17.  61624  Godoy, Alan                          (381) 574-4877   17  
        18.  61595  González Patti, Valentín             (381) 655-9195   17  
        19.  61562  Helguera, Agustina Elizabeth         (381) 694-9619   19  
        20.  61318  Herrera, Dalma Luján                 (381) 341-4968   11  
        21.  62053  Herrera Palomino, Ivam Agustín       (381) 697-0643   21  
        22.  61450  Jiménez Paz, Patricio Agustín        (387) 388-2674   19  
        23.  61627  Juárez Fernández, Lourdes Abril      (381) 647-9914   21  
        24.  61473  Lagoria García, Tomás Gustavo        (381) 357-7724   20  
        25.  61956  Leglisé, Laureano                    (261) 468-9809   19  
        26.  61679  Lobo Barrera, Mia de los Angeles     (381) 677-0639   21  
        27.  61794  Lobo Campero, Hernán Ignacio         (381) 590-6461   15    
    
    # Citas
        Martes a Jueves de 8:00 a 10:00 hs tengo clases en la Comision 3
        Lunes y Miércoles de 8:00 a 10:00 hs tengo clases en la Comision 2
        Lunes y Martes de 10:00 a 12:00 hs tengo clases en la Comision 1
        El miercoles por la tarde tengo que asistir a la conferencia sobre IA de la UTN, es de 18:00 a 20:00 hs
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
    string entradaUsuario = ReadLine();
    
    // Verificar si el usuario quiere salir
    if (string.IsNullOrWhiteSpace(entradaUsuario) || 
        entradaUsuario.Trim().Equals("salir", StringComparison.OrdinalIgnoreCase)) {
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
    WriteLine($"\nIA: {respuestaIA}");

    // Opcional: guardar toda la conversación en un archivo después de cada interacción
    File.WriteAllText("17.3.respuesta.md", ConvertirHistorialAMarkdown(historialChat));
}

// Función para convertir el historial a formato Markdown para guardar
string ConvertirHistorialAMarkdown(List<Content> historial) {
    var sb = new StringBuilder();
    sb.AppendLine("## Conversación con Gemini");
    sb.AppendLine();
    
    foreach (var mensaje in historial) {
        string autor = mensaje.Role.Equals("user", StringComparison.OrdinalIgnoreCase) ? "**Tú**" : "**IA**";
        sb.AppendLine($"{autor}: {mensaje.Parts[0].Text}");
        sb.AppendLine();
    }
    
    return sb.ToString();
}
