using System.Runtime.InteropServices;
using static System.Console;


// Characteres

char letra = 'A';
char letra2 = 'a';

char.IsDigit(letra); // false
char.IsLetter(letra); // true
char.IsWhiteSpace(letra); // false
char.IsUpper(letra); // true
char.IsLower(letra); // false
char.IsPunctuation(letra); // false
char.IsSymbol(letra); // false
char.IsControl(letra); // false
// Carácter en ruso (cirílico)
char letraRusa = 'Ж'; // 'Zh' en ruso cirílico

// Carácter en griego
char letraGriega = 'Ω'; // Omega

// Carácter en latín
char letraLatin = 'æ'; // ae ligature
letraRusa = char.ToUpper(letraRusa);
WriteLine($"Ruso: {letraRusa}, Griego: {letraGriega}, Latín: {letraLatin}");


// string texto = "Hola, mundo!";
// WriteLine(texto); 
// Console.Clear();
// var nombre = "Juan";
// var apellido = "Pérez";
// var nombreCompleto = apellido + ", " + nombre;
// nombreCompleto = $"{apellido}, {nombre}"; // Interpolación de cadenas

// WriteLine($"Nombre completo: {nombreCompleto}"); // Pérez, Juan

// var n1 = 2;
// var n2 = 3;

// WriteLine($"[{n1:D3}] + {n2,3} = {n1+n2,3}"); // Suma: 5
// // Principales formatos de interpolación de strings
// WriteLine("\n===== Formatos de interpolación de strings =====");

// // D: Formato decimal con dígitos específicos
// WriteLine($"Formato D (decimal): {n1:D4}");  // 0002

// // C: Formato de moneda
// decimal precio = 1234.56m;
// WriteLine($"Formato C (moneda): {precio:C}");  // $1,234.56 (depende de la cultura)

// // N: Formato numérico con separadores de miles
// WriteLine($"Formato N (numérico): {precio:N}");  // 1,234.56

// // P: Formato de porcentaje
// double porcentaje = 0.1234;
// WriteLine($"Formato P (porcentaje): {porcentaje:P}");  // 12.34%

// // E: Formato científico/exponencial
// double cientifico = 12345.6789;
// WriteLine($"Formato E (exponencial): {cientifico:E}");  // 1.234568E+004

// // F: Formato de punto fijo con decimales específicos
// WriteLine($"Formato F (punto fijo): {cientifico:F2}");  // 12345.68

// // X: Formato hexadecimal
// int hex = 255;
// WriteLine($"Formato X (hexadecimal): {hex:X}");  // FF

// // Alineación y relleno
// WriteLine($"Alineación derecha: {n1,5}");  // '    2'
// WriteLine($"Alineación izquierda: {n1,-5}|");  // '2    |'

// // Formato personalizado para fechas
// DateTime ahora = DateTime.Now;
// WriteLine($"Fecha corta: {ahora:d}");  // MM/dd/yyyy
// WriteLine($"Fecha larga: {ahora:D}");  // dddd, MMMM dd, yyyy
// WriteLine($"Hora: {ahora:t}");  // HH:mm
// WriteLine($"Personalizado: {ahora:yyyy-MM-dd HH:mm:ss}");  // 2023-05-25 14:30:45

// var s2 = n2.ToString("D3"); // Convierte el número a string con formato D3
// WriteLine($"Número como string: {s2}");  // "3"

// Clear();

// WriteLine($"Cantidad de caracteres: {nombreCompleto.Length}"); // 12
// // ToLower() - convierte el texto a minúsculas
// string textoOriginal = "   ESTE ES UN EJEMPLO DE TEXTO   ";
// string textoMinusculas = textoOriginal.ToLower();
// WriteLine($"ToLower(): {textoMinusculas}");

// // Trim() - elimina espacios en blanco al inicio y final
// string textoSinEspacios = textoOriginal.Trim();
// WriteLine($"Trim(): |{textoSinEspacios}|");

// // Combinación de métodos
// string textoLimpio = textoOriginal.Trim().ToLower();
// WriteLine($"Trim() y ToLower(): {textoLimpio}");

// // Substring() - extrae parte de una cadena
// string mensaje = "Hola Mundo C#";
// // Extraer desde el índice 5, 5 caracteres
// string subcadena1 = mensaje.Substring(5, 5);
// WriteLine($"Substring(5, 5): |{subcadena1}|");

// // Extraer desde el índice 10 hasta el final
// string subcadena2 = mensaje.Substring(10);
// WriteLine($"Substring(10): |{subcadena2}|");

// var l1 = mensaje[0]; // Acceso a un carácter específico
// WriteLine($"Primer carácter: {l1}"); // H

// var raro = "Hola 😄";
// var c1 = raro[5]; // Acceso a un carácter específico
// WriteLine($"Quinto carácter raro: {c1}"); // H
// WriteLine(raro);


int[] numeros = { 1, 2, 3, 4, 5 };
var n0 = numeros[0];
var n1 = numeros[1];
var n2 = numeros[2];

var raro2 = numeros;

numeros[1] = 1000;
raro2[2] = 2000;
// Mostrar el vector de números por consola
WriteLine("\nVector de números:");
foreach (var numero in numeros)
{
    Console.Write($"{numero} ");
}
WriteLine();

foreach (var numero in raro2)
{
    Console.Write($"{numero} ");
}
WriteLine();

