using System;       // Para usar la consola  (Console)
using System.IO;    // Para leer archivos    (File)

// Ayuda: 
//   Console.Clear() : Borra la pantalla
//   Console.Write(texto) : Escribe texto sin salto de línea
//   Console.WriteLine(texto) : Escribe texto con salto de línea
//   Console.ReadLine() : Lee una línea de texto
//   Console.ReadKey() : Lee una tecla presionada

// File.ReadLines(origen) : Lee todas las líneas de un archivo y devuelve una lista de strings
// File.WriteLines(destino, lineas) : Escribe una lista de líneas en un archivo

// Escribir la solucion al TP1 en este archivo. (Borre el ejemplo de abajo)
class Program 
{
    public struct Contacto
    {
        public int ID;
        public string Nombre;
        public string Telefono;
        public string Email;

        public Contacto(int id, string nombre, string telefono, string email)
        {
            ID = id;
            Nombre = nombre;
            Telefono = telefono;
            Email = email;
        }

        public void Mostrar()
        {
            Console.WriteLine($"ID: {ID} | Nombre: {Nombre} | Teléfono: {Telefono} | Email: {Email}");
        }
    }

    static Contacto[] agenda = new Contacto[100];
    static int contadorID = 1; 
    static int cantidadContactos = 0; 

    static void Main()
    {
        bool continuar = true;
        while (continuar)
        {
            Console.Clear();
            Console.WriteLine("Agenda de Contactos");
            Console.WriteLine("1. Agregar contacto");
            Console.WriteLine("2. Modificar contacto");
            Console.WriteLine("3. Borrar contacto");
            Console.WriteLine("4. Listar contactos");
            Console.WriteLine("5. Buscar contacto");
            Console.WriteLine("6. Salir");
            Console.Write("Seleccione una opción: ");

            string opcion = Console.ReadLine();

            if (opcion == "1") AgregarContacto();
            else if (opcion == "2") ModificarContacto();
            else if (opcion == "3") BorrarContacto();
            else if (opcion == "4") ListarContactos();
            else if (opcion == "5") BuscarContacto();
            else if (opcion == "6") continuar = false;
            else Console.WriteLine("Opción no válida.");
        }
    }

    static void AgregarContacto()
    {
        Console.Clear();
        Console.WriteLine("Agregar nuevo contacto");

        if (cantidadContactos >= agenda.Length)
        {
            Console.WriteLine("La agenda está llena.");
            Console.ReadKey();
            return;
        }

        Console.Write("Nombre: ");
        string nombre = Console.ReadLine();
        Console.Write("Teléfono: ");
        string telefono = Console.ReadLine();
        Console.Write("Email: ");
        string email = Console.ReadLine();

        agenda[cantidadContactos] = new Contacto(contadorID++, nombre, telefono, email);
        cantidadContactos++;

        Console.WriteLine("Contacto agregado con éxito!");
        Console.ReadKey();
    }

    static void ModificarContacto()
    {
        Console.Clear();
        Console.Write("Ingrese el ID del contacto a modificar: ");
        int id = int.Parse(Console.ReadLine());

        int index = -1;
        for (int i = 0; i < cantidadContactos; i++)
        {
            if (agenda[i].ID == id)
            {
                index = i;
                break;
            }
        }

        if (index == -1)
        {
            Console.WriteLine("No se encontró el contacto.");
        }
        else
        {
            Console.Write("Nuevo nombre: ");
            string nombre = Console.ReadLine();
            Console.Write("Nuevo teléfono: ");
            string telefono = Console.ReadLine();
            Console.Write("Nuevo email: ");
            string email = Console.ReadLine();

            agenda[index] = new Contacto(
                agenda[index].ID,
                string.IsNullOrEmpty(nombre) ? agenda[index].Nombre : nombre,
                string.IsNullOrEmpty(telefono) ? agenda[index].Telefono : telefono,
                string.IsNullOrEmpty(email) ? agenda[index].Email : email
            );

            Console.WriteLine("Contacto modificado con éxito!");
        }
        Console.ReadKey();
    }

    static void BorrarContacto()
    {
        Console.Clear();
        Console.Write("Ingrese el ID del contacto a borrar: ");
        int id = int.Parse(Console.ReadLine());

        int index = -1;
        for (int i = 0; i < cantidadContactos; i++)
        {
            if (agenda[i].ID == id)
            {
                index = i;
                break;
            }
        }

        if (index == -1)
        {
            Console.WriteLine("No se encontró el contacto.");
        }
        else
        {
            for (int i = index; i < cantidadContactos - 1; i++)
            {
                agenda[i] = agenda[i + 1];
            }
            cantidadContactos--;
            Console.WriteLine("Contacto borrado con éxito!");
        }
        Console.ReadKey();
    }

    static void ListarContactos()
    {
        Console.Clear();
        Console.WriteLine("Lista de contactos");

        if (cantidadContactos == 0)
        {
            Console.WriteLine("No hay contactos registrados.");
        }
        else
        {
            for (int i = 0; i < cantidadContactos; i++)
            {
                agenda[i].Mostrar();
            }
        }
        Console.ReadKey();
    }

    static void BuscarContacto()
    {
        Console.Clear();
        Console.Write("Ingrese el nombre o parte del nombre del contacto: ");
        string busqueda = Console.ReadLine().ToLower();

        bool encontrado = false;
        for (int i = 0; i < cantidadContactos; i++)
        {
            string nombreLower = "";
            for (int j = 0; j < agenda[i].Nombre.Length; j++)
            {
                nombreLower += char.ToLower(agenda[i].Nombre[j]);
            }
            
            if (nombreLower.Contains(busqueda))
            {
                agenda[i].Mostrar();
                encontrado = true;
            }
        }

        if (!encontrado)
        {
            Console.WriteLine("No se encontraron contactos con ese nombre.");
        }
        Console.ReadKey();
    }
}
