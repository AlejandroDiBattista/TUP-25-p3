using System;
using System.IO;
struct Contacto
    {
        public int ID;
        public string Nombre;
        public string Telefono;
        public string Email;
        public bool Mostrar;
    }

     string ruta = "contactos.csv";
        // Creamos un array (arreglo) para almacenar los contactos
        Contacto[] contactos = new Contacto[100];

        // Contador de cuántos contactos tenemos guardados(Id)
        int totalContactos = 0;

        // Cargamos contactos desde el archivo CSV al iniciar
        CargarCSV();

        while (true)
        {
            Console.WriteLine("\n===== AGENDA DE CONTACTOS =====");
            Console.WriteLine("1) Agregar contacto");
            Console.WriteLine("2) Modificar contacto");
            Console.WriteLine("3) Borrar contacto");
            Console.WriteLine("4) Listar contactos");
            Console.WriteLine("5) Buscar contacto");
            Console.WriteLine("6) Salir");

            Console.Write("Seleccione una opción: ");
            string opcion = Console.ReadLine();

            if (opcion == "1")
            {
                AgregarContacto();
            }
            else if (opcion == "2")
            {
                ModificarContacto();
            }
            else if (opcion == "3")
            {
                BorrarContacto();
            }
            else if (opcion == "4")
            {
                ListarContactos();
            }
            else if (opcion == "5")
            {
                BuscarContacto();
            }
            else if (opcion == "6")
            {
                GuardarCSV();
                break; // Rompe el while y finaliza el programa
            }
            else
            {
                Console.WriteLine("Opción inválida. Intente de nuevo.");
            }
        }

        void ModificarContacto()
        {
            Console.Write("Ingrese el ID del contacto a modificar: ");
            int idModificar = int.Parse(Console.ReadLine());

            for (int i = 0; i < totalContactos; i++)
            {
                if (contactos[i].ID == idModificar)
                {
                    Console.WriteLine($"Datos actuales => Nombre: {contactos[i].Nombre}, Teléfono : {contactos[i].Telefono}, Email: {contactos[i].Email}");
                    Console.Write("Nombre: " + contactos[i].Nombre);
                    string nuevoNombre = Console.ReadLine();
                    if ((contactos[i].Nombre == "" && nuevoNombre != "") || (contactos[i].Nombre != "" && nuevoNombre != contactos[i].Nombre && nuevoNombre != ""))
                    {
                        contactos[i].Nombre = nuevoNombre;
                    }

                    Console.Write("Teléfono: " + contactos[i].Telefono);
                    string nuevoTelefono = Console.ReadLine();
                    if ((contactos[i].Telefono == "" && nuevoTelefono != "") || (contactos[i].Telefono != "" && nuevoTelefono != contactos[i].Telefono && nuevoTelefono != ""))
                    {
                        contactos[i].Telefono = nuevoTelefono;
                    }

                    Console.Write("Email: " + contactos[i].Email);
                    string nuevoEmail = Console.ReadLine();
                    if (contactos[i].Email == "" && nuevoEmail != "" || (contactos[i].Email != "" && nuevoEmail != contactos[i].Email && nuevoEmail != ""))
                    {
                        contactos[i].Email = nuevoEmail;
                    }

                    Console.Write("El contacto fue modificado con éxito.");
                    break;
                }
            }
        }

        void AgregarContacto()
        {
            // Verificamos que no esté completo el array.
            if (totalContactos >= 100)
            {
                Console.WriteLine("No se pueden agregar más contactos. Agenda llena.");
                return;
            }

            Contacto nuevo = new Contacto();

            // Asignamos el ID
            nuevo.ID = totalContactos + 1;

            Console.Write("Ingrese el nombre: ");
            // Leemos el nombre que ingresa el usuario y se lo asignamos
            nuevo.Nombre = Console.ReadLine();

            Console.Write("Ingrese el teléfono: ");
            nuevo.Telefono = Console.ReadLine();

            Console.Write("Ingrese el email: ");
            nuevo.Email = Console.ReadLine();

            nuevo.Mostrar = true;

            // Guardamos el nuevo contacto en el array
            contactos[totalContactos] = nuevo;

            // incrementamos el "Id"
            totalContactos++;

            Console.WriteLine("Contacto agregado con ID " + nuevo.ID);
        }

        void BorrarContacto()
        {
            Console.Write("Ingrese el ID del contacto a eliminar: ");
            int idABorrar = int.Parse(Console.ReadLine());

            // Recorremos los contactos para encontrar ese ID
            for (int i = 0; i < totalContactos; i++)
            {
                if (contactos[i].ID == idABorrar)
                {
                    contactos[i].Mostrar = false;
                    Console.WriteLine("Contacto eliminado.");
                    return;
                }
            }
            Console.WriteLine("Contacto no encontrado.");
        }

        void ListarContactos()
        {
            Console.WriteLine("\n--- LISTA DE CONTACTOS ---");
            Console.WriteLine("ID   NOMBRE   TELÉFONO   EMAIL");
            Console.WriteLine("------------------------------");

            // Recorremos todos los contactos que tenemos guardados
            for (int i = 0; i < totalContactos; i++)
            {
                if (contactos[i].Mostrar == true)
                {
                    Console.WriteLine($"{contactos[i].ID}   {contactos[i].Nombre}   {contactos[i].Telefono}   {contactos[i].Email}");
                }
            }
        }

        void BuscarContacto()
        {
            Console.Write("Ingrese término de búsqueda: ");
            string termino = Console.ReadLine().ToLower();

            Console.WriteLine("\n--- RESULTADOS DE LA BÚSQUEDA ---");
            Console.WriteLine("ID   NOMBRE   TELÉFONO   EMAIL");
            Console.WriteLine("--------------------------------");

            for (int i = 0; i < totalContactos; i++)
            {
                // Buscamos que lo ingresado coincida en nombre, teléfono o email
                if (contactos[i].Nombre.ToLower().Contains(termino) ||
                    contactos[i].Telefono.Contains(termino) ||
                    contactos[i].Email.ToLower().Contains(termino))
                {
                    Console.WriteLine($"{contactos[i].ID}   {contactos[i].Nombre}   {contactos[i].Telefono}   {contactos[i].Email}");
                }
            }
        }

        void GuardarCSV()
        {
            if (!File.Exists(ruta))
            {
                var archivo = File.Create(ruta);
                // Le decimos a C# que nos libere el arhivo creado.
                archivo.Close();
                archivo.Dispose();
            }

            string contenido = "Nombre,Email,Telefono\n"; // encabezado

            for (int i = 0; i < totalContactos; i++)
            {
                contenido += $"{contactos[i].Nombre},{contactos[i].Email},{contactos[i].Telefono},{contactos[i].Mostrar}\n";
            }

            File.WriteAllText(ruta, contenido);
        }

        void CargarCSV()
        {
            // Verificamos si el archivo no existe
            if (!File.Exists(ruta))
            {
                return; // Si no existe, no hacemos nada
            }

            string contenido = File.ReadAllText(ruta);
            // Dividimos el contenido en líneas
            string[] lineas = contenido.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 1; i < lineas.Length; i++) // empezamos desde 1 para saltar el encabezado
            {
                string[] partes = lineas[i].Split(',');
                contactos[i - 1] = new Contacto
                {
                    ID = i,
                    Nombre = partes[0].Trim(),
                    Email = partes[1].Trim(),
                    Telefono = partes[2].Trim(),
                    Mostrar = bool.Parse(partes[3])
                };

                totalContactos++;
            }
        }