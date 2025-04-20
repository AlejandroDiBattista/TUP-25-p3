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


class Program
{
    struct Contacto
    {
        public int id;
        public string nombre;
        public string telefono;
        public string email;
    }
    static Contacto[] agenda = new Contacto[10];
    static int CantidadContactos = 0;
    static int SiguienteId = 1;

    static void Main()
    {
        CargarAgendaDeArchivos();
        
        bool salir = false;
        while (!salir)
        {
            Console.Clear();
            Console.WriteLine("==AGENDA DE CONTACTO==");
            Console.WriteLine("1)Agregar contacto");
            Console.WriteLine("2)Modificar contacto");
            Console.WriteLine("3)Borrar contacto");
            Console.WriteLine("4)Listar contacto");
            Console.WriteLine("5)Buscar contacto");
            Console.WriteLine("0)Salir");
            Console.WriteLine("SELLECCIONE UNA OPCION:");

            String opcion = Console.ReadLine();

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
                ListarContacto();
                break;
                case "5":
                BuscarContacto();
                break;
                case "0":
                GuardarAgendaEnArchivo();
                salir = true;
                break;
                default:
                Console.WriteLine("opcion no valida");
                break;
            }
            if (!salir)
            {
                Console.WriteLine("PRESIONE UNA TECLA PARA CONTINUAR");
                Console.ReadKey();
            }
        }
        Console.WriteLine("SALIENDO DE LA APLICACION...");
    }

    static void AgregarContacto()
    {
        Console.Clear();
        Console.WriteLine("==AGREGAR CONTACTO==");

        if (CantidadContactos < agenda.Length)
        {
            Contacto nuevoContacto = new Contacto();
            nuevoContacto.id = SiguienteId++;

            Console.Write("Nombre :");
            nuevoContacto.nombre = Console.ReadLine();

            Console.Write("Telefono :");
            nuevoContacto.telefono = Console.ReadLine();

            Console.Write("Email :");
            nuevoContacto.email = Console.ReadLine();

            agenda[CantidadContactos] = nuevoContacto;
            CantidadContactos++;

            Console.WriteLine($"Contacto agregado con id = {nuevoContacto.id}");
        }
        else
        {
            Console.WriteLine("No se puede agregar mas contactos, el limite fue alcanzado");
        }
    }

    static void ListarContacto()
    {
        Console.Clear();
        Console.WriteLine("==LISTAR CONTACTOS==");

        if (CantidadContactos == 0)
        {
            Console.WriteLine("no hay contactos en la agenda");
        }
        else
        {
            Console.WriteLine($"{ "ID",-5}{ "NOMBRE",-15}{ "TELEFONO",-20}{ "EMAIL",-25}");
            for(int i = 0; i < CantidadContactos; i++ )
            {
                Console.WriteLine($"{agenda[i].id,-5}{agenda[i].nombre,-15}{agenda[i].telefono,-15}{agenda[i].email,-20}");
            }
        }
        Console.WriteLine("PRESIONE UNA TECLA PARA CONTINUAR...");
        Console.ReadKey();
    }

    static void BuscarContacto()
    {
        Console.Clear();
        Console.WriteLine("==BUSCAR CONTACTO==");

        Console.WriteLine("INGRESAR ALGUN TERMINO DE BUSQUEDA (NOMBRE,TELEFONO,EMAIL): ");
        string busqueda = Console.ReadLine().ToLower();

        bool encontrado = false;
        Console.WriteLine("RESULTADO DE LA BUSQUEDA:");
        Console.WriteLine($"{ "ID",-5 }{ "NOMBRE",-15}{ "TELEFONO",-20}{ "EMAIL", -25}");

        for (int i = 0; i < CantidadContactos; i++)
        {
            if (agenda[i].nombre.ToLower().Contains(busqueda) || agenda[i].telefono.ToLower().Contains(busqueda) || agenda[i].email.ToLower().Contains(busqueda))
            {
                Console.WriteLine($"{agenda[i].id,-5}{agenda[i].nombre,-15}{agenda[i].telefono,-15}{agenda[i].email,-25}");
                encontrado = true;
            }
        }
        if (!encontrado)
        {
            Console.WriteLine("no se a encontrado ningun resultado");
        }
        Console.WriteLine("PRESIONE CUALQUIER TECLA PARA CONTINUAR...");
        Console.ReadKey();
    }

    static void ModificarContacto()
    {
        Console.Clear();
        Console.WriteLine("==MODIFICAR CONTACTO==");
        Console.WriteLine("ingrese el Id del contacto que desea modificar: ");
        int id = int.Parse(Console.ReadLine());

        bool encontrado = false;
        for (int i = 0; i < CantidadContactos; i++)
        {
            if (agenda[i].id == id)
            {
                Console.WriteLine($"datos actuales => Nombre: {agenda[i].nombre}, telefono: {agenda[i].telefono}, Email: {agenda[i].email}");

                Console.WriteLine("nuevo nombre (dejar en blanco para no modificar): ");
                string nuevoNombre = Console.ReadLine();
                if(!string.IsNullOrEmpty(nuevoNombre)) agenda[i].nombre = nuevoNombre;

                Console.Write("Nuevo teléfono (deje en blanco para no modificar): ");
                string nuevoTelefono = Console.ReadLine();
                if (!string.IsNullOrEmpty(nuevoTelefono)) agenda[i].telefono = nuevoTelefono;

                Console.Write("Nuevo email (deje en blanco para no modificar): ");
                string nuevoEmail = Console.ReadLine();
                if (!string.IsNullOrEmpty(nuevoEmail)) agenda[i].email = nuevoEmail;

                Console.WriteLine("modificacion exitosa");
                encontrado = true;
                break;
            }
        }
        if (!encontrado)
        {
            Console.WriteLine("No se encontro ningun contacto con ese Id ");
        }
        Console.WriteLine("PRESIONE UNA TECLA CUALQUIERA PARA CONTINUAR...");
        Console.ReadKey();
    }

    static void BorrarContacto()
    {
        Console.Clear();
        Console.WriteLine("==BORRAR CONTACTO==");
        Console.WriteLine("Ingrese en Id que desea borrar: ");
        int id = int.Parse(Console.ReadLine());

        bool encontrado = false;
        for(int i = 0; i < CantidadContactos; i++)
        {
            if (agenda[i].id == id)
            {
                for (int j = i; j < CantidadContactos -1; j++)
                {
                    agenda[j] = agenda[j + 1];
                }
                CantidadContactos--;
                Console.WriteLine("Contacto eliminado con exito");
                encontrado = true;
                break;
            }
        }
        if (!encontrado)
        {
            Console.WriteLine("No se a encontrado un contcto con ese Id");
        }
        Console.WriteLine("PRESIONE UNA TECLA CUALQUIERA PARA CONTINUAR...");
        Console.ReadKey();
    }

    static void CargarAgendaDeArchivos()
    {
        if (File.Exists("agenda.csv"))
        {
            string[] lineas = File.ReadAllLines("agenda.csv");
            foreach (string linea in lineas)
            {
                string[] datos = linea.Split(',');
                Contacto contacto = new Contacto
                {
                    id = int.Parse(datos[0]),
                    nombre = datos[1],
                    telefono = datos[2],
                    email = datos[3]
                };
                agenda[CantidadContactos++] = contacto;
                SiguienteId = contacto.id + 1; 
            }
        }
    }

    static void GuardarAgendaEnArchivo()
    {
        using (StreamWriter archivo = new StreamWriter("agenda.csv"))
        {
            foreach (var contacto in agenda)
            {
                if (!string.IsNullOrEmpty(contacto.nombre)) 
                {
                    archivo.WriteLine($"{contacto.id},{contacto.nombre},{contacto.telefono},{contacto.email}");
                }
            }
        }
    }
}