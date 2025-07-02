using static System.Console;
using System.Text;
using System.Globalization;

// Funcion auxiliar para facilitar la lectura de datos
string Leer(string mensaje){
    Write(mensaje);
    return ReadLine();
}

// Simula una pila de nombres mediante funciones y un array.

int cantidad = 0;
string[] pila = new string[100];

void Push(string valor){
    pila[cantidad++] = valor;
}

string Pop(){
    return pila[--cantidad];
}

bool IsEmpty(){
    return cantidad == 0;
}


// Ejemplo de uso de la pila
Clear();
WriteLine("\n -- Ejemplo de pila (global) -- \n");
Push("Juan");
Push("Pedro");
while(!IsEmpty()){
    WriteLine($" - {Pop()}");
}

WriteLine("\n -- Ejemplo de pila cargar desde teclado -- \n");
WriteLine("Ingrese nombres (deje vacío para terminar):");
while(true){
    var valor = Leer("Nombre: ");
    if(valor=="") break;
    Push(valor);
}

while(!IsEmpty()){
    WriteLine($" - {Pop()}");
}


// -------------------------

// Defino un nuevo tipo de dato llamado Stack
// me permite tener multiples copias o instancias de la pila
// Stack es una estructura de datos que permite almacenar elementos
// en un orden específico, siguiendo el principio LIFO (Last In, First Out).

struct Stack {
    int cantidad = 0;
    string[] elementos = new string[100];

    public Stack() { 
        // Inicializamos explícitamente los campos
        cantidad = 0;
        elementos = new string[100];
    }

    public void Push(string valor) {
        elementos[cantidad++] = valor;
    }

    public string Pop() {
        return elementos[--cantidad];
    }

    public bool IsEmpty() {
        return cantidad == 0;
    }
}


WriteLine("\n -- Ejemplo de pila (Stack) -- \n");
var s = new Stack();  // Crea una nueva instancia de Stack
s.Push("Juan");
s.Push("Pedro");
s.Push("Maria");
while(!s.IsEmpty()) {
    WriteLine($" - {s.Pop()}");
}

// Aca se define una pila de 'int' (enteros)
// Es identica a las anterior pero con int en lugar de string
// Implementacion minima usando funciones lambda ( => )
struct StackInt {
    int cantidad = 0;
    int[] elementos = new int[100];
    
    public StackInt() {}
    public void Push(int valor) => elementos[cantidad++] = valor;
    public int Pop() => elementos[--cantidad];
    public bool IsEmpty() => cantidad == 0;
}

// Se puede definir una pila de 'string' (cadenas)
// Es identica a las anterior pero con string en lugar de int
struct StackString {
    int cantidad = 0;
    string[] elementos = new string[100];

    public StackString() {}
    public void Push(string valor) => elementos[cantidad++] = valor;
    public string Pop() => elementos[--cantidad];
    public bool IsEmpty() => cantidad == 0;
}

/// TIPO GENERICO ///
// Podemos generalizar la pila para cualquier tipo de dato
// Stack<T> es una pila del tipo generico T 
// Para usarlos reemplazamos T por el tipo deseado
struct Stack<T> {
    int cantidad = 0;
    T[] elementos = new T[100];

    public Stack() {}
    public void Push(T valor) => elementos[cantidad++] = valor;
    public T Pop() => elementos[--cantidad];
    public bool IsEmpty() => cantidad == 0;
}

WriteLine("\n -- Ejemplo de pila generica (Stack<T>) -- \n");
WriteLine("\nEjemplo de Stack con enteros\n");
var si = new Stack<int>();
si.Push(1);
si.Push(2);
while(!si.IsEmpty()) WriteLine($" - {si.Pop()}");

WriteLine("\nEjemplo de Stack con dobles\n");
var sd = new Stack<double>();
sd.Push(1.1);
sd.Push(2.2);
while(!sd.IsEmpty()) WriteLine($" - {sd.Pop()}");

WriteLine("\nEjemplo de Stack con cadenas\n");
var ss = new Stack<string>(); //Exactamente igual a Stack
ss.Push("Juan");
ss.Push("Pedro");
while(!ss.IsEmpty()) WriteLine($" - {ss.Pop()}");
