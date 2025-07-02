
// Mostrame cual es el menor de 2 numeros.

int a = 10, b = 20;

if(a < b)
    WriteLine(a);
else
    WriteLine(b);

// El if acepta una sentencia para el caso de que la condicion sea verdadera y otra para cuando es falsa.
// Si requiere mas de una sentencias se debe usar {} para crear un bloque.

// Los bloques se comportan como si fuera una unica sentencia. 
// El ";" actua como terminador de una sentencia... no se usa despues de un bloque.

// La expresion anterior es equivalente a esta...
if(a < b) {
    WriteLine(a);
} else {
    WriteLine(b);
}
// Aunque las {} no son necesaria en este caso (ya que es una sola sentencia) se suelen poner 
// para mayor claridad

// Aunque muchas veces se ve asi...
if( a < b )
{
    WriteLine(a);
}
else
{
    WriteLine(b);
}

// Yo prefiero la 2da forma, aunque en la documentacion se suele usar la 3ra. 

// La escencia de la computacion esta en procesar los datos, es decir en tomar decisiones sobre los mismos.
// El if es la sentecia esencial. 

// Veamos como la solucion directa (mostrar el numero menor) se complejiza rapidamente.

// Supongamos que tenemos 3 variables
var c = 5;

if( a < b ){
    if( a < c) { // a < b & a < c               -> a es e menor
        WriteLine(a);
    } else { // a < b & c <= a -> c < a < b     -> c es el menor
        WriteLine(c);
    }
} else { // b < a 
    if( b < c ){ // b < a & b < c               -> b es el menor
        WriteLine(b);
    } else { // b < a & c < b -> c < b < a      -> c es el menor 
        WriteLine(c);
    }
}

// Aca podemos observar que lo complejidad aumenta rapidamente.
// Con 2 variables teniamos 2 caminos posibles (a < b o b < a).
// Con 3 variables tenemos 4 caminos posibles (2^2).
// Con 4 variables tenemos 8 caminos posibles (2^3).
// Con n variables tenemos 2^(n-1) caminos posibles. (Crece exponencialmente)

// Una forma alternativa de resolver el problema es probar las combinaciones de los 3 numeros

if( a < b && a < c) {
    WriteLine(a);
} else if( b < a && b < c) {
    WriteLine(b);
} else {
    WriteLine(c);
}

// O incluso una forma mas explicita 
if( a < b && a < c) WriteLine(a);
if( b < a && b < c) WriteLine(b);
if( c < a && c < b) WriteLine(c);

// En esta caso realizamos la comparacion completa. 

// Si tenemos 4 variables, la cantidad de caminos posibles es 8 (2^3).
var d = 7;
// Con if anidados

if( a < b ) {
    if( a < c ) {
        if( a < d ) {
            WriteLine(a);   // Caso 1
        } else {
            WriteLine(d);   // Caso 2
        }
    } else {
        if( c < d ) {
            WriteLine(c); // Caso 3
        } else {
            WriteLine(d); // Caso 4
        }
    }
} else { // b < a
    if( b < c ) {
        if( b < d ) {
            WriteLine(b); // Caso 5
        } else {
            WriteLine(d); // Caso 6
        }
    } else {
        if( c < d ) {
            WriteLine(c); // Caso 7
        } else {
            WriteLine(d); // Caso 8
        }
    }
}

// Si usamos la comparacion completa

if( a < b && a < c && a < d) WriteLine(a); // Caso 1
if( b < a && b < c && b < d) WriteLine(b); // Caso 2
if( c < a && c < b && c < d) WriteLine(c); // Caso 3
if( d < a && d < b && d < c) WriteLine(d); // Caso 4

// Ahora tenemos menos caminos posibles (4) pero la cantidad de comparaciones es la misma (3).
// Ya no crece exponencialmente, ahora crece linealmente los caminos posibles.
// Pero la cantidad de comparaciones en cada if crece lineamente tambien. 
// en este caso la cantidad de comparaciones va a ser 12 (3 * 4).
// En general para n numeros vamos a tener n*(n-1) comparaciones. 
// Sigue creciendo (mucho menos) 

// El problema es que el algorito original no esta bien definido.
// Tiene dos problemas:
// 1. Hacer dos cosas a la vez (comparar y mostrar el menor).
// 2. Tienen mucho codigo repetido 

// Supongamos que ahora queremes que muestre el mensaje "El menor es: " + menor
// Deberiamos ir a modificar el mensaje en todos lados.

// Una solucion es separar las dos cosas tareas.

var menor; // Inicializo el menor con el primer numero
if( a < b ) {
    menor = a;
} else {
    menor = b;
}
WriteLine($"El menor es: {menor}");
// ahora, gracias a la variable auxilar 'menor' podemos mostrar el menor en un solo lugar.
// Pero el problema sigue estando, ya que si tenemos 4 variables, el algoritmo sigue siendo muy largo.

// Cambiemos ligeramente el enfoque.


var min = a;

if( b < min ) min = b;
if( c < min ) min = c;
if( d < min ) min = d;
if( e < min ) min = e;

WriteLine($"El menor es: {min}");

// Si asumimos que el menor es `a` y que queda en 'menor' el valor ahora podemos `acumular` el resultado.
// La cantidad de comparaciones crece en forma lineal,  

// Incluso podemos generalizar la idea y aplicarla array 
var num = new int[5] { 10, 20, 5, 7, 15 };

var menor = num[0];  // Asume que el primer elementos es el menor
for( int i = 1; i < num.Length; i++ ) { // Empieza desde el segundo elemento
    if( num[i] < menor ) menor = num[i];
}
WriteLine($"El menor es: {menor}");

// Este codigo es mas simple y general pero puede contener error (si paso un array con 0 elementos???)

menor = int.MaxValue;  // Asumimos el mayor numero posible 
for(var i = 0; i < num.Length; i++ ) {
    if( num[i] < menor ) 
        menor = num[i];
}
WriteLine($"El menor es: {menor}");

// Este codigo es mas robusto, ya que si el array tiene 0 elementos, el menor va a ser el mayor posible.
// Pero aun tiene problemas... observe que usamos 2 veces num[i];

menor = int.MaxValue;
for(var i = 0; i <= num.Length; i++){
    var x = num[i];
    if(x < menor)
        menor = x;
}
WriteLine($"El menor es {menor}");

// Esta patron de uso (recorrer un array y trabajar con cada uno de los elementos)
// que c# implemento una forma especifica 
menor = int.MaxValue;
foreach(var x in num)
    if( x < min )
        min = x;
WriteLine($"El menor es {min}");

// lo mismo podriamos usar par encontrar el mayor...
mayor = int.MinValue; // Esto me asegura que todos los valores sean mayor al inicial
                      // Aunque podriamos usar p.e mayor = 0 si sabemos que todos los vales son positivos.

foreach( var x in mayor )
    if(x > mayor ) mayor = x;

// O para sumar...

var suma = 0;
foreach( var x in num )
    suma += x;

// o para contar...
var cuenta = 0;
foreach(var x in num)
    cuenta += 1;

WriteLine($"Hay {cuenta} elementos");
// O para contar los que cumplan una condicion p.e. Contar los numeros mayores a 10

cuenta = 0;
foreach( var x in num )
    if( x > 10 ) 
        contar++;
WriteLine($"Hay {cuenta} numeros > 10");

// Un detalle interesante es que foreach funciona para cualquier coleccion no solo los array !!

// Otra forma de repetir valores en con while(<condicion>) <sentencia>

menor = int.MaxValue;
var i = 0;              // Inicializacion
while(i < num.Length){  // Condicion
    var x = num[i];
    if(x < menor)
        menor = 0;
    i++;                // Incremento
}

// for(<inicializacion>;<condicion>;<incremento>)
//    <sentencia>

// Todos los valores son opciones pero los ";" deben estar.
for(;;){
    // Ejecutar por siempre.
}

var i = 0;
for( ; i<num.Length ; ) // Se comporta como un while!
    WriteLine(num[i++]);

// Una interesante es el operador "," que permite poner varias declaraciones juntas.
// var a = 10, b = 20 es un ejemplo.

// p.e. si quemos mostar pares de numeros donde uno crece y el otro decrece...

for(int i=1, j=10; i < 10; i++, j--)
    WriteLine($"Sube Baja {i} {j}");

// Existe otros dos elementos (que se puede usar en while, for o foreach) que tambien me permite 
// controlar el flujo: continue y break

// p.e Si tengo n numeros y quiero sumar los menores a 10 hasta que suma 100

var suma = 0;
foreach(var x in num){
    if(x >= 0) continue;
    suma += x;
    if(suma > 100) break;
}

// Aca `continue` vuelva al comenzar el bucle (salta el resto del bloque) y 
// `break` da por terminado el bucle (aunque todavia no alla recorrido toda la coleccion)

// Y como un `foreach` es un `for` simplifcado y a la vez el `for` es una forma compacta del `while`...
// `for` y `while` tambien soporta `continue` y `break`

// Hay otra estructura de control especializada 

// supongamos que tenemos la posicion de una vocal y queremos conocer la vocal...

var posicion = 2;
string vocal = "";

if( posicion == 1 ){
    vocal = "A";
} else if( posicion == 2){
    vocal = "E";
} else if( posicion == 3){
    vocal = "I";
} else if( posicion == 4){
    vocal = "O";
} else if( posicion == 5){
    vocal = "U"
} else {
    vocal = "?";
}

// Implementado con `if` "encadenados"...
// como en esta caso la opciones son excluyente se puede incluso hacer con una "secuencia" de `if`

vocal = "?";
if(posicion == 1) vocal = "A";
if(posicion == 2) vocal = "E";
if(posicion == 3) vocal = "I";
if(posicion == 4) vocal = "O";
if(posicion == 5) voval = "U";

// La primera tiene la ventaja de que bien encuentra el valor no sigue comparando
// En la segunda compara siempre aunque ya tenga el resultado.

// De nuevo... simplicidad versus eficiencia...
// para este clase de problemas se usa switch 

switch(p){
    case 1: 
        vocal = "A";
        break;
    case 2: 
        vocal = "E";
        break;
    case 3:
        vocal = "I";
        break;
    case 4:
        vocal = "O";
        break;
    case 5:
        vocal = "U";
        break;
    default: 
        vocal = "?";
}

// Dos cosas raras que hay que observar...
// 1. yo hace falta el {} aunque tenga dos sentencias vocal = "A"; break;
// 2. Que debe llevar `break`;

// Esto es por razones historicas y lo que pretendia es que se pueda usar como if encadendados o if secuenciale.
// En la practica siempre va el break.. a no ser que se use un return (cuando esta dentro de una funcion)

// Por supuesto dentro del `case` puede ponerse cualquier cantidad de codigo...
// pero para el caso especial (que es el mas frecuente) que solo se quiera asignar una variable hay un forma compacta

var vocal = posicion switch {
    1 => "A",
    2 => "E",
    3 => "I",
    4 => "O",
    5 => "U",
    _ => "?"  // Observe que `_` tiene un uso especial... es el `default`
}

vocal = posicion switch{ 1 => "A", 2 => "E", 3 => "I", 4 => "O", 5 => "U", _ => "?" }
// Esto es usar un switch como expresion (en cualquier lugar donde se espera un valor)

// Algo parecido existe para el `if` `else`
// para el caso especial que se usa el if para asignar una variable...

if( a < b)          // Condicion
    menor = a;      // valor para true
else
    menor = b;      // valor para false

menor = a < b ? a : b; 
// <condicion> ? <valorParaTrue> : <valorParaFalse>;

// Determinar el menor de 5 numeros...

menor = a;
menor = b < menor ? b : menor;
menor = c < menor ? c : menor;
menor = d < menor ? d : menor;
menor = e < menor ? e : menor;

// El operador ternario (asi se lo conoce) es un 'if' que puede ser usando como expresion 


// Este if anidados se podrias convertir...
if( a < b ){
    if( a < c) {
        menor = a;
    } else { 
        menor = c;
    }
} else { 
    if( b < c ){
        menor = b;
    } else { 
        menor = c;
    }
}

// Primero los de adentro...
if( a < b ){
    menor = a < c ? a : c;
} else {
    menor = b < c ? b : c;
}

// y despues los de afuera...

// menor = a < b ? (...) : (...)
menor = a < b ? ( a < c ? a : c) : ( b < c ? b : c);

// Esto es mucho mas compacto... (y para algunas mas facil de entender...)

