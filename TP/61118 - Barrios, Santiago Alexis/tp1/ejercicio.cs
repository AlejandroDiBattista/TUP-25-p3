using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Contacto
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Telefono { get; set; }
    public string Email { get; set; }
}

public class Agenda
{
    private Dictionary<int, Contacto> contactos = new Dictionary<int, Contacto>();
    private int siguienteId = 1;
    private ArchivoManager archivoManager = new ArchivoManager("agenda.csv");

    public Agenda()
    {
        CargarContactos();
    }

    public void AgregarContacto()
    {
        Console.Write("Nombre: ");
        string nombre = Console.ReadLine()?.Trim();
        Console.Write("Teléfono: ");
        string telefono = Console.ReadLine()?.Trim();
        Console.Write("Email: ");
        string email = Console.ReadLine()?.Trim();

        if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(telefono))
        {
            Console.WriteLine("Nombre y Teléfono son obligatorios.");
            return;
        }

        var nuevoContacto = new Contacto { Id = siguienteId++, Nombre = nombre, Telefono = telefono, Email = email };
        contactos[nuevoContacto.Id] = nuevoContacto;
        Console.WriteLine("Contacto agregado correctamente.");
    }

    public void ModificarContacto()
    {
        int id = SolicitarId("Ingrese el ID del contacto a modificar: ");
        if (!contactos.ContainsKey(id))
        {
            Console.WriteLine("Contacto no encontrado.");
            return;
        }

        var contacto = contactos[id];
        Console.Write($"Nuevo nombre ({contacto.Nombre}): ");
        string nombre = Console.ReadLine()?.Trim();
        Console.Write($"Nuevo teléfono ({contacto.Telefono}): ");
        string telefono = Console.ReadLine()?.Trim();
        Console.Write($"Nuevo email ({contacto.Email}): ");
        string email = Console.ReadLine()?.Trim();

        if (!string.IsNullOrWhiteSpace(nombre)) contacto.Nombre = nombre;
        if (!string.IsNullOrWhiteSpace(telefono)) contacto.Telefono = telefono;
        if (!string.IsNullOrWhiteSpace(email)) contacto.Email = email;

        Console.WriteLine("Contacto modificado correctamente.");
    }

    public void BorrarContacto()
    {
        int id = SolicitarId("Ingrese el ID del contacto a eliminar: ");
        if (contactos.Remove(id))
        {
            Console.WriteLine("Contacto eliminado correctamente.");
        }
        else
        {
            Console.WriteLine("ID no encontrado.");
        }
    }

    public void ListarContactos()
    {
        Console.WriteLine("ID   Nombre                 Teléfono        Email");
        Console.WriteLine("--------------------------------------------------");
        foreach (var c in contactos.Values)
        {
            Console.WriteLine($"{c.Id,-4} {c.Nombre,-20} {c.Telefono,-15} {c.Email,-25}");
        }
    }

    public void BuscarContacto()
    {
        Console.Write("Ingrese término de búsqueda: ");
        string termino = Console.ReadLine()?.ToLower();
        var resultados = contactos.Values.Where(c => c.Nombre.ToLower().Contains(termino) ||
                                                     c.Telefono.ToLower().Contains(termino) ||
                                                     (c.Email != null && c.Email.ToLower().Contains(termino))).ToList();

        Console.WriteLine("ID   Nombre                 Teléfono        Email");
        Console.WriteLine("--------------------------------------------------");
        foreach (var c in resultados)
        {
            Console.WriteLine($"{c.Id,-4} {c.Nombre,-20} {c.Telefono,-15} {c.Email,-25}");
        }
    }

    public void CargarContactos()
    {
        var datos = archivoManager.Cargar();
        foreach (var c in datos)
        {
            contactos[c.Id] = c;
            siguienteId = Math.Max(siguienteId, c.Id + 1);
        }
    }

    public void GuardarContactos()
    {
        archivoManager.Guardar(contactos.Values.ToList());
    }

    private int SolicitarId(string mensaje)
    {
        Console.Write(mensaje);
        return int.TryParse(Console.ReadLine(), out int id) ? id : -1;
    }
}

public class ArchivoManager
{
    private readonly string archivo;

    public ArchivoManager(string archivo)
    {
        this.archivo = archivo;
    }

    public List<Contacto> Cargar()
    {
        List<Contacto> contactos = new List<Contacto>();

        if (!File.Exists(archivo))
            return contactos;

        foreach (var linea in File.ReadAllLines(archivo))
        {
            var datos = linea.Split(',');
            if (datos.Length == 4 && int.TryParse(datos[0], out int id))
            {
                contactos.Add(new Contacto
                {
                    Id = id,
                    Nombre = datos[1],
                    Telefono = datos[2],
                    Email = datos[3]
                });
            }
        }

        return contactos;
    }

    public void Guardar(List<Contacto> contactos)
    {
        File.WriteAllLines(archivo, contactos.Select(c => $"{c.Id},{c.Nombre},{c.Telefono},{c.Email}"));
    }
}

// Controla el menú del programa
class Menu
{
    private Agenda agenda = new Agenda();

    public void Mostrar()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("MENU");
            Console.WriteLine("1. Agregar contacto");
            Console.WriteLine("2. Modificar contacto");
            Console.WriteLine("3. Borrar contacto");
            Console.WriteLine("4. Listar contactos");
            Console.WriteLine("5. Buscar contacto");
            Console.WriteLine("6. Guardar y salir");
            Console.Write("Seleccione una opción: ");

            switch (Console.ReadLine())
            {
                case "1": agenda.AgregarContacto(); break;
                case "2": agenda.ModificarContacto(); break;
                case "3": agenda.BorrarContacto(); break;
                case "4": agenda.ListarContactos(); break;
                case "5": agenda.BuscarContacto(); break;
                case "6": agenda.GuardarContactos(); return;
                default: Console.WriteLine("Opción no válida."); break;
            }

            Console.WriteLine("\nPresione ENTER para continuar...");
            Console.ReadLine();
        }
    }
}

class Program
{
    static void Main()
    {
        new Menu().Mostrar();
    }
}
