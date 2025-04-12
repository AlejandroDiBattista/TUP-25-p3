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
Console.ReadKey();using System;
using System.IO;

namespace AgendaDeContactos
{
    struct Contacto
    {
        public int Id;
        public string Nombre;
        public string Telefono;
        public string Email;
    }

    class Program
    {
        const int MAX_CONTACTOS = 100;
        static Contacto[] contactos = new Contacto[MAX_CONTACTOS];
        static int cantidad = 0; // número de contactos actuales
        static int siguienteId = 1; // ID incremental

        static void Main(string[] args)
        {
            // Cargar contactos desde archivo
            CargarContactos();

            bool salir = false;
            while (!salir)
            {
                Console.Clear();
                Console.WriteLine("===== Agenda de Contactos =====");
                Console.WriteLine("1. Agregar contacto");
                Console.WriteLine("2. Modificar contacto");
                Console.WriteLine("3. Borrar contacto");
                Console.WriteLine("4. Listar contactos");
                Console.WriteLine("5. Buscar contacto");
                Console.WriteLine("6. Salir");
                Console.Write("Elija una opción: ");
                string opcion = Console.ReadLine();

                switch (opcion)
                {
                    case "1":
                        AgregarContacto();
                        break;
                    case "2":
                        ModificarContacto();
                        break;
                    case "3":
                        BorrarContacto();
                        break;
                    case "4":
                        ListarContactos();
                        break;
                    case "5":
                        BuscarContacto();
                        break;
                    case "6":
                        salir = true;
                        break;
                    default:
                        Console.WriteLine("Opción inválida. Presione ENTER para continuar.");
                        Console.ReadLine();
                        break;
                }
            }

            // Guardar contactos en archivo
            GuardarContactos();
        }

        static void AgregarContacto()
        {
            if (cantidad >= MAX_CONTACTOS)
            {
                Console.WriteLine("Se alcanzó el límite de contactos.");
                Console.ReadLine();
                return;
            }

            Contacto nuevo;
            nuevo.Id = siguienteId++;
            Console.Write("Nombre: ");
            nuevo.Nombre = Console.ReadLine();
            Console.Write("Teléfono: ");
            nuevo.Telefono = Console.ReadLine();
            Console.Write("Email: ");
            nuevo.Email = Console.ReadLine();

            contactos[cantidad] = nuevo;
            cantidad++;

            Console.WriteLine("Contacto agregado correctamente. Presione ENTER para continuar.");
            Console.ReadLine();
        }

        static void ModificarContacto()
        {
            Console.Write("Ingrese el ID del contacto a modificar: ");
            int id;
            if (!int.TryParse(Console.ReadLine(), out id))
            {
                Console.WriteLine("ID inválido. Presione ENTER para continuar.");
                Console.ReadLine();
                return;
            }

            int pos = BuscarIndicePorId(id);
            if (pos == -1)
            {
                Console.WriteLine("Contacto no encontrado. Presione ENTER para continuar.");
                Console.ReadLine();
                return;
            }

            Console.WriteLine("Dejar en blanco para no modificar el campo.");
            Console.Write("Nuevo nombre (actual: {0}): ", contactos[pos].Nombre);
            string nombre = Console.ReadLine();
            if (!string.IsNullOrEmpty(nombre))
                contactos[pos].Nombre = nombre;

            Console.Write("Nuevo teléfono (actual: {0}): ", contactos[pos].Telefono);
            string telefono = Console.ReadLine();
            if (!string.IsNullOrEmpty(telefono))
                contactos[pos].Telefono = telefono;

            Console.Write("Nuevo email (actual: {0}): ", contactos[pos].Email);
            string email = Console.ReadLine();
            if (!string.IsNullOrEmpty(email))
                contactos[pos].Email = email;

            Console.WriteLine("Contacto modificado correctamente. Presione ENTER para continuar.");
            Console.ReadLine();
        }

        static void BorrarContacto()
        {
            Console.Write("Ingrese el ID del contacto a borrar: ");
            int id;
            if (!int.TryParse(Console.ReadLine(), out id))
            {
                Console.WriteLine("ID inválido. Presione ENTER para continuar.");
                Console.ReadLine();
                return;
            }

            int pos = BuscarIndicePorId(id);
            if (pos == -1)
            {
                Console.WriteLine("Contacto no encontrado. Presione ENTER para continuar.");
                Console.ReadLine();
                return;
            }

            // Mover todos los elementos siguientes un lugar hacia atrás
            for (int i = pos; i < cantidad - 1; i++)
            {
                contactos[i] = contactos[i + 1];
            }
            cantidad--;

            Console.WriteLine("Contacto borrado correctamente. Presione ENTER para continuar.");
            Console.ReadLine();
        }

        static void ListarContactos()
        {
            Console.WriteLine("===== Listado de Contactos =====");
            Console.WriteLine("{0,-5} {1,-20} {2,-15} {3,-30}", "ID", "Nombre", "Teléfono", "Email");
            for (int i = 0; i < cantidad; i++)
            {
                Console.WriteLine("{0,-5} {1,-20} {2,-15} {3,-30}",
                    contactos[i].Id, contactos[i].Nombre, contactos[i].Telefono, contactos[i].Email);
            }
            Console.WriteLine("Presione ENTER para continuar.");
            Console.ReadLine();
        }

        static void BuscarContacto()
        {
            Console.Write("Ingrese término de búsqueda: ");
            string termino = Console.ReadLine().ToLower();

            Console.WriteLine("===== Resultados de la búsqueda =====");
            Console.WriteLine("{0,-5} {1,-20} {2,-15} {3,-30}", "ID", "Nombre", "Teléfono", "Email");
            for (int i = 0; i < cantidad; i++)
            {
                if (contactos[i].Nombre.ToLower().Contains(termino) ||
                    contactos[i].Telefono.ToLower().Contains(termino) ||
                    contactos[i].Email.ToLower().Contains(termino))
                {
                    Console.WriteLine("{0,-5} {1,-20} {2,-15} {3,-30}",
                        contactos[i].Id, contactos[i].Nombre, contactos[i].Telefono, contactos[i].Email);
                }
            }
            Console.WriteLine("Presione ENTER para continuar.");
            Console.ReadLine();
        }

        static int BuscarIndicePorId(int id)
        {
            for (int i = 0; i < cantidad; i++)
            {
                if (contactos[i].Id == id)
                    return i;
            }
            return -1;
        }

        static void CargarContactos()
        {
            if (!File.Exists("agenda.csv"))
                return;

            try
            {
                string[] lineas = File.ReadAllLines("agenda.csv");
                for (int i = 0; i < lineas.Length; i++)
                {
                    // Cada línea debe tener formato: id;nombre;telefono;email
                    string[] datos = lineas[i].Split(';');
                    if (datos.Length >= 4)
                    {
                        Contacto c;
                        c.Id = int.Parse(datos[0]);
                        c.Nombre = datos[1];
                        c.Telefono = datos[2];
                        c.Email = datos[3];
                        contactos[cantidad] = c;
                        cantidad++;

                        // Actualiza el siguiente ID
                        if (c.Id >= siguienteId)
                            siguienteId = c.Id + 1;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al cargar los contactos: " + ex.Message);
                Console.WriteLine("Presione ENTER para continuar.");
                Console.ReadLine();
            }
        }

        static void GuardarContactos()
        {
            try
            {
                using (StreamWriter sw = new StreamWriter("agenda.csv"))
                {
                    for (int i = 0; i < cantidad; i++)
                    {
                        // Formato: id;nombre;telefono;email
                        sw.WriteLine("{0};{1};{2};{3}", contactos[i].Id, contactos[i].Nombre, contactos[i].Telefono, contactos[i].Email);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al guardar los contactos: " + ex.Message);
                Console.WriteLine("Presione ENTER para finalizar.");
                Console.ReadLine();
            }
        }
    }
}