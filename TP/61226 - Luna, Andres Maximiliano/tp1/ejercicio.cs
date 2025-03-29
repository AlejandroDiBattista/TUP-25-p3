using System;       // Para usar la consola  (Console)
using System.IO;
using System.Security.Cryptography.X509Certificates;    // Para leer archivos    (File)

// Ayuda: 
//   Console.Clear() : Borra la pantalla
//   Console.Write(texto) : Escribe texto sin salto de línea
//   Console.WriteLine(texto) : Escribe texto con salto de línea
//   Console.ReadLine() : Lee una línea de texto
//   Console.ReadKey() : Lee una tecla presionada

// File.ReadLines(origen) : Lee todas las líneas de un archivo y devuelve una lista de strings
// File.WriteLines(destino, lineas) : Escribe una lista de líneas en un archivo

// Escribir la solucion al TP1 en este archivo. (Borre el ejemplo de abajo)

struct Contacto
{
    public string Nombre;
    public int Id;
    public string Telefono;
    public string Email;
    public Contacto (string nombre, int id, string telefono, string email)
    {
        Nombre = nombre;
        Id = id;
        Telefono = telefono;
        Email = email;
    }

    public void mostrarContacto(){
        Console.WriteLine($"ID: {Id} Nombre: {Nombre} Telefono: {Telefono} Email: {Email}");
    }
}


class menuContactos
{
    static Contacto[] contactos = new Contacto[50];
    static int cantidadContactos = 0;
    static void mostrarMenu(){
        
        Console.Clear();
        Console.WriteLine("Mi agenda");
        Console.WriteLine("1. Agregar contacto");
        Console.WriteLine("2. Modificar contactos");
        Console.WriteLine("3. Borrar contacto");
        Console.WriteLine("4. Listar contactos");
        Console.WriteLine("5. Buscar contactos");
        Console.WriteLine("6. Salir");
        Console.WriteLine("------Seleccione una opcion------");
        
        string caso = Console.ReadLine() ?? string.Empty;
        switch (caso)
        {
            case "1":
                agregarContacto();
                break;
            case "2":
                modificarContacto();
                break;
            case "3":
                eliminarContacto();
                break;
            case "4":
                listarContactos();
                break;
            case "5":
                buscarContacto();
                break;
            case "6":
                salirAplicacion();
                return;
            default:
                Console.WriteLine("Opcion invalida, seleccione una correcta.");
                Console.ReadKey();
                mostrarMenu();
                break;
        }
        static void agregarContacto(){
            if (cantidadContactos <= 50){
                Contacto nuevoContacto;            
            nuevoContacto.Id = cantidadContactos + 1;
            Console.WriteLine("Ingrese el nombre del contacto: ");
            nuevoContacto.Nombre = Console.ReadLine() ?? string.Empty;
            Console.WriteLine("Ingrese el telefono del contacto: ");
            nuevoContacto.Telefono = Console.ReadLine() ?? string.Empty;
            Console.WriteLine("Ingrese el email del contacto: ");
            nuevoContacto.Email = Console.ReadLine() ?? string.Empty;

            contactos[cantidadContactos] = nuevoContacto;
            cantidadContactos++;
            Console.WriteLine("Contacto agregado correctamente. Presione una tecla para continuar.");
            Console.ReadKey();
            mostrarMenu();
            }else{
                Console.WriteLine("No se pueden agregar mas contactos. Presione una tecla para continuar.");
                Console.ReadKey();
                mostrarMenu();
            }
            
            


    }
    static void modificarContacto(){
        Console.WriteLine("Ingrese el ID del contacto a modificar: ");
        string? input = Console.ReadLine();
        if (string.IsNullOrEmpty(input) || !int.TryParse(input, out int id))
        {
            Console.WriteLine("Entrada inválida. Presione una tecla para continuar.");
            Console.ReadKey();
            Console.Clear();
            mostrarMenu();
            return;
        }
        if (id < 1 || id > cantidadContactos){
            Console.WriteLine("ID invalido. Presione una tecla para continuar.");
            Console.ReadKey();
            Console.Clear();
            mostrarMenu();
                        
        }
        Contacto contacto = contactos[id - 1];
        Console.WriteLine("A continuacion se mostraran los campos a modificar, dejar en blanco si no se desea modificar: ");
        Console.WriteLine($"Nombre actual: {contacto.Nombre}");
        Console.WriteLine("Ingrese el nuevo nombre: ");
        contacto.Nombre = Console.ReadLine() ?? string.Empty;
        Console.WriteLine($"Telefono actual: {contacto.Telefono}");
        Console.WriteLine("Ingrese el nuevo telefono: ");
        contacto.Telefono = Console.ReadLine() ?? string.Empty;
        Console.WriteLine($"Email actual: {contacto.Email}");
        Console.WriteLine("Ingrese el nuevo email: ");
        contacto.Email = Console.ReadLine() ?? string.Empty;

        contactos[id - 1] = contacto;
        Console.WriteLine("Contacto modificado correctamente. Presione una tecla para continuar.");
        Console.ReadKey();
        Console.Clear();
        mostrarMenu();
        
    
    }
    static void eliminarContacto(){
                Console.WriteLine("Ingrese el ID del contacto a borrar: ");
                string? input = Console.ReadLine();
                if (string.IsNullOrEmpty(input) || !int.TryParse(input, out int id))
                {
                    Console.WriteLine("Entrada inválida. Presione una tecla para continuar.");
                    Console.ReadKey();
                    Console.Clear();
                    mostrarMenu();
                    return;
                }
                if (id > cantidadContactos || id < 1){
                Console.WriteLine("ID invalido. Presione una tecla para continuar.");
                Console.ReadKey();
                Console.Clear();
                mostrarMenu();
                return;
            }
            for (int i = id - 1; i < cantidadContactos - 1; i++){
                contactos[i] = contactos[i + 1];
            }
            cantidadContactos--;
            Console.WriteLine("Contacto borrado correctamente. Presione una tecla para continuar.");
            Console.ReadKey();
            Console.Clear();
            mostrarMenu();
            return;
    }
    static void listarContactos(){
        Console.WriteLine("Lista de contactos: ");
        for (int i = 0; i < cantidadContactos; i++){
            contactos[i].mostrarContacto();
        }
        Console.WriteLine("Presione una tecla para continuar.");
        Console.ReadKey();
        Console.Clear();
        mostrarMenu();
        return;
    }
    static void buscarContacto(){
        Console.WriteLine("Ingrese el ID del contacto a buscar: ");
        string? input = Console.ReadLine();
        if (string.IsNullOrEmpty(input) || !int.TryParse(input, out int id))
        {
            Console.WriteLine("Entrada inválida. Presione una tecla para continuar.");
            Console.ReadKey();
            Console.Clear();
            mostrarMenu();
            return;
        }
        if (id > cantidadContactos || id < 1){
            Console.WriteLine("ID invalido. Presione una tecla para continuar.");
            Console.ReadKey();
            Console.Clear();
            mostrarMenu();
            return;
        }
        contactos[id - 1].mostrarContacto();
        Console.WriteLine("Presione una tecla para continuar.");
        Console.ReadKey();
        Console.Clear();
        mostrarMenu();
        return;
    }
}
static void salirAplicacion(){
    Console.WriteLine("Saliendo de la aplicacion...");
    return;
}

        static void Main(string[] args)
        {
            Console.Clear();
            Console.WriteLine("Bienvenido a la agenda de contactos.");
            Console.WriteLine("Presione una tecla para continuar.");
            Console.ReadKey();
            mostrarMenu();
        }
    }