using System;
using System.Collections.Generic;
using System.Text.Json;

namespace JsonConversion {
    // Clases para el primer JSON (con "candidates")
    public class Part {
        public string Text { get; set; }
    }

    public class Content {
        public List<Part> Parts { get; set; }
        public string Role { get; set; }
    }

    public class Candidates {
        public List<Candidate> Candidates { get; set; }
    }

    public class Contents {
        public List<Content> Contents { get; set; }
    }
    
    public class Candidate {
        public Content Content { get; set; }
    }




    class Program {
        static void Main(string[] args) {
            // Inicialización del primer JSON (con "candidates")
            Candidates rootWithCandidates = new Candidates {
                Candidates = new List<Candidate> {
                    new Candidate {
                        Content = new Content {
                            Parts = new List<Part> {
                                new Part { Text = "Hello" },
                                new Part { Text = "World" }
                            },
                            Role = "user"
                        }
                    }
                }
            };

            // Inicialización del segundo JSON (con "contents")
            var rootWithContents = new Contents {
                Contents = new List<Content> {
                    new Content {
                        Parts = new List<Part> {
                            new Part { Text = "Hello" },
                            new Part { Text = "World" }
                        },
                        Role = "user"
                    }
                }
            };

            // Convertir objetos a JSON
            string jsonWithCandidates = JsonSerializer.Serialize(rootWithCandidates, new JsonSerializerOptions { WriteIndented = true });
            string jsonWithContents = JsonSerializer.Serialize(rootWithContents, new JsonSerializerOptions { WriteIndented = true });

            // Mostrar los JSON generados
            Console.WriteLine("JSON con 'candidates':");
            Console.WriteLine(jsonWithCandidates);
            Console.WriteLine("\nJSON con 'contents':");
            Console.WriteLine(jsonWithContents);

            // Demostración de deserialización
            Console.WriteLine("\nDeserialization test:");
            var deserializedCandidates = JsonSerializer.Deserialize<Candidates>(jsonWithCandidates);
            Console.WriteLine($"Primer texto en 'candidates': {deserializedCandidates.Candidates[0].Content.Parts[0].Text}");
        }
    }
}