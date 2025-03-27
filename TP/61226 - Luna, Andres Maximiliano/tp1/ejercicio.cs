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
        
        string opcion = Console.ReadLine();
        switch (opcion)
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
                return;
            default:
                Console.WriteLine("Opcion invalida, seleccione una correcta.");
                Console.ReadKey();
                mostrarMenu();
                break;
        }
        static void agregarContacto(){
            if (cantidadContactos <= 50){
                Console.WriteLine("No se pueden agregar mas contactos, la agenda esta llena. Presione una tecla para continuar.");
                Console.ReadKey();
                return;
            }
            
            Contacto nuevoContacto;            
            nuevoContacto.Id = cantidadContactos + 1;
            Console.WriteLine("Ingrese el nombre del contacto: ");
            nuevoContacto.Nombre = Console.ReadLine();
            Console.WriteLine("Ingrese el telefono del contacto: ");
            nuevoContacto.Telefono = Console.ReadLine();
            Console.WriteLine("Ingrese el email del contacto: ");
            nuevoContacto.Email = Console.ReadLine();

            contactos[cantidadContactos] = nuevoContacto;
            cantidadContactos++;
            Console.WriteLine("Contacto agregado correctamente. Presione una tecla para continuar.");
            Console.ReadKey();


    }
    static void modificarContacto(){
        Console.WriteLine("Ingrese el ID del contacto a modificar: ");
        int id = int.Parse(Console.ReadLine());
        if (id > cantidadContactos || id < 1){
            Console.WriteLine("ID invalido. Presione una tecla para continuar.");
            Console.ReadKey();
            return;
        }
        Contacto contacto = contactos[id - 1];
        Console.WriteLine("A continuacion se mostraran los campos a modificar, dejar en blanco si no se desea modificar: ");
        Console.WriteLine($"Nombre actual: {contacto.Nombre}");
        Console.WriteLine("Ingrese el nuevo nombre: ");
        contacto.Nombre = Console.ReadLine();
        Console.WriteLine($"Telefono actual: {contacto.Telefono}");
        Console.WriteLine("Ingrese el nuevo telefono: ");
        contacto.Telefono = Console.ReadLine();
        Console.WriteLine($"Email actual: {contacto.Email}");
        Console.WriteLine("Ingrese el nuevo email: ");
        contacto.Email = Console.ReadLine();

        contactos[id - 1] = contacto;
        Console.WriteLine("Contacto modificado correctamente. Presione una tecla para continuar.");
        Console.ReadKey();
        return;
        
    
    }
    static void eliminarContacto(){
                Console.WriteLine("Ingrese el ID del contacto a borrar: ");
                int id = int.Parse(Console.ReadLine());
                if (id > cantidadContactos || id < 1){
                Console.WriteLine("ID invalido. Presione una tecla para continuar.");
                Console.ReadKey();
                return;
            }
            for (int i = id - 1; i < cantidadContactos - 1; i++){
                contactos[i] = contactos[i + 1];
            }
            cantidadContactos--;
            Console.WriteLine("Contacto borrado correctamente. Presione una tecla para continuar.");
            Console.ReadKey();
            return;
    }
    static void listarContactos(){
        Console.WriteLine("Lista de contactos: ");
        for (int i = 0; i < cantidadContactos; i++){
            contactos[i].mostrarContacto();
        }
        Console.WriteLine("Presione una tecla para continuar.");
        Console.ReadKey();
        return;
    }
    static void buscarContacto(){
        Console.WriteLine("Ingrese el ID del contacto a buscar: ");
        int id = int.Parse(Console.ReadLine());
        if (id > cantidadContactos || id < 1){
            Console.WriteLine("ID invalido. Presione una tecla para continuar.");
            Console.ReadKey();
            return;
        }
        contactos[id - 1].mostrarContacto();
        Console.WriteLine("Presione una tecla para continuar.");
        Console.ReadKey();
        return;
    }
}
    }



