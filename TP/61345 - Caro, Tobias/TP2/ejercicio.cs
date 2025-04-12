using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

class Program
{
    static Banco banco = new Banco();
    static int id = 10000;

    static void Main()
    {
        while (true)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[1] Agregar cliente");
            Console.WriteLine("[2] Mostrar clientes");
            Console.WriteLine("[3] Realizar operación");
            Console.WriteLine("[4] Reporte completo");
            Console.WriteLine("[0] Salir");
            Console.Write("Seleccione una opción: ");
            string seleccion = Console.ReadLine();

            switch (seleccion)
            {
                case "1":
                    AgregarCliente();
                    break;
                case "2":
                    MostrarClientes();
                    break;
                case "3":
                    RealizarOperacion();
                    break;
                case "4":
                    banco.ReporteCompleto();
                    break;
                case "0":
                    return;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Selección no válida. Intente nuevamente.");
                    Thread.Sleep(1000);
                    break;
            }
        }
    }

    static void AgregarCliente()
    {
        Console.Clear();
        Console.Write("Ingrese el nombre del cliente: ");
        string nombre = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(nombre))
        {
            Console.WriteLine("Nombre inválido.");
            return;
        }

        Cliente cliente = banco.ObtenerCliente(nombre);
        if (cliente == null)
        {
            cliente = new Cliente(nombre);
            banco.AgregarCliente(cliente);
        }

        Console.WriteLine("Seleccione el tipo de cuenta:");
        Console.WriteLine("[1] Cuenta Oro");
        Console.WriteLine("[2] Cuenta Plata");
        Console.WriteLine("[3] Cuenta Bronce");
        string tipo = Console.ReadLine();

        Console.Write("Ingrese el monto inicial: ");
        if (!double.TryParse(Console.ReadLine(), out double montoInicial) || montoInicial <= 0)
        {
            Console.WriteLine("Monto inválido.");
            return;
        }
        Cuenta cuenta;
        switch (tipo)
        {
            case "1":
                cuenta = new CuentaOro((id++).ToString(), montoInicial);
                break;
            case "2":
                cuenta = new CuentaPlata((id++).ToString(), montoInicial);
                break;
            case "3":
                cuenta = new CuentaBronce((id++).ToString(), montoInicial);
                break;
            default:
                Console.WriteLine("Tipo inválido.");
                return;
        }

        cliente.AgregarCuenta(cuenta);
        Console.WriteLine("Cuenta creada con éxito.");
        Thread.Sleep(1000);
    }

    static void MostrarClientes()
    {
        Console.Clear();
        foreach (var cliente in banco.Clientes)
        {
            Console.WriteLine($"Cliente: {cliente.Nombre}");
            cliente.MostrarCuentas();
        }
        Console.WriteLine("Presione una tecla para continuar...");
        Console.ReadKey();
    }

    static void RealizarOperacion()
    {
        Console.Clear();
        Console.Write("Ingrese el nombre del cliente: ");
        string nombre = Console.ReadLine();

        Cliente cliente = banco.ObtenerCliente(nombre);
        if (cliente == null)
        {
            Console.WriteLine("Cliente no encontrado.");
            Thread.Sleep(1000);
            return;
        }

        Console.WriteLine("Seleccione la cuenta:");
        for (int i = 0; i < cliente.Cuentas.Count; i++)
        {
            Console.WriteLine($"[{i}] {cliente.Cuentas[i].Numero} - Saldo: {cliente.Cuentas[i].Saldo}");
        }

        if (!int.TryParse(Console.ReadLine(), out int index) || index < 0 || index >= cliente.Cuentas.Count)
        {
            Console.WriteLine("Índice inválido.");
            Thread.Sleep(1000);
            return;
        }

        Cuenta cuenta = cliente.Cuentas[index];

        Console.WriteLine("Seleccione la operación:");
        Console.WriteLine("[1] Depositar");
        Console.WriteLine("[2] Extraer");
        Console.WriteLine("[3] Pagar");
        Console.WriteLine("[4] Transferir");
        string op = Console.ReadLine();

        Console.Write("Ingrese el monto: ");
        if (!double.TryParse(Console.ReadLine(), out double monto) || monto <= 0)
        {
            Console.WriteLine("Monto inválido.");
            Thread.Sleep(1000);
            return;
        }

        Operacion operacion = null;
        switch (op)
        {
            case "1":
                operacion = new Deposito(cuenta, monto);
                break;
            case "2":
                operacion = new Retiro(cuenta, monto);
                break;
            case "3":
                operacion = new Pago(cuenta, monto);
                break;
            case "4":
                Console.Write("Ingrese el número de cuenta destino: ");
                string numDest = Console.ReadLine();
                Cuenta destino = banco.BuscarCuenta(numDest);
                if (destino == null)
                {
                    Console.WriteLine("Cuenta destino no encontrada.");
                    Thread.Sleep(1000);
                    return;
                }
                operacion = new Transferencia(cuenta, destino, monto);
                break;
            default:
                Console.WriteLine("Operación inválida.");
                Thread.Sleep(1000);
                return;
        }

        if (operacion.Realizar())
        {
            cliente.RegistrarOperacion(operacion);
            banco.RegistrarOperacion(operacion);
            Console.WriteLine("Operación realizada con éxito.");
        }
        else
        {
            Console.WriteLine("Operación fallida.");
        }
        Thread.Sleep(1500);
    }
}

class Banco
{
    public List<Cliente> Clientes { get; } = new List<Cliente>();
    public List<Operacion> Operaciones { get; } = new List<Operacion>();

    public void AgregarCliente(Cliente cliente)
    {
        Clientes.Add(cliente);
    }

    public Cliente ObtenerCliente(string nombre)
    {
        return Clientes.FirstOrDefault(c => c.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase));
    }

    public void RegistrarOperacion(Operacion operacion)
    {
        Operaciones.Add(operacion);
    }

    public Cuenta BuscarCuenta(string numero)
    {
        foreach (var cliente in Clientes)
        {
            foreach (var cuenta in cliente.Cuentas)
            {
                if (cuenta.Numero == numero)
                    return cuenta;
            }
        }
        return null;
    }

    public void ReporteCompleto()
    {
        Console.Clear();
        Console.WriteLine("REPORTE COMPLETO:");
        Console.WriteLine("Operaciones globales:");
        foreach (var op in Operaciones)
        {
            Console.WriteLine(op.Descripcion());
        }

        foreach (var cliente in Clientes)
        {
            Console.WriteLine($"\nCliente: {cliente.Nombre}");
            cliente.MostrarCuentas();
            cliente.MostrarHistorial();
        }

        Console.WriteLine("\nPresione una tecla para continuar...");
        Console.ReadKey();
    }
}

class Cliente
{
    public string Nombre { get; }
    public List<Cuenta> Cuentas { get; } = new List<Cuenta>();
    public List<Operacion> Historial { get; } = new List<Operacion>();

    public Cliente(string nombre)
    {
        Nombre = nombre;
    }

    public void AgregarCuenta(Cuenta cuenta)
    {
        Cuentas.Add(cuenta);
    }

    public void RegistrarOperacion(Operacion op)
    {
        Historial.Add(op);
    }

    public void MostrarCuentas()
    {
        foreach (var cuenta in Cuentas)
        {
            Console.WriteLine($" - {cuenta.GetType().Name}: {cuenta.Numero} | Saldo: {cuenta.Saldo} | Puntos: {cuenta.Puntos}");
        }
    }

    public void MostrarHistorial()
    {
        Console.WriteLine("Historial de operaciones:");
        foreach (var op in Historial)
        {
            Console.WriteLine($"   {op.Descripcion()}");
        }
    }
}

abstract class Cuenta
{
    public string Numero { get; }
    public double Saldo { get; protected set; }
    public double Puntos { get; protected set; }

    public Cuenta(string numero, double saldo)
    {
        Numero = numero;
        Saldo = saldo;
    }

    public virtual void Depositar(double monto)
    {
        Saldo += monto;
    }

    public virtual bool Extraer(double monto)
    {
        if (Saldo >= monto)
        {
            Saldo -= monto;
            return true;
        }
        return false;
    }

    public abstract bool Pagar(double monto);
}

class CuentaOro : Cuenta
{
    public CuentaOro(string numero, double saldo) : base(numero, saldo) { }

    public override bool Pagar(double monto)
    {
        if (Extraer(monto))
        {
            Puntos += monto > 1000 ? monto * 0.05 : monto * 0.03;
            return true;
        }
        return false;
    }
}

class CuentaPlata : Cuenta
{
    public CuentaPlata(string numero, double saldo) : base(numero, saldo) { }

    public override bool Pagar(double monto)
    {
        if (Extraer(monto))
        {
            Puntos += monto * 0.02;
            return true;
        }
        return false;
    }
}

class CuentaBronce : Cuenta
{
    public CuentaBronce(string numero, double saldo) : base(numero, saldo) { }

    public override bool Pagar(double monto)
    {
        if (Extraer(monto))
        {
            Puntos += monto * 0.01;
            return true;
        }
        return false;
    }
}

abstract class Operacion
{
    public double Monto { get; }
    public abstract string Descripcion();
    public abstract bool Realizar();

    protected Operacion(double monto)
    {
        Monto = monto;
    }
}

class Deposito : Operacion
{
    private Cuenta Cuenta;
    public Deposito(Cuenta cuenta, double monto) : base(monto)
    {
        Cuenta = cuenta;
    }

    public override bool Realizar()
    {
        Cuenta.Depositar(Monto);
        return true;
    }

    public override string Descripcion() => $"Depósito de ${Monto} a cuenta {Cuenta.Numero}";
}

class Retiro : Operacion
{
    private Cuenta Cuenta;
    public Retiro(Cuenta cuenta, double monto) : base(monto)
    {
        Cuenta = cuenta;
    }

    public override bool Realizar() => Cuenta.Extraer(Monto);

    public override string Descripcion() => $"Extracción de ${Monto} de cuenta {Cuenta.Numero}";
}

class Pago : Operacion
{
    private Cuenta Cuenta;
    public Pago(Cuenta cuenta, double monto) : base(monto)
    {
        Cuenta = cuenta;
    }

    public override bool Realizar() => Cuenta.Pagar(Monto);

    public override string Descripcion() => $"Pago de ${Monto} desde cuenta {Cuenta.Numero}";
}

class Transferencia : Operacion
{
    private Cuenta Origen;
    private Cuenta Destino;

    public Transferencia(Cuenta origen, Cuenta destino, double monto) : base(monto)
    {
        Origen = origen;
        Destino = destino;
    }

    public override bool Realizar()
    {
        if (Origen.Extraer(Monto))
        {
            Destino.Depositar(Monto);
            return true;
        }
        return false;
    }

    public override string Descripcion() => $"Transferencia de ${Monto} de cuenta {Origen.Numero} a cuenta {Destino.Numero}";
}
