using System;       // Para usar la consola  (Console)
using System.IO;    // Para leer archivos    (File)
using System;
using System.Collections.Generic;
using System.Linq;

// Ayuda: 
//   Console.Clear() : Borra la pantalla
//   Console.Write(texto) : Escribe texto sin salto de línea
//   Console.WriteLine(texto) : Escribe texto con salto de línea
//   Console.ReadLine() : Lee una línea de texto
//   Console.ReadKey() : Lee una tecla presionada
// Cuenta base
abstract class CuentaBase
{
    public string Numero { get; }
    public double Saldo { get; protected set; }
    public double Puntos { get; protected set; }

// File.ReadLines(origen) : Lee todas las líneas de un archivo y devuelve una lista de strings
// File.WriteLines(destino, lineas) : Escribe una lista de líneas en un archivo
    public CuentaBase(string numero) => Numero = numero;

// Escribir la solucion al TP1 en este archivo. (Borre el ejemplo de abajo)
Console.WriteLine("Hola, soy el ejercicio 1 del TP1 de la materia Programación 3");
Console.Write("Presionar una tecla para continuar...");
Console.ReadKey();using System;
using System.IO;
    public virtual void Depositar(double monto) => Saldo += monto;

namespace AgendaDeContactos
{
    struct Contacto
    public virtual bool Extraer(double monto)
    {
        public int Id;
        public string Nombre;
        public string Telefono;
        public string Email;
        if (Saldo >= monto)
        {
            Saldo -= monto;
            return true;
        }
        return false;
    }

    class Program
    public bool Pagar(double monto)
    {
        const int MAX_CONTACTOS = 100;
        static Contacto[] contactos = new Contacto[MAX_CONTACTOS];
        static int cantidad = 0; // número de contactos actuales
        static int siguienteId = 1; // ID incremental

        static void Main(string[] args)
        if (Extraer(monto))
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
            AcumularPuntos(monto);
            return true;
        }
        return false;
    }

        static void AgregarContacto()
        {
            if (cantidad >= MAX_CONTACTOS)
            {
                Console.WriteLine("Se alcanzó el límite de contactos.");
                Console.ReadLine();
                return;
            }
    protected abstract void AcumularPuntos(double monto);
}

class CuentaOro : CuentaBase
{
    public CuentaOro(string numero) : base(numero) { }

            Contacto nuevo;
            nuevo.Id = siguienteId++;
            Console.Write("Nombre: ");
            nuevo.Nombre = Console.ReadLine();
            Console.Write("Teléfono: ");
            nuevo.Telefono = Console.ReadLine();
            Console.Write("Email: ");
            nuevo.Email = Console.ReadLine();
    protected override void AcumularPuntos(double monto)
        => Puntos += monto > 1000 ? monto * 0.05 : monto * 0.03;
}

            contactos[cantidad] = nuevo;
            cantidad++;
class CuentaPlata : CuentaBase
{
    public CuentaPlata(string numero) : base(numero) { }

            Console.WriteLine("Contacto agregado correctamente. Presione ENTER para continuar.");
            Console.ReadLine();
        }
    protected override void AcumularPuntos(double monto)
        => Puntos += monto * 0.02;
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
class CuentaBronce : CuentaBase
{
    public CuentaBronce(string numero) : base(numero) { }

            int pos = BuscarIndicePorId(id);
            if (pos == -1)
            {
                Console.WriteLine("Contacto no encontrado. Presione ENTER para continuar.");
                Console.ReadLine();
                return;
            }
    protected override void AcumularPuntos(double monto)
        => Puntos += monto * 0.01;
}

            Console.WriteLine("Dejar en blanco para no modificar el campo.");
            Console.Write("Nuevo nombre (actual: {0}): ", contactos[pos].Nombre);
            string nombre = Console.ReadLine();
            if (!string.IsNullOrEmpty(nombre))
                contactos[pos].Nombre = nombre;
class Cliente
{
    public string Nombre { get; }
    public List<CuentaBase> Cuentas { get; } = new();
    public List<string> Historial { get; } = new();

            Console.Write("Nuevo teléfono (actual: {0}): ", contactos[pos].Telefono);
            string telefono = Console.ReadLine();
            if (!string.IsNullOrEmpty(telefono))
                contactos[pos].Telefono = telefono;
    public Cliente(string nombre) => Nombre = nombre;

            Console.Write("Nuevo email (actual: {0}): ", contactos[pos].Email);
            string email = Console.ReadLine();
            if (!string.IsNullOrEmpty(email))
                contactos[pos].Email = email;
    public void AgregarCuenta(CuentaBase cuenta) => Cuentas.Add(cuenta);
    public void RegistrarOperacion(string detalle) => Historial.Add(detalle);
}

            Console.WriteLine("Contacto modificado correctamente. Presione ENTER para continuar.");
            Console.ReadLine();
        }
class Banco
{
    private List<Cliente> clientes = new();
    private List<string> operaciones = new();

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
    public void AgregarCliente(Cliente cliente) => clientes.Add(cliente);

            int pos = BuscarIndicePorId(id);
            if (pos == -1)
            {
                Console.WriteLine("Contacto no encontrado. Presione ENTER para continuar.");
                Console.ReadLine();
                return;
            }
    private CuentaBase? BuscarCuenta(string numero) =>
        clientes.SelectMany(c => c.Cuentas).FirstOrDefault(c => c.Numero == numero);

            // Mover todos los elementos siguientes un lugar hacia atrás
            for (int i = pos; i < cantidad - 1; i++)
            {
                contactos[i] = contactos[i + 1];
            }
            cantidad--;
    private Cliente? BuscarClientePorCuenta(string numero) =>
        clientes.FirstOrDefault(c => c.Cuentas.Any(cuenta => cuenta.Numero == numero));

            Console.WriteLine("Contacto borrado correctamente. Presione ENTER para continuar.");
            Console.ReadLine();
        }
    private void Registrar(string detalle, string nroCuenta)
    {
        operaciones.Add(detalle);
        var cliente = BuscarClientePorCuenta(nroCuenta);
        if (cliente != null) cliente.RegistrarOperacion(detalle);
    }

        static void ListarContactos()
    public void Depositar(string nroCuenta, double monto)
    {
        var cuenta = BuscarCuenta(nroCuenta);
        if (cuenta != null)
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
            cuenta.Depositar(monto);
            Registrar($"DEPÓSITO de ${monto} en {nroCuenta}", nroCuenta);
        }
    }

        static void BuscarContacto()
        {
            Console.Write("Ingrese término de búsqueda: ");
            string termino = Console.ReadLine().ToLower();
    public void Extraer(string nroCuenta, double monto)
    {
        var cuenta = BuscarCuenta(nroCuenta);
        if (cuenta != null && cuenta.Extraer(monto))
            Registrar($"RETIRO de ${monto} en {nroCuenta}", nroCuenta);
    }

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
    public void Pagar(string nroCuenta, double monto)
    {
        var cuenta = BuscarCuenta(nroCuenta);
        if (cuenta != null && cuenta.Pagar(monto))
            Registrar($"PAGO de ${monto} en {nroCuenta} (Puntos: {cuenta.Puntos})", nroCuenta);
    }

        static int BuscarIndicePorId(int id)
    public void Transferir(string origen, string destino, double monto)
    {
        var ctaOrigen = BuscarCuenta(origen);
        var ctaDestino = BuscarCuenta(destino);
        if (ctaOrigen != null && ctaDestino != null && ctaOrigen.Extraer(monto))
        {
            for (int i = 0; i < cantidad; i++)
            {
                if (contactos[i].Id == id)
                    return i;
            }
            return -1;
            ctaDestino.Depositar(monto);
            Registrar($"TRANSFERENCIA de ${monto} de {origen} a {destino}", origen);
            Registrar($"TRANSFERENCIA recibida de ${monto} desde {origen}", destino);
        }
    }

        static void CargarContactos()
    public void Reporte()
    {
        Console.WriteLine("---- Operaciones Globales ----");
        operaciones.ForEach(Console.WriteLine);

        foreach (var cli in clientes)
        {
            if (!File.Exists("agenda.csv"))
                return;
            Console.WriteLine($"\nCliente: {cli.Nombre}");
            foreach (var c in cli.Cuentas)
                Console.WriteLine($"Cuenta {c.Numero} | Saldo: ${c.Saldo} | Puntos: {c.Puntos}");
            Console.WriteLine("Historial:");
            cli.Historial.ForEach(op => Console.WriteLine(" - " + op));
        }
    }
}

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
class Program
{
    static void Main()
    {
        Banco banco = new();

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
        var juan = new Cliente("Juan");
        juan.AgregarCuenta(new CuentaOro("00001"));
        banco.AgregarCliente(juan);

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
        var ana = new Cliente("Ana");
        ana.AgregarCuenta(new CuentaPlata("00002"));
        banco.AgregarCliente(ana);

        banco.Depositar("00001", 2000);
        banco.Pagar("00001", 1500);
        banco.Transferir("00001", "00002", 300);
        banco.Extraer("00002", 100);

        banco.Reporte();
    }
}