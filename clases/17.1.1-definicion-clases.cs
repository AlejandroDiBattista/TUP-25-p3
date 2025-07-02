// === Formato de entrada ===
//{ 
//  "contents": [{
//      "parts": [{ 
//          "text": "string" 
//      }],
//      "role": "user"
//  }]
//}

// === Formato de salida ===
//{
//  "candidates": [
//     { 
//          "content": { 
//              "parts": [{ "text": "string"}],
//              "role": "assistant"
//          }
//      }
//   ]
// }

// DTO: Data Transfer Object -> 
//  Objeto de Transferencia de Datos -> 
//  Se utiliza para transferir datos entre sistemas o capas de una aplicación.
//  En este caso, se utiliza para transferir datos entre la aplicación y la API de Gemini.
//
public record MessageResponse(List<Candidate> Candidates);
public record ContentRequest(List<Content> Contents);
public record Candidate(Content Content);
public record Content(List<Part> Parts, string Role);
public record Part(string Text);

// public class MessageResponse{
//     public List<Candidate> Candidates { get; set; }
//     public MessageResponse(List<Candidate> candidates) {
//         Candidates = candidates;
//     }
// }

// public class ContentRequest{
//     public List<Content> Contents { get; set; }
//     public ContentRequest(List<Content> contents) {
//         Contents = contents;
//     }
// }
// public class Candidate{
//     public Content Content { get; set; }
//     public Candidate(Content content) {
//         Content = content;
//     }
// }

