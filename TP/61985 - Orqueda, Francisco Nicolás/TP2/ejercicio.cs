using System;
using System.Collections.Generic;
using System.Linq;

abstract class Cuenta
{
    public string Numero { get; private set; }
    public decimal Saldo { get; protected set; }
    public int Puntos { get; protected set; }
    public Cuenta(string numero)
    {
        Numero = numero;
        Saldo = 0;
        Puntos = 0;
    }
    public virtual void Depositar(decimal monto)
    {
        Saldo += monto;
    }
    public virtual bool Extraer(decimal monto)
    {
        if (Saldo >= monto)
        {
            Saldo -= monto;
            return true;
        }
        return false;
    }
    public virtual bool Pagar(decimal monto)
    {
        if (Extraer(monto))
        {
            AcumularPuntos(monto);
            return true;
        }
        return false;
    }
    protected abstract void AcumularPuntos(decimal monto);
}
class CuentaOro : Cuenta
{
    public CuentaOro(string numero) : base(numero) { }
    protected override void AcumularPuntos(decimal monto)
    {
        if (monto > 1000)
            Puntos += (int)(monto * 0.05m);
        else
            Puntos += (int)(monto * 0.03m);
    }
}
class CuentaPlata : Cuenta
{
    public CuentaPlata(string numero) : base(numero) { }
    protected override void AcumularPuntos(decimal monto)
    {
        Puntos += (int)(monto * 0.02m);
    }
}
class CuentaBronce : Cuenta
{
    public CuentaBronce(string numero) : base(numero) { }
    protected override void AcumularPuntos(decimal monto)
    {
        Puntos += (int)(monto * 0.01m);
    }
}
abstract class Operacion
{
    public decimal Monto { get; protected set; }
    public string Tipo { get; protected set; }
    public Cuenta CuentaOrigen { get; protected set; }
    public Cuenta CuentaDestino { get; protected set; }
    public DateTime Fecha { get; private set; } = DateTime.Now;

    public abstract bool Ejecutar();
}

class Deposito : Operacion
{
    public Deposito(Cuenta cuenta, decimal monto)
    {
        CuentaOrigen = cuenta;
        Monto = monto;
        Tipo = "Depósito";
    }

    public override bool Ejecutar()
    {
        CuentaOrigen.Depositar(Monto);
        return true;
    }
}

class Retiro : Operacion
{
    public Retiro(Cuenta cuenta, decimal monto)
    {
        CuentaOrigen = cuenta;
        Monto = monto;
        Tipo = "Retiro";
    }

    public override bool Ejecutar()
    {
        return CuentaOrigen.Extraer(Monto);
    }
}

class Pago : Operacion
{
    public Pago(Cuenta cuenta, decimal monto)
    {
        CuentaOrigen = cuenta;
        Monto = monto;
        Tipo = "Pago";
    }

    public override bool Ejecutar()
    {
        return CuentaOrigen.Pagar(Monto);
    }
}

class Transferencia : Operacion
{
    public Transferencia(Cuenta origen, Cuenta destino, decimal monto)
    {
        CuentaOrigen = origen;
        CuentaDestino = destino;
        Monto = monto;
        Tipo = "Transferencia";
    }

    public override bool Ejecutar()
    {
        if (CuentaOrigen.Extraer(Monto))
        {
            CuentaDestino.Depositar(Monto);
            return true;
        }
        return false;
    }
}

class Cliente
{
    public int Id { get; private set; }
    public string Nombre { get; private set; }
    public List<Cuenta> Cuentas { get; private set; }
    public List<Operacion> Historial { get; private set; }

    public Cliente(int id, string nombre)
    {
        Id = id;
        Nombre = nombre;
        Cuentas = new List<Cuenta>();
        Historial = new List<Operacion>();
    }

    public void AgregarCuenta(Cuenta cuenta)
    {
        Cuentas.Add(cuenta);
    }

    public void RegistrarOperacion(Operacion op)
    {
        Historial.Add(op);
    }
}

class Banco
{
    private int siguienteIdCliente = 1000;
    private int siguienteNumeroCuenta = 1000; 
    private Dictionary<int, Cliente> clientes = new Dictionary<int, Cliente>();
    private List<Operacion> operaciones = new List<Operacion>();
    private HashSet<string> numerosDeCuenta = new HashSet<string>();
    private static Random random = new Random(); 

    public Cliente CrearCliente(string nombre)
    {
        var cliente = new Cliente(siguienteIdCliente++, nombre);
        clientes[cliente.Id] = cliente;
        return cliente;
    }

    public List<Cliente> BuscarTodosClientes()
    {
        return clientes.Values.ToList();
    }

    public void MostrarClientes()
    {
        Console.WriteLine("\n--- Lista de Clientes y sus Cuentas ---");
        foreach (var cliente in clientes.Values)
        {
            Console.WriteLine($"ID: {cliente.Id}, Nombre: {cliente.Nombre}");

            if (cliente.Cuentas.Count > 0)
            {
                foreach (var cuenta in cliente.Cuentas)
                {
                    string tipoCuenta = cuenta.GetType().Name; // Obtiene el tipo de cuenta (CuentaOro, CuentaPlata, etc.)
                    Console.WriteLine($"   - Cuenta N°: {cuenta.Numero}, Tipo: {tipoCuenta}, Saldo: {cuenta.Saldo}, Puntos: {cuenta.Puntos}");
                }
            }
            else
            {
                Console.WriteLine("   - No tiene cuentas asociadas.");
            }
        }
    }

    public Cuenta CrearCuenta(Cliente cliente, string tipo)
    {
        string numero = (siguienteNumeroCuenta++).ToString(); // Incrementa el número de cuenta
        numerosDeCuenta.Add(numero);

        Cuenta cuenta;
        switch (tipo.ToLower())
        {
            case "oro": cuenta = new CuentaOro(numero); break;
            case "plata": cuenta = new CuentaPlata(numero); break;
            case "bronce": cuenta = new CuentaBronce(numero); break;
            default: throw new Exception("Tipo de cuenta inválido.");
        }

        cliente.AgregarCuenta(cuenta);
        return cuenta;
    }

    public bool EjecutarOperacion(Operacion op, Cliente cliente)
    {
        if (cliente.Cuentas.Contains(op.CuentaOrigen))
        {
            if (op.Ejecutar())
            {
                operaciones.Add(op);
                cliente.RegistrarOperacion(op);
                return true;
            }
        }
        return false;
    }

    public void ReporteCompleto()
    {
        Console.WriteLine("--- Historial Global de Operaciones ---");
        foreach (var op in operaciones)
        {
            
            var clienteOrigen = clientes.Values.FirstOrDefault(c => c.Cuentas.Contains(op.CuentaOrigen));
            string nombreOrigen = clienteOrigen != null ? clienteOrigen.Nombre : "N/A";

            
            var clienteDestino = clientes.Values.FirstOrDefault(c => c.Cuentas.Contains(op.CuentaDestino));
            string nombreDestino = clienteDestino != null ? clienteDestino.Nombre : "N/A";

            
            Console.WriteLine($"{op.Tipo} - Monto: {op.Monto}, Origen: {op.CuentaOrigen?.Numero ?? "N/A"} ({nombreOrigen}), " +
                              $"Destino: {op.CuentaDestino?.Numero ?? "N/A"} ({nombreDestino}), Fecha: {op.Fecha}");
        }

        Console.WriteLine("\n--- Estado Final de las Cuentas ---");
        foreach (var cliente in clientes.Values)
        {
            foreach (var cuenta in cliente.Cuentas)
            {
                Console.WriteLine($"Cuenta {cuenta.Numero}: Saldo = {cuenta.Saldo}, Puntos = {cuenta.Puntos}");
            }
        }

        Console.WriteLine("\n--- Historial por Cliente ---");
        foreach (var cliente in clientes.Values)
        {
            Console.WriteLine($"\nCliente {cliente.Id} - {cliente.Nombre}");
            foreach (var op in cliente.Historial)
            {
                Console.WriteLine($"{op.Fecha} - {op.Tipo}: Monto = {op.Monto}, Cuenta = {op.CuentaOrigen.Numero}");
            }
        }
    }

    public Cliente BuscarClientePorId(int id)
    {
        return clientes.ContainsKey(id) ? clientes[id] : null;
    }
}

Banco banco = new Banco(); 

while (true)
{
    Console.Clear(); 
    Console.WriteLine("\n--- Menú Principal ---");
    Console.WriteLine("1. Crear Cliente");
    Console.WriteLine("2. Crear Cuenta");
    Console.WriteLine("3. Realizar Operación");
    Console.WriteLine("4. Ver Reporte");
    Console.WriteLine("5. Mostrar Clientes y Cuentas");
    Console.WriteLine("6. Salir");
    Console.Write("Seleccione una opción: ");

    switch (Console.ReadLine())
    {
        case "1":
            Console.Clear();
            Console.Write("Nombre del cliente: ");
            var cliente = banco.CrearCliente(Console.ReadLine());
            Console.WriteLine($"Cliente creado. ID: {cliente.Id}");
            Console.WriteLine("\nPresione cualquier tecla para continuar...");
            Console.ReadKey();
            break;

        case "2":
            Console.Clear();
            Console.Write("ID del cliente: ");
            if (int.TryParse(Console.ReadLine(), out int idCliente))
            {
                var cli = banco.BuscarClientePorId(idCliente);
                if (cli != null)
                {
                    Console.Write("Tipo de cuenta (Oro/Plata/Bronce): ");
                    var cuenta = banco.CrearCuenta(cli, Console.ReadLine());
                    Console.WriteLine($"Cuenta creada. Número: {cuenta.Numero}");
                }
                else
                {
                    Console.WriteLine("Cliente no encontrado.");
                }
            }
            else
            {
                Console.WriteLine("ID de cliente inválido.");
            }
            Console.WriteLine("\nPresione cualquier tecla para continuar...");
            Console.ReadKey();
            break;

        case "3": 
            Console.Clear();
            Console.Write("ID del cliente: ");
            if (int.TryParse(Console.ReadLine(), out int idOp))
            {
                var cli = banco.BuscarClientePorId(idOp);
                if (cli != null)
                {
                    Console.WriteLine("Seleccione operación: 1. Depósito, 2. Retiro, 3. Pago, 4. Transferencia");
                    string op = Console.ReadLine();
                    Console.Write("Número de cuenta origen: ");
                    var origen = cli.Cuentas.FirstOrDefault(c => c.Numero == Console.ReadLine());
                    if (origen != null)
                    {
                        Console.Write("Monto: ");
                        if (decimal.TryParse(Console.ReadLine(), out decimal monto))
                        {
                            Operacion operacion = null;
                            switch (op)
                            {
                                case "1": operacion = new Deposito(origen, monto); break;
                                case "2": operacion = new Retiro(origen, monto); break;
                                case "3": operacion = new Pago(origen, monto); break;
                                case "4":
                                    Console.Write("Número de cuenta destino: ");
                                    string destinoNum = Console.ReadLine();
                                    var destino = banco.BuscarTodosClientes()
                                        .SelectMany(c => c.Cuentas)
                                        .FirstOrDefault(c => c.Numero == destinoNum);
                                    if (destino != null)
                                    {
                                        operacion = new Transferencia(origen, destino, monto);
                                    }
                                    else
                                    {
                                        Console.WriteLine("Cuenta destino no encontrada.");
                                    }
                                    break;
                                default:
                                    Console.WriteLine("Operación no válida.");
                                    break;
                            }
                            if (operacion != null && banco.EjecutarOperacion(operacion, cli))
                                Console.WriteLine("Operación realizada con éxito.");
                            else
                                Console.WriteLine("Error al realizar la operación.");
                        }
                        else
                        {
                            Console.WriteLine("Monto inválido.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Cuenta origen no encontrada.");
                    }
                }
                else
                {
                    Console.WriteLine("Cliente no encontrado.");
                }
            }
            else
            {
                Console.WriteLine("ID de cliente inválido.");
            }
            Console.WriteLine("\nPresione cualquier tecla para continuar...");
            Console.ReadKey();
            break;

        case "4": 
            Console.Clear();
            Console.WriteLine("\n--- Reporte Completo ---");
            banco.ReporteCompleto();
            Console.WriteLine("\nPresione cualquier tecla para continuar...");
            Console.ReadKey();
            break;

        case "5": 
            Console.Clear();
            Console.WriteLine("\n--- Lista de Clientes y Cuentas ---");
            banco.MostrarClientes();
            Console.WriteLine("\nPresione cualquier tecla para continuar...");
            Console.ReadKey();
            break;

        case "6":
            return;

        default:
            Console.WriteLine("Opción no válida. Intente nuevamente.");
            Console.WriteLine("\nPresione cualquier tecla para continuar...");
            Console.ReadKey();
            break;
    }
}
