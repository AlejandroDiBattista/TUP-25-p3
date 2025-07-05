using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Contacto {
    public int Id { get; }
    public string Nombre { get; set; }
    public string Telefono { get; set; }
    public string Email { get; set; }

    public Contacto(int id, string nombre, string telefono, string email) {
        Id = id;
        Nombre = nombre;
        Telefono = telefono;
        Email = email;
    }

    public override string ToString() => $"Id: {Id}, Nombre: {Nombre}, Teléfono: {Telefono}, Email: {Email}";
}

public class Registros {
    private List<Contacto> listaContactos = new List<Contacto>();
    private string rutaArchivo = "agenda.csv";
    private int ultimoId = 0;

    public Registros() {
        CargarContactos();
    }

    private void CargarContactos() {
        if (File.Exists(rutaArchivo)) {
            foreach (var linea in File.ReadAllLines(rutaArchivo)) {
                var datos = linea.Split(',');
                if (datos.Length == 4 && int.TryParse(datos[0], out int id)) {
                    listaContactos.Add(new Contacto(id, datos[1], datos[2], datos[3]));
                    ultimoId = Math.Max(ultimoId, id);
                }
            }
        }
    }

    private void GuardarContactos() {
        var lineas = listaContactos.Select(c => $"{c.Id},{c.Nombre},{c.Telefono},{c.Email}");
        File.WriteAllLines(rutaArchivo, lineas);
    }

    public void AgregarContacto() {
        Console.Write("Nombre: ");
        string nombre = Console.ReadLine();
        Console.Write("Teléfono: ");
        string telefono = Console.ReadLine();
        Console.Write("Email: ");
        string email = Console.ReadLine();

        var nuevoContacto = new Contacto(++ultimoId, nombre, telefono, email);
        listaContactos.Add(nuevoContacto);
        GuardarContactos();
    }

    public void MostrarContactos() {
        if (listaContactos.Any()) {
            listaContactos.ForEach(c => Console.WriteLine(c));
        } else {
            Console.WriteLine("No hay contactos.");
        }
    }

    public void ModificarContacto() {
        Console.Write("ID del contacto a modificar: ");
        if (int.TryParse(Console.ReadLine(), out int id)) {
            var contacto = listaContactos.FirstOrDefault(c => c.Id == id);
            if (contacto != null) {
                Console.Write($"Nuevo nombre (actual: {contacto.Nombre}): ");
                contacto.Nombre = Console.ReadLine() ?? contacto.Nombre;
                Console.Write($"Nuevo teléfono (actual: {contacto.Telefono}): ");
                contacto.Telefono = Console.ReadLine() ?? contacto.Telefono;
                Console.Write($"Nuevo email (actual: {contacto.Email}): ");
                contacto.Email = Console.ReadLine() ?? contacto.Email;

                GuardarContactos();
                Console.WriteLine("Contacto modificado.");
            } else {
                Console.WriteLine("Contacto no encontrado.");
            }
        } else {
            Console.WriteLine("ID inválido.");
        }
    }

    public void Buscarcontacto() {
        Console.WriteLine("Ingresa el id a buscar:");
        int id = int.Parse(Console.ReadLine());

        // Usamos un for para recorrer la lista de contactos
        bool encontrado = false;
        for (int i = 0; i < listaContactos.Count; i++) {
            if (listaContactos[i].Id == id) {
                Console.WriteLine($"Contacto encontrado: {listaContactos[i].Nombre}, {listaContactos[i].Telefono}, {listaContactos[i].Email}");
                encontrado = true;
                break;
            }
        }
        if (!encontrado) {
            Console.WriteLine("Contacto no encontrado.");
        }
    }

    public void EliminarContacto() {
        Console.WriteLine("Ingresa el id a eliminar:");
        int id = int.Parse(Console.ReadLine());

        bool encontrado = false;
        for (int i = 0; i < listaContactos.Count; i++) {
            if (listaContactos[i].Id == id) {
                listaContactos.RemoveAt(i);
                GuardarContactos();
                Console.WriteLine("Contacto eliminado.");
                encontrado = true;
                break;
            }
        }
        if (!encontrado) {
            Console.WriteLine("Contacto no encontrado.");
        }
    }
}

var registros = new Registros();
while (true) {
    Console.WriteLine("\n1. Agregar contacto\n2. Mostrar contactos\n3. Modificar contacto\n4. Buscar contacto\n5. Eliminar contacto\n6. Salir");
    switch (Console.ReadLine()) {
        case "1":
            registros.AgregarContacto();
            break;
        case "2":
            registros.MostrarContactos();
            break;
        case "3":
            registros.ModificarContacto();
            break;
        case "4":
            registros.Buscarcontacto();
            break;
        case "5":
            registros.EliminarContacto();
            break;
        case "6":
            return;
        default:
            Console.WriteLine("Opción no válida.");
            break;
    }
}