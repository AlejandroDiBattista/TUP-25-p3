using static System.Console; // Importa funciones de la consola


// Funciones básicas de la consola.
void demoConsola(){
    Clear();                    // Limpia la consola
    WriteLine("Hello, World!"); // Escribe en la consola en una nueva línea

    Write("Hola");              // Escribe en la consola sin salto de línea
    Write(" Mundo");
    WriteLine();                // Salto de línea

    Write("Escriba un valor: ");    
    string valor = ReadLine();  // Lee un valor de la consola

    WriteLine("Pulse una tecla para continuar...");
    ReadKey();                  // Espera a que el usuario presione una tecla
    WriteLine($"Valor: {valor}");
}

// Ejercicio 1: Leer nombres y mostrarlos en orden inverso //

// Leer 5 nombres de la consola y mostrarlos en orden inverso
void demoIngresarNombres(){
    // Se usan variables con nombres sucesivos para almacenar los nombres
    // y luego mostrarlos en orden inverso.
    
    Clear();
    Write("Ingrese el nombre 1:");
    string nombre1 = ReadLine();

    Write("Ingrese el nombre 2:");
    string nombre2 = ReadLine();
    
    Write("Ingrese el nombre 3:");
    string nombre3 = ReadLine();
    
    Write("Ingrese el nombre 4:");
    string nombre4 = ReadLine();
    
    Write("Ingrese el nombre 5:");
    string nombre5 = ReadLine();

    WriteLine("Los nombres son:");
    WriteLine($" - {nombre5}");
    WriteLine($" - {nombre4}");
    WriteLine($" - {nombre3}");
    WriteLine($" - {nombre2}");
    WriteLine($" - {nombre1}");
}

// Leer 5 nombres de la consola y mostrarlos en orden inverso usando función auxiliar
void demoIngresarNombresAuxilar(){
    // Se utiliza una función auxiliar para facilitar la lectura de nombres.
    
    string leer(string texto){ // Función auxiliar para leer un nombre
        Write(texto);
        return ReadLine();
    }

    Clear();
    string nombre0 = leer("Ingrese nombre 0:");
    string nombre1 = leer("Ingrese nombre 1:");
    string nombre2 = leer("Ingrese nombre 2:"); 
    string nombre3 = leer("Ingrese nombre 3:");
    string nombre4 = leer("Ingrese nombre 4:");

    WriteLine("Los nombres son:");
    WriteLine($" - {nombre4}");
    WriteLine($" - {nombre3}");
    WriteLine($" - {nombre2}");
    WriteLine($" - {nombre1}");
    WriteLine($" - {nombre0}");
}

// ¿Para qué sirven los arrays? Permiten acceder a un conjunto de valores del mismo tipo mediante un índice.
void demoIngresarNombresConArray(){
    // Declarar un array de 5 elementos
    string[] nombres = new string[5];

    Clear();

    // Leer 5 nombres de la consola
    for (int i = 0; i < nombres.Length; i++){ // Recorrido en orden normal
        Write($"Ingrese el nombre {i+1}:");
        nombres[i] = ReadLine();
    }

    // Mostrar los nombres en orden inverso
    WriteLine("Los nombres son:");
    for (int i = nombres.Length - 1; i >= 0; i--){ // Recorrido en orden inverso
        WriteLine($" - {nombres[i]}");
    }
}

void demoDeclaracionArray(){
    // 1) Declarar un array de 5 elementos
    string[] nombres;           // Declarar un array de cadenas
    nombres = new string[5];    // Crear un array de 5 elementos
    nombres[0] = "Juan";        // Se asigna el primer elemento
    nombres[1] = "Pedro"; 
    nombres[2] = "Maria"; 
    nombres[3] = "Jose";  
    nombres[4] = "Ana";   

    // 2) Declarar y crear un array de 5 elementos
    int[] numeros = new int[5]; // Declara y crea un array de 5 elementos
    numeros[0] = 1;            // Se asigna el primer elemento
    numeros[1] = 2;
    numeros[2] = 3;
    numeros[3] = 4;
    numeros[4] = 5;

    // 3) Declarar y crear un array de 5 elementos con asignación de valores
    int[] pares   = new int[]{2, 4, 6, 8, 10}; // Declaración completa
    int[] impares = new []{1, 3, 5, 7, 9};      // Tipo inferido del array
    int[] primos  = {2, 3, 5, 7, 11};           // Tipo implícito del array
    var triples   = new int[]{3, 6, 9, 12, 15};  // Array con tipo inferido
}

void demoMultidimencion(){
    int[,] matriz = new int[3, 4]; // Declara una matriz de 3 filas y 4 columnas

    for (int i = 0; i < matriz.GetLength(0); i++){
        for (int j = 0; j < matriz.GetLength(1); j++){
            matriz[i, j] = i * j;
        }
    }
    WriteLine("Matriz:");
    for (int i = 0; i < matriz.GetLength(0); i++){
        for (int j = 0; j < matriz.GetLength(1); j++){
            Write($"{matriz[i, j]} ");
        }
        WriteLine();
    }
    WriteLine();
    WriteLine($"Filas   : {matriz.GetLength(0)}");
    WriteLine($"Columnas: {matriz.GetLength(1)}");
    WriteLine($"Total   : {matriz.Length}");
    WriteLine($"Dimensiones: {matriz.Rank}"); // Cantidad de dimensiones
    WriteLine($"      [0,0]: {matriz[0, 0]}"); // Primer elemento
    WriteLine($"      [1,1]: {matriz[1, 1]}"); // Segundo elemento
}

demoMultidimencion();

void demoConvertir(){
    Clear();

    // Inicializamos un array con 5 nombres
    string[] nombres = new string[]{ "Juan","Pedro","Maria","Jose","Ana" };

    // Inicializamos un array con 10 dígitos
    int[] digitos = new int[]{0, 1, 2, 3, 4, 5, 6, 7, 8, 9};
    
    // Formas de inicializar un array:
    int[] primos  = new int[]{2, 3, 5, 7, 11, 13};   // Declaración completa
    int[] primos1 = new []{2, 3, 5, 7, 11, 13};      // Tipo inferido del array
    int[] primos2 = {2, 3, 5, 7, 11, 13};            // Tipo implícito del array

    var impares = new int[]{1, 3, 5, 7, 9, 11, 13, 15, 17, 19};
    int[] pares = [2, 4, 6, 8, 10, 12, 14, 16, 18, 20]; // Sintaxis alternativa: {} -> []

    for (int i = 0; i < nombres.Length; i++){
        Write($"{nombres[i]} - ");
    }
    WriteLine();

    // string.Join(separador, lista) une los elementos de la lista con el separador.
    // Convierte el string[] en un string.
    WriteLine($"Nombres: {string.Join(", ", nombres)}");

    // string.Split(separador) divide el string y lo convierte en un string[]
    string[] otro = "Juan, Pedro, Maria, Jose, Ana".Split(", ");
    WriteLine($"Otro: {string.Join("; ", otro)}");
    for (int i = 0; i < otro.Length; i++){
        WriteLine(otro[i]);
    }
    WriteLine($"Primos: {string.Join(", ", primos)}");
}

// demoConvertir();

void demoCopiaReferencia(){
    Clear();
    string[] nombres = new string[]{"Juan","Pedro","Maria","Jose","Ana"};

    WriteLine($"Nombres : {string.Join(",", nombres)}");

    string[] alias = nombres; // Se copia la referencia

    WriteLine($"Alias   : {string.Join(",", alias)}");
    alias[0] = "Clotilde";  // Cambia el primer elemento

    WriteLine("Cambio alias[0]");
    WriteLine($"Alias   : {string.Join(",", alias)}");
    WriteLine($"Nombres : {string.Join(",", nombres)}"); // Los nombres también cambian

    nombres[^1] = "Clodomira"; // ^1 -> último elemento
    WriteLine("Cambio nombres[^1]");
    WriteLine($"Alias   : {string.Join(",", alias)}");   // Alias también cambia
    WriteLine($"Nombres : {string.Join(",", nombres)}");

    string[] copia = new string[nombres.Length];

    for (int i = 0; i < nombres.Length; i++){
        copia[i] = nombres[i];
    }

    WriteLine($"Copia   : {string.Join(",", copia)}");
    copia[2] = "Bostezon";

    WriteLine("\nCambio en copia[2]");
    WriteLine($"Copia   : {string.Join(",", copia)}");
    WriteLine($"Nombres : {string.Join(",", nombres)}"); // El array original no cambia
}

void demoIngresoVariable(){
    // Lee nombres de la consola hasta que se ingrese un nombre vacío
    // y luego los muestra en orden inverso.

    int cantidad = 0;
    string[] pila = new string[100];

    Clear();
    while(cantidad < pila.Length){
        Write("Ingrese un nombre:");
        string valor = ReadLine();
        if(valor == "") break;
        pila[cantidad++] = valor;           // Almacena el nombre en la pila
    }
    WriteLine("Nombres en orden inverso");
    while(cantidad > 0){
        WriteLine($"- {pila[--cantidad]}"); // Extrae el nombre de la pila
    }
}

void demoPila(){
    // Simula una pila de nombres mediante funciones y un array.

    string[] pila = new string[100];
    int cantidad = 0;

    void Push(string valor){
        pila[cantidad++] = valor;
    }

    string Pop(){
        return pila[--cantidad];
    }

    bool IsEmpty(){
        return cantidad == 0;
    }

    Clear();
    while(true){
        Write("Nombre :");
        var valor = ReadLine();
        if(valor=="") break;
        Push(valor);
    }

    while(!IsEmpty()){
        WriteLine($" - {Pop()}");
    }
}

void demoFuncionesDeArrays()
{
    Clear();
    int[] numeros = { 5, 2, 8, 1, 9 };
    WriteLine($"- Array original: {string.Join(", ", numeros)}");

    // Ordenar el array de forma ascendente
    Array.Sort(numeros);
    WriteLine($"- Después de Sort (ascendente): {string.Join(", ", numeros)}");

    // Invertir el array
    Array.Reverse(numeros);
    WriteLine($"- Después de Reverse: {string.Join(", ", numeros)}");

    // Buscar el índice de un elemento
    int indice = Array.IndexOf(numeros, 2);
    WriteLine($"- Índice del número 2: {indice}");

    // Encontrar el primer número par
    int primerPar = Array.Find(numeros, n => n % 2 == 0); // (expresión lambda)
    WriteLine($"- Primer número par: {primerPar}");

    // Obtener todos los números impares
    int[] impares = Array.FindAll(numeros, n => n % 2 != 0);
    WriteLine($"- Números impares: {string.Join(", ", impares)}");

    // Verificar si existe algún número mayor a 10
    bool existeMayor10 = Array.Exists(numeros, n => n > 10);
    WriteLine($"- Existe un número mayor a 10: {existeMayor10}");

    // Limpiar los dos primeros elementos (ponerlos a 0)
    Array.Clear(numeros, 0, 2);
    WriteLine($"- Después de Clear en dos primeros elementos: {string.Join(", ", numeros)}");
}

void demoIndicesYRangos()
{
    Clear();

    int[] numeros = { 10, 20, 30, 40, 50 };
    WriteLine($"- Elemento al final con índice inverso (^1): {numeros[^1]}"); // 50
    WriteLine($"- Elemento antes del último con índice inverso (^2): {numeros[^2]}"); // 40

    // Extraer subarrays usando rangos: seleccionar elementos desde el índice 1 hasta el 3
    int[] subarray = numeros[1..4];
    WriteLine($"- Subarray usando rango [1..4]: {string.Join(", ", subarray)}"); // 20, 30, 40

    // Usar rango con índices inversos: extraer elementos desde el tercer elemento hasta el penúltimo
    int[] subarray2 = numeros[^4..^1];
    WriteLine($"- Subarray usando rango [^4..^1]: {string.Join(", ", subarray2)}"); // 20, 30, 40
}

void demoPasarPorReferencia()
{
    void Duplicar(ref int[] numeros){
        for(int i = 0; i < numeros.Length; i++){
            numeros[i] *= 2;
        }
    }

    int[] numeros = new int[]{1, 2, 3, 4, 5};
    Clear();
    WriteLine($"- Array original : {string.Join(", ", numeros)}");
    Duplicar(ref numeros);
    WriteLine($"- Array duplicado: {string.Join(", ", numeros)}");
}


void demoCopiarArray()
{
    int[] Copiar(int[] numeros) {
        int[] copia = new int[numeros.Length];
        for(int i = 0; i < numeros.Length; i++){
            copia[i] = numeros[i];
        }
        return copia;
    }

    int[] CopiaConCopyTo(int[] numeros){
        int[] copia = new int[numeros.Length];
        numeros.CopyTo(copia, 0);
        return copia;
    }

    int[] CopiarConRango(int[] numeros) {
        return numeros[0..^1]; // Crea un nuevo array con los elementos desde el índice 0 hasta el penúltimo (Magic!!)
    }

    Clear();
    int[] numeros = { 1, 2, 3, 4, 5 };
    WriteLine($"- Array original : {string.Join(", ", numeros)}");
    
    int[] copia1 = Copiar(numeros);
    WriteLine($"- Array copiado  : {string.Join(", ", copia1)}");
    
    numeros[0] = 99;
    WriteLine("Modifico el original pero la copia no se ve afectada");
    WriteLine($"- Array original : {string.Join(", ", numeros)}");
    WriteLine($"- Array copiado  : {string.Join(", ", copia1)}");
    
    int[] copia2 = CopiaConCopyTo(numeros);
    WriteLine($"- Array clonado  : {string.Join(", ", copia2)}");
    
    int[] copia3 = CopiarConRango(numeros); // 
    copia3[^1] = 99;
    WriteLine($"- Array original : {string.Join(", ", numeros)}");
    WriteLine($"- Array copiado  : {string.Join(", ", copia3)}");
}
