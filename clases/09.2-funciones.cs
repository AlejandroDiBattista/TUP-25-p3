using System.IO;
// Otra forma de manejar la complejidad son las funciones...
// Encondemos en una 'caja negra' la funcionalidad y evitamos ver los detalles internos.

int a = 10, b = 10;

int menor; 
if(a < b)
    menor = a;
else 
    menor = b;

// Lo ponemos dentro de un bloque

{
    int menor;          // Observe que esta variable solo existe dentro del {}
    if(a < b)
        menor = a;
    else 
        menor = b;
}

// le ponemos un nombre... , le damos las entradas y produce la salida...

int Minimo(int a, int b){  // Un punto de entradad
    int min;
    if( a < b){
        min = a;
    } else {
        min = b;
    }
    return min;             // Un punto de salida
}


a = 10;
b = 5;
menor = Minimo(a, b);

// Lo que equivale a ...
menor = Minimo(10, 5);

// Ahora con el nombre ya podemos ver que hace... no parece muy importante...

int Minimo(int a, int b, int c){
    int menor;
    if( a < b) {
        if( a < c) {
            menor = a;
        } else {
            menor = b;
        }
    } else {
        if( b < c ){
            menor = b;
        } else {
            menor = c;
        }
    }
    return menor;
}

menor = Minimo(10, 20, 5); // Mucho mas facil de entender y usar...

// Veamos con 4 parametros....

int Minimo(int a, int b, int c, int d){
    return Minimo( Minimo(a, b), Minimo(c, d));
}

// Aca volvimos a los `if anidados` pero potencialmente mas simple de leer y entender.
int Minimo(int a, int b, int c, int d, int e){
    var menor = a;
    menor = Minimo(menor, a);
    menor = Minimo(menor, b);
    menor = Minimo(menor, c);
    menor = Minimo(menor, d);
    menor = Minimo(menor, e);
    return menor;
}

// Aca lo implementamos con 5 parametros y usando `if en secuencia' 
// ahora tenemos varias funciones.
//   Minimo(a,b)
//   Minimo(a, b, c)
//   Minimo(a, b, c, d)
//   Minimo(a, b, c, d, e)

// o en redalidad
//   int Minimo(int, int)
//   int Minimo(int, int, int)
//   int Minimo(int, int, int, int)
//   int Minimo(int, int, int, int, int)

// C# permite definir varias funciones con el mismo nombre si puede distinguirlas...
// y usa la cantidad y tipo de parametros para ello.

double Minimo(double a, double b){
    return a < b ? a : b;
}

// Aca no hay conflicto con Minimo(int, int) porque este es Minimo(double, double)
// Incluso podriamos tener algo asi como Minimo(int, double) o Minimo(double, int) y todas seria
// funciones diferente, que hace lo mismo conceptualmente, pero que tiene cada una su implementacion.

// A esta altura si tuvieramos que calcular en minimo de 10 valores... de haria muy grande...
// Para estas cosas se crearon los array (coleccion de valores del mismo tipo accesible por un indice)

int Minimo(int []num){
    var min = int.MaxValue;
    foreach(var x in num)
        if(x < min)
            min = x;
    return min;
}

// Esta funcion es muy flexible pero cambia la forma de usarser...
int c = 5, d = 3, e = 15;

menor = Minimo([a, b, c]);          // Simula 3 parametros
menor = Minimo([a, b, c, d, e]);    // Simula 5 parametros

// pero pero pero... ahora tengo que trabajar con array.. pero hay una solucion.

int Maximo(params int []num){
    var max = int.MaxValue;
    foreach(var x in num)
        if(x > max)
            max = x;
    return max;
}

// Y ahora se puede usar...
var mayor = Maximo(a, b, c);    // 3 parametros 
mayor = Maximo(a, b, c, d, e);  // 5 parametros
// Pasamos los parametros como lista de valores y lo recibe como un array.


// Resumiendo... las funciones se pueden llamar igual siempre que el tipo y cantidad de parametros sean distintos.
// Existe la posibilidad de pasar parametros variables...

// Una funcion de libreria que se aprovecha del 'polimorfismo' es WriteLine


// Habitualmente la llamados como

WriteLine("Hola Mundo");
// o incluso 
var nombre = "Juan";
WriteLine($"Hola {nombre}"); // 1ra Forma (un string)

// En ambos casos recibe un unico string.

int x = 10, y = 20;

WriteLine("Punto({0}, {1})", x, y); // 2da Forma... (un string para el formato, parametros variables...)

// Pasaje por valor / referencia...

int[] pares = [2, 4, 6, 8, 10];

void Duplicar(int[] numeros){
    for(int i = 0; i < numeros.Length; i++)
        numeros[i] *= 2;
}

Duplicar(pares);
WriteLine("{0} {1} {2}", pares[0], pares[1], pares[2]);

// Sorpresa... los valores por referncias se modifican dentro de la funcion...

// void DuplicarMal(int [] numeros){
//     foreach(var x in numeros)
//         x *= 2; // No afecta a la variable original
//         // Error no se puede modicar la variable con la que recorre el array
// }

void Incrementar(int x){
    x++;
}

y = 10;
Incrementar(y);
WriteLine($"El valor de y es {y}");
// Sorpresa... el valor no es cambiado... 

// Los tipos por valor... copian el valor.
// Los tipos por referencias... copian la referencia..

struct PuntoS{
    public int X, Y;
}

var ps = new PuntoS{X = 10, Y = 10};

void Mover(PuntoS p){
    p.X += 1;
    p.Y += 1;
}

WriteLine("\nStruct -> por Valor");
WriteLine($"Antes   > Punto S: {ps.X}, {ps.Y} ");
Mover(ps);
WriteLine($"Despues > Punto S: {ps.X}, {ps.Y} ");


class PuntoC{
    public int X, Y;
}

var pc = new PuntoC{X = 10, Y = 10};

void Mover(PuntoC p){
    p.X += 1;
    p.Y += 1;
}

WriteLine("\nClass -> por Referencia");
WriteLine($"Antes   > Punto C: {pc.X}, {pc.Y} ");
Mover(pc);
WriteLine($"Despues > Punto C: {pc.X}, {pc.Y} ");



void Mover(ref PuntoS p){ // Observe `ref`
    p.X += 1;
    p.Y += 1;
}

WriteLine("\nref Struct -> por Referencia");
WriteLine($"Antes   > Punto S: {ps.X}, {ps.Y} ");
Mover(ref ps); // Observe `ref`
WriteLine($"Despues > Punto S: {ps.X}, {ps.Y} ");

// En este caso el struct se pasa por referencia y se modifica el original.


void Incrementar(ref int x){
    x++;
}

int z = 10;
WriteLine($"Antes  > z: {z} ");
Incrementar(ref z);
WriteLine($"Despues > z: {z}");

// Y si quiero retornar 2 valores??
// Dame el minimo y maximo de un array.

void MinimoMaximo(int[] numeros, out int min, out int max){
    min = int.MaxValue;
    max = int.MinValue;
    foreach(var x in numeros){
        if(x < min)
            min = x;
        if(x > max)
            max = x;
    }
}

MinimoMaximo([10, 20, 10, 30], out x, out y);
WriteLine($"Min: {x}, Max: {y}");

// Valores por defecto 
//  Declarar parametros es como declarar variables, asi como se puede asignar variables se puede asignar paratros.

void Triplicar(int[] numeros){
    for(var i = 0; i < numeros.Length; i++)
        numeros[i] *= 3;
}

void Cuadruplicar(int[] numeros){           // Identico a triplicar pero cambia el 3 por 4
    for(var i = 0; i < numeros.Length; i++)
        numeros[i] *= 4;            
}


void Multiplicar(int[] numeros, int factor = 2){
    for(var i = 0; i < numeros.Length; i++)
        numeros[i] *= factor;            

}
int[] numeros = [10, 200, 20, 100, 2, 1];

Multiplicar(numeros, 2);            // x 2
Multiplicar(numeros, 3);            // x 3
Multiplicar(numeros);               // x 2 (valor por defecto)
Multiplicar(numeros, factor: 4);    // x 4 (parametro nombrado)


// string[][] LeerCSV(string origen){
//     //...
// }

// En realidad el separador de campos es `,` pero podria sea cualquiera, lo mismo para las lineas o cabecera..

string[][] LeerCSV(string origen, string separador = ";", string linea = "\n", bool cabecera = true){
    string texto = File.ReadAllText(origen);
    string lineas[] = string.Split(texto, linea);

    int saltar = cabecera ? 1 : 0;
    string[lineas.Length - saltar][] salida = new()[];
    
    for(var i = 0; i < salida.Length; i++){
        salida[i] = string.Split( lineas[i + salta], linea);
    }
    return salida;
}

