using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

public class CompletarTP7 {
    
    /// <summary>
    /// Procesa los archivos para agregar círculos verdes o rojos según la presentación de TP7
    /// </summary>
    public static void CompletarTP7() {
        try {
            // Extraer legajos del archivo calculadoras.md
            HashSet<string> legajosConTP7 = ExtraerLegajosDeCalculadoras();
            
            // Procesar el archivo RESULTADOS.md
            string[] lineasResultados = File.ReadAllLines("resultados.md");
            List<string> lineasModificadas = new List<string>();
            
            foreach (string linea in lineasResultados) {
                string lineaModificada = ProcesarLinea(linea, legajosConTP7);
                lineasModificadas.Add(lineaModificada);
            }
            
            // Guardar en el nuevo archivo
            File.WriteAllLines("RESULTADOS-TP7.md", lineasModificadas);
            
            Console.WriteLine("Archivo RESULTADOS-TP7.md generado exitosamente.");
            Console.WriteLine($"Total de legajos con TP7: {legajosConTP7.Count}");
        } catch (Exception ex) {
            Console.WriteLine($"Error al procesar archivos: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Extrae todos los legajos del archivo calculadoras.md
    /// </summary>
    /// <returns>HashSet con los legajos que presentaron TP7</returns>
    private static HashSet<string> ExtraerLegajosDeCalculadoras() {
        HashSet<string> legajos = new HashSet<string>();
        
        if (!File.Exists("calculadoras.md")) {
            throw new FileNotFoundException("No se encontró el archivo calculadoras.md");
        }
        
        string[] lineas = File.ReadAllLines("calculadoras.md");
        Regex regexLegajo = new Regex(@"^(\d{5})");
        
        foreach (string linea in lineas) {
            Match match = regexLegajo.Match(linea.Trim());
            if (match.Success) {
                legajos.Add(match.Groups[1].Value);
            }
        }
        
        return legajos;
    }
    
    /// <summary>
    /// Procesa una línea individual, agregando círculo verde o rojo según corresponda
    /// </summary>
    /// <param name="linea">Línea a procesar</param>
    /// <param name="legajosConTP7">Set de legajos que presentaron TP7</param>
    /// <returns>Línea modificada con el círculo correspondiente</returns>
    private static string ProcesarLinea(string linea, HashSet<string> legajosConTP7) {
        // Regex para detectar líneas que comienzan con número de orden y legajo
        Regex regexLineaAlumno = new Regex(@"^(\d{2}\.\s+)(\d{5})(\s+.*)$");
        Match match = regexLineaAlumno.Match(linea);
        
        if (match.Success) {
            string numeroOrden = match.Groups[1].Value;
            string legajo = match.Groups[2].Value;
            string restoLinea = match.Groups[3].Value;
            
            // Determinar el círculo a agregar
            string circulo = legajosConTP7.Contains(legajo) ? "🟢" : "🔴";
            
            // Reconstruir la línea con el círculo
            return $"{numeroOrden}{legajo}{restoLinea} {circulo}";
        }
        
        // Si no es una línea de alumno, devolver sin modificar
        return linea;
    }
    
}
