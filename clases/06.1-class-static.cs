using static System.Console;

/* Clase como contenedor de métodos */

// Hacemos una clase para facilitar el uso de la consola
// y evitar repetir código

// La clase Consola tiene métodos estáticos
// No se deben instanciar, solo se usan
// para agrupar métodos

static class Consola{
    public static void Limpiar(){
        Clear();
    }
    public static void Escribir(string mensaje){
        WriteLine(mensaje);
    }
    public static string Leer(string texto){
        Write(texto);
        return ReadLine();
    }
    public static void EsperarTecla(string mensaje = "Presione una tecla para continuar..."){
        Write(mensaje);
        ReadKey();
        WriteLine();
    }
}

// Ejemplo de uso de la clase Consola

Consola.Limpiar();

Consola.Escribir("Ingrese tu nombre: ");
var nombre   = Consola.Leer("  Nombre  :");
var apellido = Consola.Leer("  Apellido:");

Consola.Escribir($"Hola {apellido}, {nombre}");
Consola.EsperarTecla();

Consola.Limpiar();
