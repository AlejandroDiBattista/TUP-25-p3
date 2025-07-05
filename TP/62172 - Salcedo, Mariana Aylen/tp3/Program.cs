using System;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Pruebas del TP3 - Lista Ordenada ===\n");
        
        // Prueba con enteros
        Console.WriteLine("--- Prueba con enteros ---");
        ListaOrdenada<int> listaInt = new ListaOrdenada<int>();
        
        listaInt.Agregar(5);
        listaInt.Agregar(2);
        listaInt.Agregar(8);
        listaInt.Agregar(1);
        listaInt.Agregar(5); // Duplicado
        
        Console.WriteLine($"Cantidad: {listaInt.Cantidad}");
        Console.WriteLine("Elementos:");
        for (int i = 0; i < listaInt.Cantidad; i++)
        {
            Console.WriteLine($"[{i}] = {listaInt[i]}");
        }
        
        Console.WriteLine($"¿Contiene 5? {listaInt.Contiene(5)}");
        Console.WriteLine($"¿Contiene 10? {listaInt.Contiene(10)}");
        
        // Filtrar números mayores a 3
        var mayoresA3 = listaInt.Filtrar(x => x > 3);
        Console.WriteLine($"Números > 3: {mayoresA3.Cantidad}");
        
        // Prueba con Contactos
        Console.WriteLine("\n--- Prueba con Contactos ---");
        ListaOrdenada<Contacto> listaContactos = new ListaOrdenada<Contacto>();
        
        listaContactos.Agregar(new Contacto("María", "123-456"));
        listaContactos.Agregar(new Contacto("Juan", "789-012"));
        listaContactos.Agregar(new Contacto("Ana", "345-678"));
        listaContactos.Agregar(new Contacto("Carlos", "901-234"));
        
        Console.WriteLine($"Cantidad de contactos: {listaContactos.Cantidad}");
        Console.WriteLine("Contactos ordenados:");
        for (int i = 0; i < listaContactos.Cantidad; i++)
        {
            Console.WriteLine($"[{i}] = {listaContactos[i]}");
        }
        
        // Filtrar contactos que empiecen con 'A'
        var contactosA = listaContactos.Filtrar(c => c.Nombre.StartsWith("A"));
        Console.WriteLine($"Contactos con 'A': {contactosA.Cantidad}");
        
        Console.WriteLine("\n=== Pruebas completadas ===");
    }
}
