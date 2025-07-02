using static System.Console;

int edad;
edad = 20;

int edad2 = 30;

// Enteros con signo
sbyte sbyteExample = 127;                   //  8 bits con signo (-128 a 127)
short shortExample = 32767;                 // 16 bits con signo (-32,768 a 32,767)
int intExample     = 2147483647;            // 32 bits con signo (-2,147,483,648 a 2,147,483,647)
long longExample   = 9223372036854775807L;  // 64 bits con signo (-9,223,372,036,854,775,808 a 9,223,372,036,854,775,807)

// Enteros sin signo
byte byteExample = 255;                     //  8 bits sin signo (0 a 255)
ushort ushortExample = 65535;               // 16 bits sin signo (0 a 65,535)
uint uintExample = 4294967295U;             // 32 bits sin signo (0 a 4,294,967,295)
ulong ulongExample = 18446744073709551615UL;// 64 bits sin signo (0 a 18,446,744,073,709,551,615)

// Mostrar los valores mínimo y máximo
WriteLine($"sbyte: {sbyte.MinValue} a {sbyte.MaxValue}");
WriteLine($"short: {short.MinValue} a {short.MaxValue}");
WriteLine($"int: {int.MinValue} a {int.MaxValue}");
WriteLine($"long: {long.MinValue} a {long.MaxValue}");
WriteLine($"byte: {byte.MinValue} a {byte.MaxValue}");
WriteLine($"ushort: {ushort.MinValue} a {ushort.MaxValue}");
WriteLine($"uint: {uint.MinValue} a {uint.MaxValue}");
WriteLine($"ulong: {ulong.MinValue} a {ulong.MaxValue}");

WriteLine($"1+2={1+2}");

int a     = 1113;
long al   = 110L;
ulong aul = 150UL;
ushort s  = 100us;

var t     = 1000;

var t2    = 1000L;
var t3    = 1000UL;

float b = 1.5;

float f1   = 100.0f;
float f2   = 100.0F;
double d1  = 100.0d;
decimal d2 = 100.0m;

var x1     = 123m; 
decimal x2 = 123m;

// Operaciones aritméticas básicas
int num1 = 10;
int num2 = 3;

// Suma
int sum = num1 + num2;
WriteLine($"Suma: {num1} + {num2} = {sum}");

// Resta
int difference = num1 - num2;
WriteLine($"Resta: {num1} - {num2} = {difference}");

// Multiplicación
int product = num1 * num2;
WriteLine($"Multiplicación: {num1} * {num2} = {product}");

// División (división entera)
int quotient = num1 / num2;
WriteLine($"División entera: {num1} / {num2} = {quotient}");

// Módulo (resto)
int remainder = num1 % num2;
WriteLine($"Módulo: {num1} % {num2} = {remainder}");

// Operadores de asignación compuesta
int x = 5;
WriteLine($"Valor inicial: x = {x}");
x += 3; // Equivalente a x = x + 3
WriteLine($"Después de x += 3: x = {x}");
x -= 2; // Equivalente a x = x - 2
WriteLine($"Después de x -= 2: x = {x}");
x *= 4; // Equivalente a x = x * 4
WriteLine($"Después de x *= 4: x = {x}");
x /= 2; // Equivalente a x = x / 2
WriteLine($"Después de x /= 2: x = {x}");
x %= 3; // Equivalente a x = x % 3
WriteLine($"Después de x %= 3: x = {x}");

// Incremento y decremento
int y = 10;
WriteLine($"y = {y}");
WriteLine($"y++ = {y++}"); // Post-incremento: devuelve el valor y luego incrementa
WriteLine($"Ahora y = {y}");
WriteLine($"++y = {++y}"); // Pre-incremento: incrementa y luego devuelve el valor
WriteLine($"Ahora y = {y}");
WriteLine($"y-- = {y--}"); // Post-decremento: devuelve el valor y luego decrementa
WriteLine($"Ahora y = {y}");
WriteLine($"--y = {--y}"); // Pre-decremento: decrementa y luego devuelve el valor
WriteLine($"Ahora y = {y}");

// Operadores de comparación
WriteLine("\nOperadores de comparación:");

int valA = 5;
int valB = 10;

// Igual a
bool equalTo = valA == valB;
WriteLine($"{valA} == {valB}: {equalTo}");

// Diferente de
bool notEqualTo = valA != valB;
WriteLine($"{valA} != {valB}: {notEqualTo}");

// Mayor que
bool greaterThan = valA > valB;
WriteLine($"{valA} > {valB}: {greaterThan}");

// Menor que
bool lessThan = valA < valB;
WriteLine($"{valA} < {valB}: {lessThan}");

// Mayor o igual que
bool greaterThanOrEqual = valA >= valB;
WriteLine($"{valA} >= {valB}: {greaterThanOrEqual}");

// Menor o igual que
bool lessThanOrEqual = valA <= valB;
WriteLine($"{valA} <= {valB}: {lessThanOrEqual}");

// Comparación de igualdad de objetos usando .Equals()
string str1 = "Hello";
string str2 = "Hello";
bool strEqual = str1.Equals(str2);
WriteLine($"Igualdad de cadenas: \"{str1}\".Equals(\"{str2}\"): {strEqual}");
