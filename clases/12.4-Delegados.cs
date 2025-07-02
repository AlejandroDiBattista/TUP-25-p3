delegate int Operacion(int a, int b);               // Definición del delegado

int Suma(int a, int b) {
    return a + b;
}

int Resta(int a, int b) {
    return a - b;
}
int Resta2(int a, int b) => a - b; // Función de una sola línea

// La definición del tipo es el nombre de la función 


Operacion op = Suma;                                // Asignación de la función Suma al delegado
int resultado = op(10, 5);                          // Llamada a la función a través del delegado
WriteLine($"Resultado de la suma: {resultado}");    // Resultado de la suma: 15

op = Resta;                                         // Asignación de la función Resta al delegado
resultado = op(10, 5);                              // Llamada a la función a través del delegado
WriteLine($"Resultado de la resta: {resultado}");   // Resultado de la resta: 5

// Delegados como parámetros
void EjecutarOperacion(int a, int b, Operacion operacion) {
    int resultado = operacion(a, b);
    WriteLine($"Resultado de la operación: {resultado}");
}

EjecutarOperacion(10, 5, Suma);  // Resultado de la operación: 15
EjecutarOperacion(10, 5, Resta); // Resultado de la operación:  5

int mul(int a, int b) => a * b; // Función de una sola línea

Operacion o2 = (int a, int b) => a * b; // Función anónima
Operacion o3 = (a, b) => a - b; // Función anónima

Func<int, int, int> suma  = (a, b) => a + b;
Func<int, int, int> resta = (a, b) => a - b;

string RepetirCadena(string texto, int veces) {
    string resultado = "";
    for (int i = 0; i < veces; i++) {
        resultado += texto;
    }
    return resultado;
}

Func<string, int, string> repetirCadena = (texto, veces) => {
    string resultado = "";
    for (int i = 0; i < veces; i++) {
        resultado += texto;
    }
    return resultado;
};


void saludar(string nombre) {
    WriteLine($"Hola {nombre}");
    return;
}

delegate void Saludo(string nombre); // Definición del delegado

Action<string> saludo = saludar;
saludo("Juan"); // Hola Juan

void repetir(string nombre, int veces) {
    for (int i = 0; i < veces; i++) {
        WriteLine($"Hola {nombre}");
    }
}

Action<string, int> repetirSaludo = repetir;

bool esPar(int n) {
    return n % 2 == 0;
}

delegate bool Chequear(int n); // Definición del delegado

Chequear par = esPar; // Asignación de la función esPar al delegado
Func<int, bool> par1 = esPar;
Predicate<int> par2  = esPar; // Predicate es un Func con un solo parámetro y devuelve bool

WriteLine(par(4)); // true
WriteLine(par1(4)); // true
WriteLine(par2(4)); // true
// Funciones anónimas

Func<int, int> cuadrado = x => x * x;
int cuadrado2(int x) => x * x;
int cuadrado3(int x) { return x * x; }


var suma2  = (int a, int b) => a + b;
var resta2 = (int a, int b) => a - b;

int[] numeros = { 1, 2, 3, 4, 5 };


var s = numeros.Sum(); // 15

var min = numeros.Min(); // 1
var max = numeros.Max(); // 5
var avg = numeros.Average(); // 3
var count = numeros.Count(); // 5

var pares = new List<int>(){2, 4, 6, 8, 10};

min = pares.Min(); // 2
max = pares.Max(); // 10
avg = pares.Average(); // 6

void mostrar(int n) {
    WriteLine($"Mostrar :{n}");
}

void grabar(int n) {
    WriteLine($"Grabar :{n}");
}

Action<int> mostrarAccion;

mostrarAccion = mostrar; // Asignación de la función mostrar al delegado
mostrarAccion += grabar; // Se pueden agregar varias acciones al mismo delegado
mostrarAccion += (n) => WriteLine($"Otro mas :{n}");     // Se pueden agregar varias acciones al mismo delegado

WriteLine("> Delegado multicasting");
mostrarAccion(10); // Mostrar :10
                   // Grabar :10

WriteLine("> Eliminamos la acción mostrar");
mostrarAccion -= mostrar; // Se pueden quitar acciones al mismo delegado
mostrarAccion(20); // Grabar :20

WriteLine("-------");
// Se pueden usar expresiones lambda para definir acciones
Action<int> mostrarAccion2 = n => WriteLine($"Mostrar :{n}");
Action<int> grabarAccion2  = n => WriteLine($"Grabar :{n}");


mostrarAccion2 += grabarAccion2; // Se pueden agregar varias acciones al mismo delegado
mostrarAccion2(30); // Mostrar :30
                     // Grabar :30
// Se pueden usar expresiones lambda para definir acciones