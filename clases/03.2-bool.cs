using System.Runtime.InteropServices;
using static System.Console;

bool a = true;
bool b = false;
bool c = 5 < 10;
bool d = 5 > 10;

WriteLine($"a: {a}, b: {b}, c: {c}, d: {d}"); // True
WriteLine("\nOperaciones lógicas en C#:");

// Operador AND (&&)
WriteLine($"a && b: {a && b}");  // true AND false = false
WriteLine($"a && c: {a && c}");  // true AND true = true

// Operador OR (||)
WriteLine($"a || b: {a || b}");  // true OR false = true
WriteLine($"b || d: {b || d}");  // false OR false = false

// Operador NOT (!)
WriteLine($"!a: {!a}");         // NOT true = false
WriteLine($"!b: {!b}");         // NOT false = true

// Operador XOR (^)
WriteLine($"a ^ b: {a ^ b}");   // true XOR false = true
WriteLine($"a ^ c: {a ^ c}");   // true XOR true = false

// Combinación de operadores
WriteLine($"(a || b) && !d: {(a || b) && !d}");  // (true OR false) AND NOT false = true
