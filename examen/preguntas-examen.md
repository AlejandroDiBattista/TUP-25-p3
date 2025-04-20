# Preguntas para el 1er Parcial

### 001 6682

¿Cuál es el rango de valores que puede almacenar una variable de tipo `byte` en C#?

a) -128 a 127
b) 0 a 255
c) -255 a 255

---

### 002 6299

¿Qué tipo entero utilizarías para almacenar un valor de 4.000 millones (4,000,000,000)?

a) `int`
b) `short`
c) `long`

---

### 003 2605

¿Qué resultado se obtiene al ejecutar la expresión `5 + 2 * 3`?

a) 21
b) 11
c) 17

---

### 004 1002

¿Cuál de los siguientes tipos ocupa **menos espacio en memoria**?

a) `long`
b) `short`
c) `int`

---

### 005 4637

¿Cuál es el valor final de `x` después de esta operación?
```csharp
int x = 10;
x = x / 2 + 3;
```

a) 8
b) 10
c) 5

---

### 006 2884

¿Qué operador tiene mayor precedencia en C#?

a) `+` (suma)
b) `/` (división)
c) `=` (asignación)

---

### 007 6885

¿Qué ocurre si intentás asignar un valor fuera del rango al tipo `short`? (Ej: `short x = 40000;`)

a) El programa compila, pero da un resultado incorrecto en tiempo de ejecución
b) El compilador muestra un error
c) El valor se ajusta automáticamente al rango del tipo

---

### 008 3307

¿Cuál es el tamaño en bits de un `int` en C#?

a) 16 bits
b) 32 bits
c) 64 bits

---

### 009 4738

Dado el siguiente código, ¿cuál es el valor final de `a`?
```csharp
int a = 2 + 3 * 4 - 1;
```

a) 13
b) 19
c) 21

---

### 010 3562

¿Cuál es el valor de `x` al ejecutar el siguiente código?
```csharp
int x = 10;
x += 5 * 2;
```

a) 20
b) 25
c) 30

---

### 011 4725

¿Cuál es el resultado de `10 % 3`?

a) 3
b) 1
c) 0

---

### 012 1315

¿Cuál de estas expresiones compara si dos valores son diferentes?

a) `a = b`
b) `a == b`
c) `a != b`

---

### 013 3368

¿Qué valor devuelve esta expresión lógica? `5 > 2 && 3 < 1`

a) `true`
b) `false`
c) Da error de compilación

---

### 014 1951

¿Qué operador se utiliza para obtener el resto de una división?

a) `/`
b) `%`
c) `\`

---

### 015 3166

¿Cuál es el resultado de `8 / 3` en C# (ambos operandos enteros)?

a) 2
b) 2.66
c) 3

---

### 016 1243

¿Cuál es el valor final de `x` en esta operación?
```csharp
int x = 7;
x = x % 4;
```

a) 1
b) 3
c) 0

---

### 017 1325

¿Qué expresión devuelve `true` si `a = 10` y `b = 5`?

a) `a < b`
b) `a == b`
c) `a > b`

---

### 018 2768

¿Cuál es el resultado de `!(4 > 2)`?

a) `true`
b) `false`
c) Da error

---

### 019 8237

¿Qué ocurre si hacés `int x = 5 / 0;`?

a) Se lanza una excepción en tiempo de ejecución
b) El resultado es `Infinity`
c) El valor de `x` es 0

---

### 020 8180

¿Qué valor devuelve la siguiente expresión?
```csharp
int a = 6;
bool resultado = a % 2 == 0;
```

a) `true`
b) `false`
c) Error de compilación

---

### 021 3929

¿Cuál de los siguientes literales representa un valor `long` en C#?

a) `1000000`
b) `1000000L`
c) `L1000000`

---

### 022 2310

¿Qué literal es correcto para asignar a una variable `uint`?

a) `42U`
b) `42u`
c) Ambas son correctas

---

### 023 8958

¿Cuál de los siguientes literales es un valor hexadecimal válido en C#?

a) `0x1A3F`
b) `#1A3F`
c) `1A3Fh`

---

### 024 5086

¿Cuál de los siguientes literales representa un valor binario válido en C#?

a) `0b1010`
b) `b1010`
c) `1010b`

---

### 025 1104

¿Qué ocurre si intentás asignar un literal fuera del rango de un tipo?
```csharp
byte x = 300;
```

a) El valor se ajusta automáticamente al máximo de `byte`
b) El compilador muestra un error
c) El programa se ejecuta pero da un resultado incorrecto

---

### 026 6525

¿Cuál de estos tipos flotantes tiene **mayor precisión** en C#?

a) `float`
b) `double`
c) `decimal`

---

### 027 2419

¿Cuál es el sufijo correcto para declarar un literal de tipo `float`?

a) `f`
b) `m`
c) `d`

---

### 028 4113

¿Cuál es el tipo de dato por defecto para un número con punto decimal como `3.14`?

a) `float`
b) `decimal`
c) `double`

---

### 029 3030

¿Cuál de los siguientes literales es correcto para asignar a una variable `decimal`?

a) `12.5d`
b) `12.5f`
c) `12.5m`

---

### 030 7125

¿Qué tipo de dato usarías para cálculos financieros donde la precisión es más importante que la velocidad?

a) `float`
b) `double`
c) `decimal`

---

### 031 7521

¿Cuál es el resultado de esta operación con tipos `float`?
```csharp
float x = 5f / 2f;
```

a) `2`
b) `2.5`
c) `2.0`

---

### 032 9421

¿Cuál es el tamaño en bits de una variable `double` en C#?

a) 32 bits
b) 64 bits
c) 128 bits

---

### 033 4977

¿Qué ocurre si intentás asignar un literal decimal a un `float` sin el sufijo correspondiente?

a) El compilador lanza un error
b) El compilador convierte automáticamente
c) Se pierde la parte decimal

---

### 034 4098

¿Cuál es el resultado de esta comparación en C#?
```csharp
float a = 0.1f + 0.2f;
bool resultado = a == 0.3f;
```

a) `true`
b) `false`
c) Error de compilación

---

### 035 6129

¿Qué tipo deberías usar si querés representar valores grandes con decimales pero sin errores de precisión en sumas o restas?

a) `float`
b) `decimal`
c) `double`

---

### 036 1974

¿Qué tipo de dato devuelve la expresión `5 > 3`?

a) `int`
b) `bool`
c) `double`

---

### 037 3719

¿Cuál es el valor de `resultado` en el siguiente código?
```csharp
bool resultado = !(true && false);
```

a) `true`
b) `false`
c) Da error de compilación

---

### 038 2263

¿Cuál de estas expresiones devuelve `true`?

a) `5 == 6`
b) `10 <= 9`
c) `7 != 3`

---

### 039 2662

¿Qué operador lógico representa la conjunción (AND lógico)?

a) `||`
b) `&&`
c) `!`

---

### 040 6476

¿Cuál es el valor de esta expresión booleana?
```csharp
true || false && false
```

a) `true`
b) `false`
c) Da error de sintaxis

---

### 041 3282

¿Qué valor tiene `resultado` en el siguiente código?
```csharp
bool resultado = 4 > 2 && 3 == 3;
```

a) `true`
b) `false`
c) Da error

---

### 042 9857

¿Qué operador invierte el valor de un booleano?

a) `~`
b) `!`
c) `^`

---

### 043 1261

¿Qué se imprime en consola?
```csharp
bool x = true;
bool y = false;
Console.WriteLine(x && y);
```

a) `true`
b) `false`
c) `x && y`

---

### 044 0984

¿Qué valor tiene esta expresión?
```csharp
!(5 > 2 || 3 < 1)
```

a) `true`
b) `false`
c) Da error de compilación

---

### 045 2104

¿Cuál es la precedencia correcta entre operadores en C#? (de mayor a menor)

a) Aritméticos > Comparación > Lógicos
b) Comparación > Aritméticos > Lógicos
c) Lógicos > Aritméticos > Comparación

---

### 046 3863

¿Qué ocurre si escribís este código?
```csharp
bool x = 5 + 2;
```

a) Compila y `x` vale `true`
b) Error: no se puede convertir `int` a `bool`
c) `x` vale `7`

---

### 047 5183

¿Cuál es el resultado de esta expresión?
```csharp
5 + 2 > 6 && 4 < 2
```

a) `true`
b) `false`
c) Error de compilación

---

### 048 6523

¿Qué valor tiene `resultado`?
```csharp
bool resultado = 3 * 2 == 6;
```

a) `true`
b) `false`
c) Error de tipo

---

### 049 5957

¿Qué se imprime al ejecutar este código?
```csharp
bool a = true;
bool b = false;
Console.WriteLine(a || b && !a);
```

a) `true`
b) `false`
c) Error de compilación

---

### 050 4062

¿Cuál de estas comparaciones es válida en C# y devuelve un `bool`?

a) `5 + 3`
b) `7 > 4`
c) `!3`

---

### 051 8179

¿Qué valor tiene esta expresión?
```csharp
(5 > 3) && (8 < 10)
```

a) `true`
b) `false`
c) Error de tipo

---

### 052 9306

¿Cuál es el resultado de la siguiente condición?
```csharp
(3 + 2 == 5) || (4 * 2 < 5)
```

a) `true`
b) `false`
c) `5`

---

### 053 5099

¿Qué tipo de dato puede usarse directamente en una condición `if` en C#?

a) Solo `bool`
b) Cualquier número distinto de 0
c) `int` y `bool`

---

### 054 5391

¿Qué operador lógico se evalúa **de izquierda a derecha y se detiene si ya sabe el resultado**?

a) `|`
b) `||`
c) `^`

---

### 055 3958

¿Cuál es la salida del siguiente código?
```csharp
bool a = true;
bool b = false;
bool resultado = !(a && b) || (a && !b);
Console.WriteLine(resultado);
```

a) `true`
b) `false`
c) Error de compilación

---

### 056 3321

¿Qué hace el prefijo `$` en un string como `$"Hola {nombre}"`?

a) Escapa comillas dentro del string
b) Permite interpolar variables dentro del texto
c) Concatena automáticamente cadenas

---

### 057 0207

¿Qué hace el prefijo `@` en un string como `@"C:\Archivos\Texto.txt"`?

a) Convierte la ruta en un string de bytes
b) Permite escribir cadenas multilínea
c) Interpreta el string literalmente (sin caracteres de escape)

---

### 058 7258

¿Cuál es la forma correcta de acceder al primer carácter de una cadena `nombre`?

a) `nombre(0)`
b) `nombre[0]`
c) `nombre.charAt(0)`

---

### 059 7681

¿Qué valor tiene `mensaje.Length` si `mensaje = "Hola"`?

a) 3
b) 4
c) 5

---

### 060 3678

¿Cuál es el resultado de `string.Concat("Hola", " ", "Mundo")`?

a) `"Hola Mundo"`
b) `"HolaMundo"`
c) `"Hola Mundo "` (con espacio al final)

---

### 061 4202

¿Cuál de las siguientes expresiones devuelve `true` si `a = "casa"` y `b = "CASA"`?

a) `a == b`
b) `a.Equals(b)`
c) `a.Equals(b, StringComparison.OrdinalIgnoreCase)`

---

### 062 8071

¿Qué hace `string.Join("-", new[] { "uno", "dos", "tres" })`?

a) `"uno-dos-tres"`
b) `"uno dos tres"`
c) `["uno", "dos", "tres"]`

---

### 063 6128

¿Qué resultado tiene `texto.ToUpper()` si `texto = "hola"`?

a) `"HOLA"`
b) `"hola"`
c) `"Hola"`

---

### 064 5019

¿Qué devuelve `"123".Substring(1, 1)`?

a) `"1"`
b) `"2"`
c) `"23"`

---

### 065 2031

¿Cuál de estos métodos convierte un número decimal a string con dos decimales?

a) `ToString("F2")`
b) `Format("0.00")`
c) `Convert.ToString(2.5, "F2")`

---

### 066 6775

¿Qué resultado da `$"El total es {total:C}"` si `total = 25.5` y cultura en español?

a) `"El total es $25.50"`
b) `"El total es 25,50"`
c) `"El total es $ 25,50"`

---

### 067 0516

¿Qué operador se usa para comparar si dos cadenas son iguales en contenido?

a) `=`
b) `==`
c) `===`

---

### 068 0832

¿Qué hace `string.IsNullOrEmpty(texto)`?

a) Verifica si el texto es `null`, vacío o contiene solo espacios
b) Verifica si el texto es `null` o una cadena vacía
c) Lanza excepción si el texto es `null`

---

### 069 7543

¿Cuál es el resultado de `string.Format("Valor: {0:F1}", 23.456)`?

a) `"Valor: 23.4"`
b) `"Valor: 23.5"`
c) `"Valor: 23.456"`

---

### 070 0085

¿Qué devuelve `"Hola Mundo".Contains("Mun")`?

a) `true`
b) `false`
c) `"Mun"`

---

### 071 2971

¿Qué hace `texto.Trim()` si `texto = "  Hola  "`?

a) Elimina espacios al principio
b) Elimina espacios al final
c) Elimina espacios al inicio y al final

---

### 072 0933

¿Cuál de estas formas crea una cadena multilínea válida?

a) `string s = @"Línea 1\nLínea 2";`
b) `string s = "Línea 1\r\nLínea 2";`
c) Ambas son válidas

---

### 073 2507

¿Qué hace `"abc".PadLeft(5, '*')`?

a) `"**abc"`
b) `"abc**"`
c) `"abc"`

---

### 074 6919

¿Qué resultado tiene esta expresión?
```csharp
"csharp"[2]
```

a) `'s'`
b) `'a'`
c) `'h'`

---

### 075 6194

¿Qué devuelve `"123".Replace("2", "9")`?

a) `"193"`
b) `"129"`
c) `"1932"`

---

### 076 5759

¿Cuál de las siguientes opciones representa correctamente una variable de tipo `char` en C#?  

a) `char letra = "A";`
b) `char letra = 'A';`
c) `char letra = A;`

---

### 077 0946

¿Cuál es la principal diferencia entre `char` y `string` en C#?  

a) `char` puede almacenar varios caracteres, mientras que `string` solo uno.
b) `string` es un valor primitivo y `char` es una clase.
c) `char` representa un único carácter, mientras que `string` puede contener una secuencia de caracteres.

---

### 078 7156

¿Qué instrucción convierte correctamente un `char` en un `string`?  

a) `char letra = 'X'; string texto = letra.ToString();`
b) `char letra = 'X'; string texto = (string)letra;`
c) `char letra = 'X'; string texto = letra + letra;`

---

### 079 8697

¿Cuál de las siguientes expresiones evalúa si una variable `char` llamada `letra` es igual al carácter 'a'?  

a) `if (letra == "a")`
b) `if (letra == 'a')`
c) `if (letra.Equals("a"))`

---

### 080 4246

¿Qué sucede si intentamos asignar una cadena de texto a una variable `char`?

a) El código compila, pero da error en tiempo de ejecución.
b) Se produce un error de compilación, ya que `char` solo puede contener un carácter.
c) El compilador convierte automáticamente la cadena a un solo carácter.

---

### 081 6043

¿Cuál de las siguientes opciones representa correctamente una variable de tipo `char` en C#?  

a) `char letra = "A";`
b) `char letra = 'A';`
c) `char letra = A;`

---

### 082 2718

¿Cuál es la principal diferencia entre `char` y `string` en C#?  

a) `char` puede almacenar varios caracteres, mientras que `string` solo uno.
b) `string` es un valor primitivo y `char` es una clase.
c) `char` representa un único carácter, mientras que `string` puede contener una secuencia de caracteres.

---

### 083 9423

¿Qué instrucción convierte correctamente un `char` en un `string`?  

a) `char letra = 'X'; string texto = letra.ToString();`
b) `char letra = 'X'; string texto = (string)letra;`
c) `char letra = 'X'; string texto = letra + letra;`

---

### 084 2552

¿Cuál de las siguientes expresiones evalúa si una variable `char` llamada `letra` es igual al carácter 'a'?  

a) `if (letra == "a")`
b) `if (letra == 'a')`
c) `if (letra.Equals("a"))`

---

### 085 9449

¿Qué sucede si intentamos asignar una cadena de texto a una variable `char`?  

a) El código compila, pero da error en tiempo de ejecución.
b) Se produce un error de compilación, ya que `char` solo puede contener un carácter.
c) El compilador convierte automáticamente la cadena a un solo carácter.

---

### 086 1623

¿Qué valor devuelve `char.IsDigit('5')`?  

a) `true`
b) `false`
c) `'5'`

---

### 087 2614

¿Cuál de las siguientes llamadas devuelve `false`?  

a) `char.IsDigit('9')`
b) `char.IsDigit('a')`
c) `char.IsDigit('3')`

---

### 088 2319

¿Qué función se puede usar para verificar si un carácter es una letra del alfabeto?  

a) `char.IsNumber()`
b) `char.IsLetter()`
c) `char.IsDigit()`

---

### 089 9896

¿Cuál es el resultado de `char.IsLetterOrDigit('%')`?  

a) `true`
b) `false`
c) `'%'`

---

### 090 6106

¿Qué función devuelve `true` para un carácter como el espacio en blanco `' '`?  

a) `char.IsWhiteSpace(' ')`
b) `char.IsDigit(' ')`
c) `char.IsControl(' ')`

---

### 091 5471

¿Cuál de las siguientes es una forma válida de declarar un arreglo de enteros en C#?  

a) `int[] numeros;`
b) `array<int> numeros;`
c) `int numeros[];`

---

### 092 7666

¿Cuál de las siguientes declaraciones crea un arreglo de 5 elementos enteros, todos inicializados a cero?  

a) `int[] arreglo = new int[5];`
b) `int[5] arreglo = new int[];`
c) `int arreglo = new int[5];`

---

### 093 3401

¿Cuál es una forma válida de declarar e inicializar un arreglo con los valores 10, 20 y 30?  

a) `int[] datos = {10, 20, 30};`
b) `int datos = new int[3] {10, 20, 30};`
c) `int datos = (10, 20, 30);`

---

### 094 8150

¿Qué sintaxis es válida para crear un arreglo de cadenas con espacio para 4 elementos, pero sin valores iniciales?  

a) `string[] nombres = new string[4];`
b) `string[4] nombres;`
c) `string nombres = string[4];`

---

### 095 6499

¿Cuál es una forma correcta de declarar e inicializar un arreglo de tipo `char` con los caracteres 'a', 'b' y 'c'?  

a) `char[] letras = new char[] {'a', 'b', 'c'};`
b) `char letras[] = {'a', 'b', 'c'};`
c) `char letras = ['a', 'b', 'c'];`

---

### 096 5386

¿Cuál es la forma correcta de acceder al primer elemento de un arreglo llamado `numeros`?  

a) `numeros[0]`
b) `numeros[1]`
c) `numeros[first]`

---

### 097 8637

¿Cómo se asigna el valor 25 al tercer elemento de un arreglo `valores`?  

a) `valores[2] = 25;`
b) `valores[3] = 25;`
c) `valores[25] = 2;`

---

### 098 9940

Si se tiene `string[] nombres = {"Ana", "Luis", "Juan"}`, ¿qué valor tiene `nombres[1]`?  

a) `"Ana"`
b) `"Luis"`
c) `"Juan"`

---

### 099 1756

¿Qué ocurre si intentas acceder a un índice fuera del rango del arreglo?  

a) El programa ignora el acceso.
b) Se lanza una excepción en tiempo de compilación.
c) Se lanza una excepción en tiempo de ejecución.

---

### 100 5844

Dado `int[] datos = {1, 2, 3, 4}`, ¿cómo se puede acceder al último elemento usando índices normales?  

a) `datos[4]`
b) `datos[3]`
c) `datos[-1]`

---

### 101 1466

¿Qué sintaxis se puede usar para acceder al último elemento de un arreglo en C# 8.0 o superior usando índices desde el final?  

a) `arreglo[^1]`
b) `arreglo[Last]`
c) `arreglo[-1]`

---

### 102 0169

¿Qué hace la siguiente instrucción? `numeros[^2] = 99;`  

a) Asigna 99 al segundo elemento.
b) Asigna 99 al penúltimo elemento.
c) Asigna 99 al segundo desde el principio.

---

### 103 3656

¿Cuál es el valor de `frutas[^3]` si `frutas = {"Manzana", "Banana", "Pera", "Kiwi", "Uva"}`?  

a) `"Banana"`
b) `"Pera"`
c) `"Kiwi"`

---

### 104 9399

¿Cómo se recorre un arreglo `edades` y se muestra cada elemento por consola?  

a) `foreach (int edad in edades) { Console.WriteLine(edad); }`
b) `for (edad in edades) { Console.Write(edad); }`
c) `print edades;`

---

### 105 2431

¿Qué instrucción permite modificar el valor del último elemento de un arreglo `valores` a 0, usando sintaxis moderna?  

a) `valores[Length - 1] = 0;`
b) `valores[^1] = 0;`
c) `valores[last] = 0;`

---

### 106 8108

¿Cuál es la estructura más común para recorrer un arreglo de principio a fin en C#?  

a) `while`
b) `foreach`
c) `switch`

---

### 107 4197

Dado `int[] numeros = {1, 2, 3, 4}`, ¿cuál es el resultado de este código?  
```csharp  
int suma = 0;  
foreach (int n in numeros) {  
    suma += n;  
}  
Console.WriteLine(suma);  
```

a) `10`
b) `1234`
c) `0`

---

### 108 9795

¿Cuál es una ventaja de usar `foreach` en lugar de `for` al recorrer un arreglo?  

a) Permite modificar el tamaño del arreglo.
b) Es más rápido para modificar los valores.
c) Es más simple y evita errores de índice.

---

### 109 2978

¿Qué salida produce el siguiente código?  
```csharp  
string[] letras = {"a", "b", "c"};  
for (int i = 0; i < letras.Length; i++) {  
    Console.Write(letras[i]);  
}  
```

a) `abc`
b) `a b c`
c) `0 1 2`

---

### 110 6150

¿Qué hace este fragmento de código?  
```csharp  
int[] numeros = {2, 4, 6};  
for (int i = 0; i < numeros.Length; i++) {  
    numeros[i] *= 2;  
}  
```

a) Reemplaza todos los valores con 2.
b) Duplica cada valor del arreglo.
c) Agrega un nuevo elemento al final del arreglo.

---

### 111 8545

¿Qué significa que los arreglos en C# sean tipos por referencia?

a) Que se copian completamente al pasarlos como argumento.
b) Que apuntan a una ubicación de memoria compartida.
c) Que no se pueden modificar una vez creados.

---

### 112 4108

¿Qué salida produce el siguiente código?  
```csharp
void Modificar(int[] datos) {
    datos[0] = 99;
}
int[] numeros = {1, 2, 3};  
Modificar(numeros);  
Console.WriteLine(numeros[0]);
```

a) `1`
b) `99`
c) Da error de compilación.

---

### 113 6579

Si se asigna un arreglo a otra variable, como en el siguiente ejemplo:  
```csharp
int[] a = {1, 2, 3};  
int[] b = a;  
b[1] = 100;  
```
¿Cuál es el valor de `a[1]`?

a) `2`
b) `100`
c) `0`

---

### 114 8480

¿Qué sucede si se pasa un arreglo como parámetro a un método y se modifica un elemento dentro del método?

a) Solo se modifica dentro del método.
b) Se modifica tanto dentro como fuera del método.
c) No es posible modificar un arreglo dentro de un método.

---

### 115 0763

¿Qué salida produce el siguiente código?  
```csharp
int[] original = {10, 20, 30};  
int[] copia = original;  
original[2] = 99;  
Console.WriteLine(copia[2]);
```

a) `30`
b) `99`
c) Error en tiempo de ejecución.

---

### 116 1900

¿Cuál es la sintaxis correcta para declarar una estructura en C#?

a) `class Punto { int X; int Y; }`
b) `struct Punto { int X; int Y; }`
c) `structure Punto { int X; int Y; }`

---

### 117 3340

¿Qué diferencia principal existe entre una `struct` y una `class` en C#?

a) Las `struct` se pasan por valor y las `class` por referencia.
b) Las `struct` permiten herencia múltiple.
c) Las `struct` pueden tener finalizadores.

---

### 118 0624

¿Qué salida produce el siguiente código?  
```csharp
struct Coordenada {
    public int X;
    public int Y;
}
Coordenada c1 = new Coordenada { X = 5, Y = 10 };
Coordenada c2 = c1;
c2.X = 99;
Console.WriteLine(c1.X);
```

a) `5`
b) `99`
c) Error de compilación

---

### 119 7807

¿Cuál es la forma correcta de declarar una propiedad automática en una `struct`?

a) `public int Edad;`
b) `public int Edad { get; set; }`
c) `private int Edad { set; get; } = 0;`

---

### 120 7315

¿Qué ocurre si no se inicializan todos los campos de una `struct` antes de usarla?

a) El compilador inicializa los campos automáticamente a 0 o null.
b) Se lanza una excepción en tiempo de ejecución.
c) Se produce un error de compilación.

---

### 121 8484

¿Cuál de las siguientes afirmaciones sobre constructores en `struct` es correcta?

a) Una `struct` puede tener un constructor sin parámetros.
b) Una `struct` puede tener constructores con parámetros, pero no uno sin parámetros.
c) Una `struct` puede tener un constructor sin parámetros definido por el usuario.

---

### 122 0201

Dado el siguiente código, ¿cuál es la salida?  
```csharp
struct Punto {
    public int X;
    public Punto(int x) {
        X = x;
    }
}
Punto p = new Punto(3);
Console.WriteLine(p.X);
```

a) `3`
b) `0`
c) Error de compilación

---

### 123 6366

¿Es posible declarar propiedades con lógica personalizada en una `struct`?

a) No, solo se permiten propiedades automáticas.
b) Sí, siempre que todos los campos estén inicializados en el constructor.
c) Sí, pero solo en clases, no en `struct`.

---

### 124 3903

¿Qué salida produce el siguiente código?  
```csharp
struct Contador {
    public int Valor;
}
Contador a = new Contador { Valor = 10 };
Contador b = a;

a) `10`
b) `20`
c) Error de compilación

---

### 125 9515

¿Cuál de estas afirmaciones sobre `struct` es verdadera?

a) Una `struct` puede heredar de otra `struct`.
b) Una `struct` puede implementar interfaces.
c) Una `struct` puede derivar de la clase `Object` de forma explícita.

---

### 126 6927

¿Qué miembros puede contener una `struct` en C#?

a) Solo campos y propiedades
b) Campos, propiedades, métodos, constructores y eventos
c) Solo propiedades y métodos estáticos

---

### 127 2250

¿Qué característica es *única* de una propiedad automática en comparación con una propiedad normal?

a) Requiere una variable explícita como respaldo
b) El compilador genera automáticamente el campo privado asociado
c) Solo se puede usar en clases, no en `struct`

---

### 128 6323

¿Qué ocurre si no se asignan valores a todos los campos de una `struct` dentro de un constructor personalizado?

a) El compilador lo corrige automáticamente
b) Se produce una excepción en tiempo de ejecución
c) Se produce un error de compilación

---

### 129 6367

¿Qué salida produce el siguiente código?  
```csharp
struct Medida {
    public int Largo;
    public int Ancho;
}
Medida m = new Medida();
Console.WriteLine(m.Largo);
```

a) `0`
b) `null`
c) Error de compilación

---

### 130 2271

¿Cuál es la principal ventaja de usar `struct` para tipos pequeños e inmutables?

a) Menor uso de memoria en el heap
b) Mayor flexibilidad para herencia
c) Posibilidad de usar finalizadores

---

### 131 6036

¿Cuál es la sintaxis correcta para declarar una clase pública en C#?

a) `class Persona { }`
b) `public class Persona { }`
c) `Persona class { }`

---

### 132 9644

¿Qué visibilidad tiene una clase si no se especifica ningún modificador de acceso?

a) `internal`
b) `public`
c) `private`

---

### 133 1558

¿Qué salida produce el siguiente código?  
```csharp
public class Persona {
    public string Nombre;
    public Persona(string nombre) {
        Nombre = nombre;
    }
}
Persona p = new Persona("Ana");
Console.WriteLine(p.Nombre);
```

a) `null`
b) `Ana`
c) Error de compilación

---

### 134 3228

¿Qué característica permite tener varios constructores con diferente número de parámetros?

a) Delegación de constructores
b) Polimorfismo
c) Sobrecarga de constructores

---

### 135 0647

¿Cuál es la salida del siguiente código?  
```csharp
public class Producto {
    public string Nombre;
    public Producto() {
        Nombre = "Sin nombre";
    }
    public Producto(string nombre) {
        Nombre = nombre;
    }
}
Producto p = new Producto();
Console.WriteLine(p.Nombre);
```

a) `Sin nombre`
b) `null`
c) `Producto`

---

### 136 2492

¿Qué ocurre si definimos dos métodos con el mismo nombre pero diferente número de parámetros?

a) Error de compilación
b) Se produce una sobrecarga válida
c) Se ignora uno de los métodos

---

### 137 0133

¿Cuál es la forma correcta de implementar una propiedad con lógica personalizada?

a) 
```csharp
public int Edad { get; set; }
```  
b) 
```csharp
public int Edad {
    get { return edad; }
    set { if (value >= 0) edad = value; }
}
```  
c) 
```csharp
private set Edad(int edad) { this.edad = edad; }
```  

---

### 138 8470

¿Qué modificador de acceso impide que una propiedad sea visible desde fuera de la clase?

a) `public`
b) `private`
c) `static`

---

### 139 4845

¿Cuál es la salida del siguiente código?  
```csharp
public class Punto {
    private int x;
    public int X {
        get { return x; }
        set { x = value; }
    }
}
Punto p = new Punto();
p.X = 10;
Console.WriteLine(p.X);
```

a) `0`
b) `10`
c) Error en tiempo de ejecución

---

### 140 4625

¿Qué palabra clave se utiliza para restringir el acceso a una propiedad a solo dentro de la clase?

a) `internal`
b) `protected`
c) `private`

---

### 141 4851

¿Qué salida produce el siguiente código?  
```csharp
public class Animal {
    public string Nombre;
    public Animal() {
        Nombre = "Sin nombre";
    }
    public Animal(string nombre) {
        Nombre = nombre;
    }
}
Animal a = new Animal("Gato");
Console.WriteLine(a.Nombre);
```

a) `Sin nombre`
b) `null`
c) `Gato`

---

### 142 9731

¿Es válido tener una propiedad con acceso `get` público y `set` privado?

a) No, ambos deben tener la misma visibilidad
b) Sí, usando `{ get; private set; }`
c) Solo si la propiedad es estática

---

### 143 2660

¿Cuál es el resultado de compilar el siguiente código?  
```csharp
class Vehiculo {
    private string marca;
}
Console.WriteLine(new Vehiculo().marca);
```

a) Se imprime `null`
b) Error de compilación por acceso a miembro privado
c) Se imprime una cadena vacía

---

### 144 3044

¿Cuál es la diferencia principal entre campo y propiedad?

a) Las propiedades no pueden tener lógica
b) Los campos no pueden ser privados
c) Las propiedades permiten controlar el acceso a los datos

---

### 145 8113

¿Cuál es la salida del siguiente código?  
```csharp
public class Caja {
    public int Largo { get; set; }
    public int Ancho { get; private set; }
    public Caja(int largo, int ancho) {
        Largo = largo;
        Ancho = ancho;
    }
}
Caja c = new Caja(5, 3);
Console.WriteLine(c.Ancho);
```

a) `3`
b) `0`
c) Error de compilación

---

### 146 4779

¿Qué significa que un método sea `private`?

a) Solo puede ser llamado desde dentro de la clase
b) Solo puede ser llamado desde subclases
c) Puede ser accedido por cualquier parte del programa

---

### 147 2851

¿Qué ocurre si definimos dos métodos con el mismo nombre y mismos parámetros?

a) Sobrecarga válida
b) Se reemplaza el primero
c) Error de compilación

---

### 148 5057

¿Qué salida produce el siguiente código?  
```csharp
public class Operaciones {
    public int Sumar(int a, int b) {
        return a + b;
    }
    public int Sumar(int a, int b, int c) {
        return a + b + c;
    }
}
Operaciones op = new Operaciones();
Console.WriteLine(op.Sumar(2, 3));
```

a) `5`
b) `6`
c) Error de compilación

---

### 149 1641

¿Se puede tener múltiples constructores con el mismo número de parámetros pero diferente tipo?

a) No, el compilador no lo permite
b) Sí, es una forma válida de sobrecarga
c) Solo en clases abstractas

---

### 150 4217

¿Qué salida da el siguiente código?  
```csharp
public class Persona {
    public string Nombre { get; private set; }
    public Persona(string nombre) {
        Nombre = nombre;
    }
}
Persona p = new Persona("Lucía");
Console.WriteLine(p.Nombre);
```

a) `null`
b) `Lucía`
c) Error de compilación

---

### 151 8378

¿Es obligatorio tener un constructor en una clase en C#?

a) Sí, siempre debe haber al menos uno
b) No, el compilador genera uno por defecto si no se define
c) Solo si la clase tiene propiedades

---

### 152 8371

¿Qué palabra clave impide que una clase sea instanciada fuera de su ensamblado?

a) `internal`
b) `private`
c) `sealed`

---

### 153 8774

¿Cuál es el propósito de un constructor?

a) Asignar valores predeterminados a campos estáticos
b) Inicializar un objeto cuando se crea
c) Declarar métodos auxiliares

---

### 154 8759

¿Cuál de las siguientes definiciones de constructor en C# sobrecarga correctamente el constructor de la clase `Persona`?

a) 
```csharp
public class Persona {
    public Persona() { }
    public Persona(string nombre) { }
}
```
b) 
```csharp
public Persona(string nombre) { }  
public Persona(string nombre) { }
```  
c) 
```csharp
public Persona(int edad);  
public Persona(string nombre);
```  

---

### 155 8295

¿Qué ocurre si una propiedad solo tiene el `get` y no el `set`?

a) Puede leerse pero no modificarse
b) No puede usarse en ninguna parte
c) Puede modificarse desde el constructor

---

### 156 1047

¿Es válido definir una propiedad como `public` pero su `set` como `private`?

a) No, debe ser todo público o todo privado
b) Sí, es una práctica común
c) Solo para clases abstractas

---

### 157 0403

¿Cuál es la salida del siguiente código?  
```csharp
public class Reloj {
    public int Hora { get; private set; }
    public Reloj(int hora) {
        Hora = hora;
    }
}
Reloj r = new Reloj(12);
Console.WriteLine(r.Hora);
```

a) `0`
b) `12`
c) Error de compilación

---

### 158 7613

¿Qué permite la sobrecarga de métodos en una clase?

a) Tener varios métodos con el mismo nombre y misma firma
b) Tener métodos con el mismo nombre pero diferentes firmas
c) Definir múltiples clases con el mismo nombre

---

### 159 6748

¿Cuál es el propósito principal de las propiedades en una clase?

a) Exponer campos públicos
b) Controlar el acceso a los datos internos
c) Reemplazar los métodos

---

### 160 5042

¿Qué modificador se usa para que un campo solo pueda ser accedido dentro de la misma clase?

a) `protected`
b) `public`
c) `private`

---

### 161 9799

¿Cuál es la diferencia principal entre una `struct` y una `class` en C#?

a) Las `struct` son tipos por valor y las `class` son tipos por referencia.
b) Las `class` no pueden contener métodos.
c) Las `struct` permiten herencia, las `class` no.

---

### 162 3763

¿Qué ocurre cuando se asigna una `struct` a otra variable?

a) Ambas variables referencian el mismo objeto.
b) Se copia el valor, son dos instancias independientes.
c) Se lanza una excepción.

---

### 163 3131

¿Cuál de las siguientes opciones es verdadera respecto a `struct` y `class`?

a) Solo las `struct` pueden tener constructores con parámetros.
b) Solo las `class` pueden tener destructores.
c) Las `struct` pueden heredar de otras `struct`.

---

### 164 1485

¿Qué tipo de tipo es más adecuado para representar una entidad simple e inmutable como un punto en 2D?

a) `class`
b) `struct`
c) `interface`

---

### 165 2905

¿Qué diferencia de memoria existe entre `struct` y `class`?

a) Las `struct` se almacenan en el heap y las `class` en el stack.
b) Las `struct` y `class` siempre se almacenan en el heap.
c) Las `struct` normalmente se almacenan en el stack y las `class` en el heap.

---

### 166 2661

¿Cuál es el principal objetivo del `encapsulamiento`?

a) Permitir herencia múltiple
b) Proteger el estado interno de un objeto
c) Ejecutar múltiples métodos al mismo tiempo

---

### 167 0680

¿Qué nivel de acceso evita que un campo pueda ser accedido directamente desde fuera de la clase?

a) `public`
b) `protected`
c) `private`

---

### 168 0080

¿Cuál es el resultado de este código?
```csharp
class Persona {
    private string nombre = "Juan";
    public string GetNombre() {
        return nombre;
    }
}
var p = new Persona();
Console.WriteLine(p.GetNombre());
```

a) Juan
b) Error de compilación
c) null

---

### 169 6713

¿Qué permite la herencia en la programación orientada a objetos?

a) Aumentar el número de constructores
b) Reutilizar código de una clase base
c) Usar operadores sobrecargados

---

### 170 3829

Dada la siguiente jerarquía:
```csharp
class Vehiculo { }
class Auto : Vehiculo { }
```
¿Qué conversión es válida sin necesidad de casting?

a) `Vehiculo v = new Auto();`
b) `Auto a = new Vehiculo();`
c) `Auto a = (Auto)new Vehiculo();`

---

### 171 6488

¿Qué palabra clave se usa para permitir que un método pueda ser sobrescrito en una subclase?

a) `override`
b) `virtual`
c) `sealed`

---

### 172 2579

¿Qué salida produce este código?
```csharp
class Animal {
    public virtual void Hablar() {
        Console.WriteLine("Animal");
    }
}
class Perro : Animal {
    public override void Hablar() {
        Console.WriteLine("Guau");
    }
}
Animal a = new Perro();

a) Animal
b) Guau
c) Error de compilación

---

### 173 8685

¿Cuál es el tipo base de todos los tipos en C#?

a) `base`
b) `Object`
c) `Any`

---

### 174 4611

¿Qué permite el uso del operador `is` en C#?

a) Realizar herencia múltiple
b) Verificar si un objeto es de un tipo específico
c) Convertir directamente un tipo a otro

---

### 175 0450

¿Qué ocurre si se hace un cast inválido entre tipos no compatibles?

a) Se realiza la conversión correctamente
b) El programa continúa sin problemas
c) Se lanza una excepción en tiempo de ejecución

---

### 176 2120

¿Qué hace el operador `as` si la conversión falla?

a) Lanza una excepción
b) Retorna `null`
c) Convierte a `object`

---

### 177 8470

¿Cuál de estas definiciones permite una clase ser heredada pero no sobrescrita?

a) 
```csharp
class MiClase { }
```  
b) 
```csharp
sealed class MiClase { }
```  
c) 
```csharp
static class MiClase { }
```  

---

### 178 2808

¿Cuál es el propósito de la palabra clave `base` en C#?

a) Crear una instancia de la clase base
b) Acceder a miembros de la clase base desde una clase derivada
c) Ocultar un método de la clase base

---

### 179 1696

¿Qué indica la sobrecarga de métodos?

a) Métodos con el mismo nombre pero diferentes firmas
b) Herencia de múltiples clases
c) Un método reemplaza otro en una clase derivada

---

### 180 9352

¿Qué salida produce este código?
```csharp
object obj = "Hola mundo";
Console.WriteLine(obj.GetType());
```

a) `System.String`
b) `System.Object`
c) `String`

---

### 181 3336

¿Cuál es la principal ventaja del polimorfismo?

a) Permitir el uso de múltiples constructores
b) Permitir usar diferentes tipos a través de una misma interfaz
c) Reescribir métodos estáticos

---

### 182 6423

¿Qué salida produce este código?
```csharp
class A {
    public void Mostrar() { Console.WriteLine("A"); }
}
class B : A {
    public new void Mostrar() { Console.WriteLine("B"); }
}
A obj = new B();
obj.Mostrar();
```

a) A
b) B
c) Error de compilación

---

### 183 1181

¿Cuál es el efecto de usar `virtual` en un método?

a) Impide que el método se sobrescriba
b) Permite que una clase lo use como método abstracto
c) Permite que una subclase lo sobrescriba con `override`

---

### 184 5890

¿Qué salida produce este código?
```csharp
Animal a = new Animal();
Gato g = a as Gato;
Console.WriteLine(g == null);
```

a) `True`
b) `False`
c) Error de compilación

---

### 185 9178

¿Qué ocurre al asignar una instancia de clase a una variable de tipo `object`?

a) Se realiza una conversión implícita
b) Se pierde el tipo original
c) Es necesario un cast explícito

---

### 186 9680

¿Cuál de las siguientes declaraciones define correctamente una propiedad automática en C#?

a) `public int Edad { get; set; }`
b) `public int Edad => edad;`
c) `private int edad; public int Edad() { return edad; }`

---

### 187 2910

¿Cuál de las siguientes declaraciones permite inicializar una propiedad automática con un valor por defecto?

a) `public int Edad { get; set; } = 18;`
b) `public int Edad { get => 18; }`
c) `private int edad = 18; public int Edad { get; set; }`

---

### 188 6654

¿Qué salida produce el siguiente código?
```csharp
class Persona {
    public string Nombre { get; set; } = "Juan";
}
var p = new Persona();
Console.WriteLine(p.Nombre);
```

a) `Juan`
b) `null`
c) Error en tiempo de compilación

---

### 189 8262

¿Cuál de estas propiedades permite solo lectura desde fuera de la clase?

a) `public int Edad { get; private set; }`
b) `public int Edad { get; set; }`
c) `private int Edad { get; set; }`

---

### 190 2477

¿Cuál de las siguientes afirmaciones es verdadera respecto a propiedades automáticas?

a) Las propiedades automáticas no pueden tener un valor por defecto.
b) Las propiedades automáticas requieren campos explícitos.
c) Las propiedades automáticas generan automáticamente el campo de respaldo.

---

### 191 6708

¿Qué sucede si una propiedad se declara en una clase base sin usar el modificador virtual?

a) La propiedad no puede ser accedida desde clases derivadas.
b) No se puede sobrescribir la propiedad en una clase derivada.
c) La propiedad se comportará como abstract.

---

### 192 4635

¿Cuál es la forma correcta de sobrescribir una propiedad en una clase derivada?

a) `public override int Edad { get; set; }`
b) `public new int Edad { get; set; }`
c) `public int Edad { override get; set; }`

---

### 193 8743

¿Qué palabra clave se necesita en la clase base para permitir que una propiedad sea sobrescrita?

a) `sealed`
b) `abstract`
c) `virtual`

---

### 194 1139

¿Qué palabra clave evita que una clase derivada sobrescriba una propiedad heredada?

a) `sealed`
b) `private`
c) `readonly`

---

### 195 4342

¿Cuál es el resultado del siguiente código?
```csharp
class Animal {
    public virtual string Sonido { get; set; } = "Desconocido";
}
class Perro : Animal {
    public override string Sonido { get; set; } = "Ladrido";
}
Animal a = new Perro();
Console.WriteLine(a.Sonido);
```

a) Ladrido
b) Desconocido
c) Error en tiempo de ejecución

---

### 196 1491

¿Cuál es el propósito principal del delegado `Predicate<T>`?

a) Devolver un `bool` según una condición sobre un objeto de tipo `T`
b) Ejecutar una acción sobre un objeto de tipo `T` sin devolver nada
c) Comparar dos objetos de tipo `T` y devolver un entero

---

### 197 6702

¿Qué valor retorna un `Comparison<T>` cuando los dos elementos comparados son iguales?

a) -1
b) 0
c) 1

---

### 198 3319

¿Cuál es la diferencia principal entre `Func<T, bool>` y `Predicate<T>`?

a) `Func<T, bool>` solo puede usarse con tipos primitivos
b) `Predicate<T>` permite más de un parámetro
c) Son equivalentes funcionalmente, pero `Predicate<T>` tiene un propósito semántico más claro

---

### 199 1648

Dado el siguiente código:
```csharp
List<int> numeros = new List<int> { 5, 10, 15 };
Predicate<int> esPar = x => x % 2 == 0;
var resultado = numeros.Find(esPar);
```
¿Qué valor tendrá `resultado`?

a) 10
b) 15
c) 5

---

### 200 2330

¿Qué tipo de delegado es más adecuado para realizar ordenamientos personalizados?

a) `Predicate<T>`
b) `Comparison<T>`
c) `Action<T>`

---

### 201 4987

¿Cuál de las siguientes opciones representa una declaración válida de un delegado genérico?

a) `public delegate T Proceso<U>(U valor);`
b) `public delegate Resultado Delegado<T>(T entrada);`
c) `public delegate bool Validar(bool entrada);`

---

### 202 3133

¿Qué representa esta expresión lambda?
```csharp
(x, y) => x > y
```

a) Un `Func<int, int, bool>`
b) Un `Predicate<int>`
c) Un `Action<int, int>`

---

### 203 4170

Dado este código:
```csharp
List<string> palabras = new List<string> { "casa", "computadora", "sol" };
var largas = palabras.FindAll(p => p.Length > 4);
```
¿Qué contiene la lista `largas`?

a) casa, sol
b) computadora
c) computadora, casa

---

### 204 8401

¿Qué hace este código?
```csharp
Action<string> mostrar = s => Console.WriteLine(s.ToUpper());
mostrar("hola");
```

a) Imprime "hola" en consola
b) Imprime "HOLA" en consola
c) Genera un error de compilación

---

### 205 8794

¿Qué tipo se adapta mejor a esta lambda: `(a, b) => a + b`?

a) `Func<int, int>`
b) `Func<int, int, int>`
c) `Action<int, int>`

---

### 206 0186

¿Qué interfaz se debe implementar para crear un iterador personalizado en C#?

a) `IEnumerable<T>`
b) `IEnumerator<T>`
c) `IList<T>`

---

### 207 2048

¿Cuál de los siguientes métodos pertenece a la interfaz `IEnumerator`?

a) `GetEnumerator()`
b) `MoveNext()`
c) `Add()`

---

### 208 8862

¿Qué valor devuelve `MoveNext()` cuando la iteración ha terminado?

a) `true`
b) `false`
c) `null`

---

### 209 5138

¿Qué propiedad se usa para acceder al elemento actual en una iteración con `IEnumerator`?

a) `CurrentItem`
b) `GetCurrent()`
c) `Current`

---

### 210 3304

¿Qué sucede si llamas a la propiedad `Current` **antes de la primera llamada a `MoveNext()`**?

a) Se lanza una excepción
b) Devuelve `null`
c) Devuelve el primer elemento de la colección

---

### 211 1173

¿Qué responsabilidad tiene el método `Reset()` en `IEnumerator`?

a) Eliminar todos los elementos de la colección
b) Volver al primer elemento de la colección
c) Cerrar la colección y liberar recursos

---

### 212 8019

¿Cuál de las siguientes afirmaciones es **correcta** respecto al uso de `IEnumerator`?

a) Se puede modificar la colección durante la iteración
b) Se puede recorrer la colección sin llamar a `MoveNext()`
c) `MoveNext()` debe llamarse antes de acceder a `Current`

---

### 213 7741

¿Qué método proporciona un objeto que implementa `IEnumerator` desde una colección que implementa `IEnumerable`?

a) `GetEnumerator()`
b) `GetIterator()`
c) `StartIteration()`

---

### 214 8975

¿Por qué se suele preferir `foreach` en lugar de usar `IEnumerator` directamente?

a) Porque `foreach` es más seguro y legible
b) Porque `IEnumerator` no permite recorrer listas
c) Porque `foreach` es más rápido en todos los casos

---

### 215 9878

¿Qué método de LINQ se utiliza para **filtrar elementos** de una colección?

a) Select
b) Where
c) OrderBy

---

### 216 2179

¿Qué hace el método `Select` en LINQ?

a) Elimina elementos duplicados
b) Proyecta cada elemento en una nueva forma
c) Filtra elementos según una condición

---

### 217 3186

¿Qué devuelve el método `First()`?

a) El último elemento de una secuencia
b) El primer elemento que cumple una condición
c) El primer elemento de una secuencia

---

### 218 4551

¿Cuál es la diferencia entre `First()` y `FirstOrDefault()`?

a) `FirstOrDefault()` lanza excepción si no hay elementos
b) `First()` devuelve null si no encuentra elementos
c) `FirstOrDefault()` devuelve un valor por defecto si no encuentra elementos

---

### 219 2459

¿Qué método se utiliza para **ordenar de forma descendente** una colección?

a) OrderByDescending
b) OrderBy
c) SortDescending

---

### 220 6470

¿Cuál de los siguientes métodos sirve para **verificar si algún elemento cumple** una condición?

a) Where
b) Any
c) All

---

### 221 5495

¿Qué método usás para **probar si todos** los elementos cumplen una condición?

a) Any
b) All
c) Contains

---

### 222 9989

¿Qué hace el método `Distinct()`?

a) Ordena la lista
b) Devuelve solo los elementos distintos
c) Elimina los valores nulos

---

### 223 9278

¿Cuál de los siguientes métodos devuelve **un único elemento** y lanza excepción si hay más de uno?

a) First
b) Single
c) Last

---

### 224 4490

¿Qué método se usa para **contar** los elementos de una colección?

a) Length()
b) Count()
c) Sum()

---

### 225 8923

¿Qué hace el método `Take(3)` en una colección?

a) Toma los últimos 3 elementos
b) Toma los primeros 3 elementos
c) Toma cualquier 3 elementos aleatorios

---

### 226 8989

¿Para qué se usa `Skip(2)`?

a) Ignora los dos primeros elementos
b) Ignora los dos últimos elementos
c) Elimina dos elementos aleatorios

---

### 227 7262

¿Qué método LINQ usás para transformar una lista de enteros a una lista de strings?

a) Select
b) Where
c) ToString

---

### 228 7492

¿Qué hace `OrderBy(x => x.Length)`?

a) Ordena por orden alfabético
b) Ordena por longitud de los elementos
c) Filtra elementos largos

---

### 229 6836

¿Cuál de estos métodos se puede combinar con `GroupBy` para obtener estadísticas?

a) Sum
b) GroupSum
c) Aggregate

---

### 230 7074

¿Cuál es el propósito de `Aggregate()`?

a) Ejecutar operaciones acumulativas sobre una secuencia
b) Dividir una colección en grupos
c) Aplicar filtros múltiples

---

### 231 8767

¿Qué devuelve `DefaultIfEmpty()`?

a) El primer elemento que cumple una condición
b) Un elemento por defecto si la colección está vacía
c) Siempre un valor nulo

---

### 232 9129

¿Qué hace el método `Max()`?

a) Devuelve el número de elementos
b) Devuelve el valor máximo
c) Filtra el máximo elemento

---

### 233 7157

¿Qué tipo de resultado devuelve el método `GroupBy()`?

a) Una lista plana
b) Una secuencia de grupos con clave
c) Un único valor

---

### 234 0765

¿Cuál es el propósito de `SelectMany()`?

a) Combina múltiples secuencias en una sola
b) Selecciona el valor más grande
c) Filtra elementos que cumplen muchas condiciones

---
