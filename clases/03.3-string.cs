string nombre = "Juan";
string apellido = "Pérez";
string nc = apellido + ", " + nombre; 

// Concatenación de cadenas
string nombreCompleto = $"{apellido}, {nombre}"; // Interpolación de cadenas

string saludo = $"Hola {nombreCompleto}";
Console.WriteLine($"Nombre completo: {nombreCompleto}"); // Pérez, Juan
Console.WriteLine(saludo); // Hola Juan
var n1 = 1;
var n2 = 20;
var suma = n1 + n2;
Console.WriteLine($"La suma de {n1:D2} + {n2:D2} es: {suma:D2}"); // La suma de 10.00 + 20.00 es: 30.00

var s1 = n1.ToString("D3");
Console.WriteLine($"El valor de n1 como cadena es: {s1}");


Console.WriteLine("\n--- Operadores de cadenas ---");

// Operador de concatenación (+)
string str1 = "Hello";
string str2 = "World";
string combinado = str1 + " " + str2;
Console.WriteLine($"Concatenación (+): {combinado}");

// Igualdad de cadenas (==)
string str4 = "test";
string s2 = "test";
string s3 = "Test";
Console.WriteLine($"Igualdad (==): str4 == s2: {str4 == s2}");  // true
Console.WriteLine($"Igualdad (==): str4 == s3: {str4 == s3}");  // false

// Desigualdad de cadenas (!=)
Console.WriteLine($"Desigualdad (!=): str4 != s3: {str4 != s3}");  // true

// Operadores de comparación de cadenas (string.Compare)
Console.WriteLine($"Comparar: str4.CompareTo(s2): {str4.CompareTo(s2)}");  // 0 (igual)
Console.WriteLine($"Comparar: str4.CompareTo(s3): {str4.CompareTo(s3)}");  // distinto de cero (diferente)

// Operador de índice ([])
Console.WriteLine($"Índice ([]): str1[1]: {str1[1]}");  // 'e'

// Propiedad Length
Console.WriteLine($"Longitud: str1.Length: {str1.Length}");  // 5

// Operador Contains (método)
Console.WriteLine($"Contiene: str1.Contains(\"el\"): {str1.Contains("el")}");  // true

var carita = "Hola 😄";

Console.WriteLine($"carita[1]: {carita[1]}"); // HOla 😄
Console.WriteLine($"carita[5]: {carita[5]}"); // HOla 😄

var linea ="545455,Perez,Juan";
var partes = linea.Split(",");

Console.WriteLine($"partes: {linea}"); // 545455,Perez,Juan
Console.WriteLine($"partes: {partes[0]}"); // 545455
Console.WriteLine($"partes: {partes[1]}"); // Perez
Console.WriteLine($"partes: {partes[2]}"); // Juan

linea
    .ToLower()
    .Trim()
    .Substring(0, 4)
    .Split(',')
    .ToList()
    .ForEach(x => Console.WriteLine($"x: {x}")); // x: 545455,Perez,Juan


// // Arreglo con los días de la semana
string[] diasDeLaSemana = { "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado", "Domingo" };

// Imprimir todos los días
Console.WriteLine("Días de la semana:");
foreach (string dia in diasDeLaSemana)
{
    Console.WriteLine(dia);
}

Console.WriteLine($"Días de la semana: {string.Join("-", diasDeLaSemana)}");


var num = new string[] { "uno", "dos", "tres" };
string[] otra = num;

num[0] = "xxxx";

var x1 = num[0];
var x2 = num[1];
var x3 = otra[0];

Console.WriteLine($"x1: {x1}"); 
Console.WriteLine($"x2: {x2}"); 
Console.WriteLine($"x3: {x3}");
