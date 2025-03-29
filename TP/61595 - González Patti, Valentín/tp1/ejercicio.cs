using System;
using System.IO;

// Definición del struct para almacenar los datos de un contacto
struct Contacto
{
    public int Id;
    public string Nombre;
    public string Apellido;
    public string Telefono;
    public string Email;
}


        // Array para almacenar contactos (máximo 100)
        Contacto[] contactos = new Contacto[100];
        int contadorContactos = 0; // Número de contactos actuales
        string archivoCSV = "contactos.csv"; // Archivo CSV para persistencia

        // Cargar contactos desde el archivo CSV si existe
        if (File.Exists(archivoCSV))
        {
            string[] lineas = File.ReadAllLines(archivoCSV);
            for (int i = 0; i < lineas.Length; i++)
            {
                string[] campos = lineas[i].Split(',');
                contactos[i].Id = int.Parse(campos[0]); // ID desde el CSV
                contactos[i].Nombre = campos[1];
                contactos[i].Apellido = campos[2];
                contactos[i].Telefono = campos[3];
                contactos[i].Email = campos[4];
                contadorContactos++;
            }
        }

        // Bucle del menú
        bool salir = false;
        while (!salir)
        {
            Console.WriteLine("\n=== Menú de la Agenda ===");
            Console.WriteLine("1. Agregar contacto");
            Console.WriteLine("2. Eliminar contacto");
            Console.WriteLine("3. Buscar contacto");
            Console.WriteLine("4. Listar todos los contactos");
            Console.WriteLine("5. Salir");
            Console.Write("Elige una opción: ");
            string opcion = Console.ReadLine();

            // Opción 1: Agregar contacto
            if (opcion == "1")
            {
                if (contadorContactos < 100)
                {
                    Contacto nuevoContacto = new Contacto();
                    // Asignar ID incremental automáticamente
                    nuevoContacto.Id = contadorContactos == 0 ? 1 : contactos[contadorContactos - 1].Id + 1;

                    Console.Write("Nombre: ");
                    nuevoContacto.Nombre = Console.ReadLine();
                    Console.Write("Apellido: ");
                    nuevoContacto.Apellido = Console.ReadLine();
                    Console.Write("Teléfono: ");
                    nuevoContacto.Telefono = Console.ReadLine();
                    Console.Write("Email: ");
                    nuevoContacto.Email = Console.ReadLine();

                    // Guardar en el array
                    contactos[contadorContactos] = nuevoContacto;
                    contadorContactos++;
                    Console.WriteLine("Contacto agregado correctamente.");
                }
                else
                {
                    Console.WriteLine("Agenda llena. No se pueden agregar más contactos.");
                }
            }
            // Opción 2: Eliminar contacto
            else if (opcion == "2")
            {
                Console.Write("ID del contacto a eliminar: ");
                int idEliminar;
                if (!int.TryParse(Console.ReadLine(), out idEliminar))
                {
                    Console.WriteLine("ID inválido.");
                    continue;
                }

                bool encontrado = false;
                for (int i = 0; i < contadorContactos; i++)
                {
                    if (contactos[i].Id == idEliminar)
                    {
                        // Desplazar los contactos siguientes hacia arriba
                        for (int j = i; j < contadorContactos - 1; j++)
                        {
                            contactos[j] = contactos[j + 1];
                        }
                        contadorContactos--;
                        encontrado = true;
                        Console.WriteLine("Contacto eliminado.");
                        break;
                    }
                }
                if (!encontrado)
                {
                    Console.WriteLine("Contacto no encontrado.");
                }
            }
            // Opción 3: Buscar contacto
            else if (opcion == "3")
            {
                Console.Write("ID del contacto a buscar: ");
                int idBuscar;
                if (!int.TryParse(Console.ReadLine(), out idBuscar))
                {
                    Console.WriteLine("ID inválido.");
                    continue;
                }

                bool encontrado = false;
                for (int i = 0; i < contadorContactos; i++)
                {
                    if (contactos[i].Id == idBuscar)
                    {
                        Console.WriteLine($"ID: {contactos[i].Id}, Nombre: {contactos[i].Nombre}, Apellido: {contactos[i].Apellido}, Teléfono: {contactos[i].Telefono}, Email: {contactos[i].Email}");
                        encontrado = true;
                        break;
                    }
                }
                if (!encontrado)
                {
                    Console.WriteLine("Contacto no encontrado.");
                }
            }
            // Opción 4: Listar todos los contactos
            else if (opcion == "4")
            {
                if (contadorContactos > 0)
                {
                    // Encabezado de la tabla
                    Console.WriteLine("\n{0,-5} {1,-15} {2,-15} {3,-15} {4,-25}", "ID", "Nombre", "Apellido", "Teléfono", "Email");
                    Console.WriteLine(new string('-', 75)); // Línea separadora

                    // Mostrar contactos en formato de tabla
                    for (int i = 0; i < contadorContactos; i++)
                    {
                        Console.WriteLine("{0,-5} {1,-15} {2,-15} {3,-15} {4,-25}",
                            contactos[i].Id, contactos[i].Nombre, contactos[i].Apellido, contactos[i].Telefono, contactos[i].Email);
                    }
                }
                else
                {
                    Console.WriteLine("No hay contactos en la agenda.");
                }
            }
            // Opción 5: Salir y guardar en CSV
            else if (opcion == "5")
            {
                // Guardar contactos en el archivo CSV
                using (StreamWriter writer = new StreamWriter(archivoCSV))
                {
                    for (int i = 0; i < contadorContactos; i++)
                    {
                        writer.WriteLine($"{contactos[i].Id},{contactos[i].Nombre},{contactos[i].Apellido},{contactos[i].Telefono},{contactos[i].Email}");
                    }
                }
                salir = true;
                Console.WriteLine("Saliendo de la agenda...");
            }
            else
            {
                Console.WriteLine("Opción no válida. Intenta de nuevo.");
            }
        }
    