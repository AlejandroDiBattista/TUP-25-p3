using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

struct Contacto
{
    public int Id;
    public string Nombre;
    public string Telefono;
    public string Email;
}

const string ArchivoAgenda = "agenda.csv";
const int MaxContactos = 10;
List<Contacto> agenda = new List<Contacto>();
int siguienteId = 1;

CargarContactosDesdeArchivo();

while (true)
{
    Console.WriteLine("===== AGENDA DE CONTACTOS =====");
    Console.WriteLine("1) Agregar contacto");
    Console.WriteLine("2) Modificar contacto");
    Console.WriteLine("3) Borrar contacto");
    Console.WriteLine("4) Listar contactos");
    Console.WriteLine("5) Buscar contacto");
    Console.WriteLine("0) Salir");
    Console.Write("Seleccione una opción: ");

    if (!int.TryParse(Console.ReadLine(), out int opcion)) continue;

    switch (opcion)
    {
        case 1: AgregarContacto(); break;
        case 2: ModificarContacto(); break;
        case 3: BorrarContacto(); break;
        case 4: ListarContactos(); break;
        case 5: BuscarContacto(); break;
        case 0: GuardarContactosEnArchivo(); return;
        default: Console.WriteLine("Opción inválida."); break;
    }
    Console.WriteLine("\nPresione cualquier tecla para volver al menú...");
    Console.ReadKey();
}

void CargarContactosDesdeArchivo()
{
    if (File.Exists(ArchivoAgenda))
    {
        foreach (var linea in File.ReadAllLines(ArchivoAgenda))
        {
            var datos = linea.Split(',');
            if (datos.Length == 4 && int.TryParse(datos[0], out int id))
            {
                agenda.Add(new Contacto { Id = id, Nombre = datos[1], Telefono = datos[2], Email = datos[3] });
                if (id >= siguienteId) siguienteId = id + 1;
            }
        }
    }
}

void GuardarContactosEnArchivo()
{
    using (StreamWriter sw = new StreamWriter(ArchivoAgenda))
    {
        foreach (var c in agenda)
        {
            sw.WriteLine($"{c.Id},{c.Nombre},{c.Telefono},{c.Email}");
        }
    }
    Console.WriteLine("Agenda guardada correctamente.");
}

void AgregarContacto()
{
    if (agenda.Count >= MaxContactos)
    {
        Console.WriteLine("La agenda está llena. No se pueden agregar más contactos.");
        return;
    }

    Contacto nuevo;
    nuevo.Id = siguienteId++;

    Console.Write("Nombre y Apellido : ");
    nuevo.Nombre = Console.ReadLine();
    if (!Regex.IsMatch(nuevo.Nombre, "^[A-Za-z]+( [A-Za-z]+)?$"))
    {
        Console.WriteLine("No ingreso correctamente el nombre y apellido.");
        return;
    }

    Console.Write("Teléfono : ");
    nuevo.Telefono = Console.ReadLine();
    if (!Regex.IsMatch(nuevo.Telefono, "^\\d+$"))
    {
        Console.WriteLine("No ingreso correctamente el numero de teléfono.");
        return;
    }

    Console.Write("Email : ");
    nuevo.Email = Console.ReadLine();
    if (!Regex.IsMatch(nuevo.Email, "^[^@\\s]+@[^@\\s]+\\.[^@\\s]+$"))
    {
        Console.WriteLine("No ingreso correctamente el email.");
        return;
    }

    agenda.Add(nuevo);
    Console.WriteLine("Contacto agregado con éxito.");
}

void ModificarContacto()
{
    Console.Write("Ingrese el ID del contacto a modificar: ");
    if (!int.TryParse(Console.ReadLine(), out int id)) return;

    int index = agenda.FindIndex(c => c.Id == id);
    if (index == -1) { Console.WriteLine("No se encontró un contacto con ese ID."); return; }

    Contacto contacto = agenda[index];
    Console.WriteLine($"Datos actuales => Nombre: {contacto.Nombre}, Teléfono: {contacto.Telefono}, Email: {contacto.Email}");
    Console.WriteLine("(Deje el campo en blanco para no modificar)");

    Console.Write("Nuevo Nombre: ");
    string nuevoNombre = Console.ReadLine();
    if (!string.IsNullOrEmpty(nuevoNombre) && Regex.IsMatch(nuevoNombre, "^[A-Za-z]+( [A-Za-z]+)?$"))
        contacto.Nombre = nuevoNombre;
    else if (!string.IsNullOrEmpty(nuevoNombre))
        Console.WriteLine("Dato inválido: El nombre solo puede contener letras y un espacio.");

    Console.Write("Nuevo Teléfono: ");
    string nuevoTelefono = Console.ReadLine();
    if (!string.IsNullOrEmpty(nuevoTelefono) && Regex.IsMatch(nuevoTelefono, "^\\d+$"))
        contacto.Telefono = nuevoTelefono;
    else if (!string.IsNullOrEmpty(nuevoTelefono))
        Console.WriteLine("Dato inválido: El teléfono solo puede contener números.");

    Console.Write("Nuevo Email: ");
    string nuevoEmail = Console.ReadLine();
    if (!string.IsNullOrEmpty(nuevoEmail) && Regex.IsMatch(nuevoEmail, "^[^@\\s]+@[^@\\s]+\\.[^@\\s]+$"))
        contacto.Email = nuevoEmail;
    else if (!string.IsNullOrEmpty(nuevoEmail))
        Console.WriteLine("Dato inválido: El email debe tener un formato válido.");

    agenda[index] = contacto;
    Console.WriteLine("Contacto modificado con éxito.");
}

void BorrarContacto()
{
    Console.Write("Ingrese el ID del contacto a borrar: ");
    if (!int.TryParse(Console.ReadLine(), out int id)) return;
    agenda.RemoveAll(c => c.Id == id);
    Console.WriteLine("Contacto eliminado.");
}

void ListarContactos()
{
    Console.WriteLine("\n=== Lista de Contactos ===");
    Console.WriteLine("| ID  | Nombre             | Teléfono       | Email              |");
    foreach (var c in agenda)
    {
        Console.WriteLine($" {c.Id,-3}  {c.Nombre,-17}  {c.Telefono,-13}  {c.Email,-18} ");
    }
}

void BuscarContacto()
{
    Console.WriteLine("\n=== Buscar Contacto ===");
    Console.Write("Ingrese un término de búsqueda (nombre, teléfono o email): ");
    string termino = Console.ReadLine().ToLower();
    Console.WriteLine("\nResultados de la búsqueda:");
    Console.WriteLine("| ID  | Nombre             | Teléfono       | Email              |");

    for (int i = 0; i < agenda.Count; i++)
    {
        var c = agenda[i];
        if (c.Nombre.ToLower().Contains(termino) || c.Telefono.ToLower().Contains(termino) || c.Email.ToLower().Contains(termino))
        {
            Console.WriteLine($" {c.Id,-3}  {c.Nombre,-17}  {c.Telefono,-13}  {c.Email,-18} ");
        }
    }
}