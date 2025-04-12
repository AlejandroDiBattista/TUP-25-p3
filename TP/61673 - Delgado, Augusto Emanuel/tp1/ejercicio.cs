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
Console.WriteLine("Hola, soy el ejercicio 1 del TP1 de la materia Programación 3");
Console.Write("Presionar una tecla para continuar...");
Console.ReadKey();

struct Contacto
{
    public int Id;
    public string Nombre;
    public string Telefono;
    public string Email;
}

class AgendaContactos
{
    const int MaxContactos = 100;
    static Contacto[] contactos = new Contacto[MaxContactos];
    static int contadorContactos = 0;

    static void Main()
    {
        CargarContactos();
        int opcion;
        do
        {
            WriteLine("\nMenú de Agenda de Contactos:");
            WriteLine("1. Agregar contacto");
            WriteLine("2. Modificar contacto");
            WriteLine("3. Borrar contacto");
            WriteLine("4. Listar contactos");
            WriteLine("5. Buscar contacto");
            WriteLine("6. Salir");
            Write("Selecciona una opción: ");
            opcion = int.Parse(ReadLine());

            switch (opcion)
            {
                case 1: AgregarContacto(); break;
                case 2: ModificarContacto(); break;
                case 3: BorrarContacto(); break;
                case 4: ListarContactos(); break;
                case 5: BuscarContacto(); break;
                case 6: GuardarContactos(); break;
                default: WriteLine("Opción inválida."); break;
            }
        } while (opcion != 6);
    }

    static void AgregarContacto()
    {
        if (contadorContactos >= MaxContactos)
        {
            WriteLine("La agenda está llena.");
            return;
        }

        Write("Nombre: ");
        string nombre = ReadLine();
        Write("Teléfono: ");
        string telefono = ReadLine();
        Write("Email: ");
        string email = ReadLine();

        contactos[contadorContactos] = new Contacto
        {
            Id = contadorContactos + 1,
            Nombre = nombre,
            Telefono = telefono,
            Email = email
        };
        contadorContactos++;
        WriteLine("Contacto agregado exitosamente.");
    }

    static void ModificarContacto()
    {
        Write("Ingresa el ID del contacto a modificar: ");
        int id = int.Parse(ReadLine());
        for (int i = 0; i < contadorContactos; i++)
        {
            if (contactos[i].Id == id)
            {
                Write("Nuevo nombre (o Enter para dejar sin cambios): ");
                string nombre = ReadLine();
                if (!string.IsNullOrEmpty(nombre)) contactos[i].Nombre = nombre;

                Write("Nuevo teléfono (o Enter para dejar sin cambios): ");
                string telefono = ReadLine();
                if (!string.IsNullOrEmpty(telefono)) contactos[i].Telefono = telefono;

                Write("Nuevo email (o Enter para dejar sin cambios): ");
                string email = ReadLine();
                if (!string.IsNullOrEmpty(email)) contactos[i].Email = email;

                WriteLine("Contacto modificado exitosamente.");
                return;
            }
        }
        WriteLine("Contacto no encontrado.");
    }

    static void BorrarContacto()
    {
        Write("Ingresa el ID del contacto a borrar: ");
        int id = int.Parse(ReadLine());
        for (int i = 0; i < contadorContactos; i++)
        {
            if (contactos[i].Id == id)
            {
                for (int j = i; j < contadorContactos - 1; j++)
                {
                    contactos[j] = contactos[j + 1];
                }
                contadorContactos--;
                WriteLine("Contacto eliminado exitosamente.");
                return;
            }
        }
        WriteLine("Contacto no encontrado.");
    }

    static void ListarContactos()
    {
        WriteLine("\n{0,-5} {1,-20} {2,-15} {3,-30}", "ID", "Nombre", "Teléfono", "Email");
        WriteLine(new string('-', 70));
        for (int i = 0; i < contadorContactos; i++)
        {
            WriteLine("{0,-5} {1,-20} {2,-15} {3,-30}", contactos[i].Id, contactos[i].Nombre, contactos[i].Telefono, contactos[i].Email);
        }
    }

    static void BuscarContacto()
    {
        Write("Ingresa un término de búsqueda: ");
        string termino = ReadLine().ToLower();
        WriteLine("\n{0,-5} {1,-20} {2,-15} {3,-30}", "ID", "Nombre", "Teléfono", "Email");
        WriteLine(new string('-', 70));
        for (int i = 0; i < contadorContactos; i++)
        {
            if (contactos[i].Nombre.ToLower().Contains(termino) ||
                contactos[i].Telefono.ToLower().Contains(termino) ||
                contactos[i].Email.ToLower().Contains(termino))
            {
                WriteLine("{0,-5} {1,-20} {2,-15} {3,-30}", contactos[i].Id, contactos[i].Nombre, contactos[i].Telefono, contactos[i].Email);
            }
        }
    }

    static void CargarContactos()
    {
        if (!File.Exists("agenda.csv")) return;

        string[] lineas = File.ReadAllLines("agenda.csv");
        for (int i = 0; i < lineas.Length; i++)
        {
            string[] datos = lineas[i].Split(',');
            contactos[contadorContactos] = new Contacto
            {
                Id = int.Parse(datos[0]),
                Nombre = datos[1],
                Telefono = datos[2],
                Email = datos[3]
            };
            contadorContactos++;
        }
    }

    static void GuardarContactos()
    {
        using (StreamWriter sw = new StreamWriter("agenda.csv"))
        {
            for (int i = 0; i < contadorContactos; i++)
            {
                sw.WriteLine("{0},{1},{2},{3}", contactos[i].Id, contactos[i].Nombre, contactos[i].Telefono, contactos[i].Email);
            }
        }
    }
}