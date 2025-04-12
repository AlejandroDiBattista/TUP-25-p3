using System;
using System.IO;

struct RegistroContacto
{
    public int Identificador;
    public string NombreCompleto;
    public string NumeroTelefono;
    public string CorreoElectronico;
}

const int LimiteContactos = 100;
RegistroContacto[] listaContactos = new RegistroContacto[LimiteContactos];
int totalContactos = 0;
int idActual = 0;
const string archivoDatos = "agenda.csv";

CargarDatosDesdeArchivo();

while (true)
{
    Console.Clear();
    Console.WriteLine("===== AGENDA DE CONTACTOS =====");
    Console.WriteLine("1) Agregar contacto");
    Console.WriteLine("2) Modificar contacto");
    Console.WriteLine("3) Eliminar contacto");
    Console.WriteLine("4) Mostrar contactos");
    Console.WriteLine("5) Buscar contacto");
    Console.WriteLine("0) Salir");
    Console.Write("Seleccione una opción: ");
    string opcionMenu = Console.ReadLine();

    switch (opcionMenu)
    {
        case "1":
            AñadirContacto();
            break;
        case "2":
            EditarContacto();
            break;
        case "3":
            QuitarContacto();
            break;
        case "4":
            ListarContactos();
            break;
        case "5":
            LocalizarContacto();
            break;
        case "0":
            GuardarDatosEnArchivo();
            Console.WriteLine("Saliendo de la aplicación...");
            return;
        default:
            Console.WriteLine("Opción inválida. Presione una tecla para continuar...");
            Console.ReadKey();
            break;
    }
}

void CargarDatosDesdeArchivo()
{
    if (File.Exists(archivoDatos))
    {
        string[] lineasArchivo = File.ReadAllLines(archivoDatos);
        foreach (string linea in lineasArchivo)
        {
            string[] campos = linea.Split(',');
            if (campos.Length == 3 && totalContactos < LimiteContactos)
            {
                listaContactos[totalContactos] = new RegistroContacto
                {
                    Identificador = ++idActual,
                    NombreCompleto = campos[0],
                    NumeroTelefono = campos[1],
                    CorreoElectronico = campos[2]
                };
                totalContactos++;
            }
        }
    }
}

void GuardarDatosEnArchivo()
{
    string[] lineasArchivo = new string[totalContactos];
    for (int i = 0; i < totalContactos; i++)
    {
        lineasArchivo[i] = $"{listaContactos[i].NombreCompleto},{listaContactos[i].NumeroTelefono},{listaContactos[i].CorreoElectronico}";
    }
    File.WriteAllLines(archivoDatos, lineasArchivo);
}

void AñadirContacto()
{
    if (totalContactos >= LimiteContactos)
    {
        Console.WriteLine("La agenda está llena. No se pueden agregar más contactos.");
        Console.ReadKey();
        return;
    }

    Console.WriteLine("=== Agregar Contacto ===");
    Console.Write("Nombre   : ");
    string nombre = Console.ReadLine();
    Console.Write("Teléfono : ");
    string telefono = Console.ReadLine();
    Console.Write("Email    : ");
    string email = Console.ReadLine();

    listaContactos[totalContactos] = new RegistroContacto
    {
        Identificador = ++idActual,
        NombreCompleto = nombre,
        NumeroTelefono = telefono,
        CorreoElectronico = email
    };
    totalContactos++;

    Console.WriteLine($"Contacto agregado con ID = {idActual}");
    Console.WriteLine("Presione cualquier tecla para continuar...");
    Console.ReadKey();
}

void EditarContacto()
{
    Console.WriteLine("=== Modificar Contacto ===");
    Console.Write("Ingrese el ID del contacto a modificar: ");
    if (int.TryParse(Console.ReadLine(), out int idBusqueda))
    {
        for (int i = 0; i < totalContactos; i++)
        {
            if (listaContactos[i].Identificador == idBusqueda)
            {
                Console.WriteLine($"Datos actuales => Nombre: {listaContactos[i].NombreCompleto}, Teléfono: {listaContactos[i].NumeroTelefono}, Email: {listaContactos[i].CorreoElectronico}");
                Console.Write("(Deje el campo en blanco para no modificar)\n");

                Console.Write("Nombre    : ");
                string nombre = Console.ReadLine();
                Console.Write("Teléfono  : ");
                string telefono = Console.ReadLine();
                Console.Write("Email     : ");
                string email = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(nombre)) listaContactos[i].NombreCompleto = nombre;
                if (!string.IsNullOrWhiteSpace(telefono)) listaContactos[i].NumeroTelefono = telefono;
                if (!string.IsNullOrWhiteSpace(email)) listaContactos[i].CorreoElectronico = email;

                Console.WriteLine("Contacto modificado con éxito.");
                Console.ReadKey();
                return;
            }
        }
    }
    Console.WriteLine("ID no encontrado. Presione una tecla para continuar...");
    Console.ReadKey();
}

void QuitarContacto()
{
    Console.WriteLine("=== Borrar Contacto ===");
    Console.Write("Ingrese el ID del contacto a borrar: ");
    if (int.TryParse(Console.ReadLine(), out int idBusqueda))
    {
        for (int i = 0; i < totalContactos; i++)
        {
            if (listaContactos[i].Identificador == idBusqueda)
            {
                for (int j = i; j < totalContactos - 1; j++)
                {
                    listaContactos[j] = listaContactos[j + 1];
                }
                totalContactos--;
                Console.WriteLine($"Contacto con ID={idBusqueda} eliminado con éxito.");
                Console.ReadKey();
                return;
            }
        }
    }
    Console.WriteLine("ID no encontrado. Presione una tecla para continuar...");
    Console.ReadKey();
}

void ListarContactos()
{
    Console.WriteLine("=== Lista de Contactos ===");
    Console.WriteLine("ID    NOMBRE               TELÉFONO       EMAIL");
    for (int i = 0; i < totalContactos; i++)
    {
        Console.WriteLine($"{listaContactos[i].Identificador,-5} {listaContactos[i].NombreCompleto,-20} {listaContactos[i].NumeroTelefono,-15} {listaContactos[i].CorreoElectronico}");
    }
    Console.WriteLine("Presione cualquier tecla para continuar...");
    Console.ReadKey();
}

void LocalizarContacto()
{
    Console.WriteLine("=== Buscar Contacto ===");
    Console.Write("Ingrese un término de búsqueda (nombre, teléfono o email): ");
    string terminoBusqueda = Console.ReadLine()?.ToLower();

    Console.WriteLine("Resultados de la búsqueda:");
    Console.WriteLine("ID    NOMBRE               TELÉFONO       EMAIL");
    for (int i = 0; i < totalContactos; i++)
    {
        if (listaContactos[i].NombreCompleto.ToLower().Contains(terminoBusqueda) ||
            listaContactos[i].NumeroTelefono.ToLower().Contains(terminoBusqueda) ||
            listaContactos[i].CorreoElectronico.ToLower().Contains(terminoBusqueda))
        {
            Console.WriteLine($"{listaContactos[i].Identificador,-5} {listaContactos[i].NombreCompleto,-20} {listaContactos[i].NumeroTelefono,-15} {listaContactos[i].CorreoElectronico}");
        }
    }
    Console.WriteLine("Presione cualquier tecla para continuar...");
    Console.ReadKey();
}