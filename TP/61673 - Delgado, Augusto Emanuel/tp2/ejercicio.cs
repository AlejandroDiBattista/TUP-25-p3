using System;
using System.Collections.Generic;
using System.Linq;

// Clase abstracta Operación
abstract class Operacion
{
    public string CuentaOrigen { get; }
    public string CuentaDestino { get; }
    public decimal Monto { get; }

    protected Operacion(string cuentaOrigen, decimal monto, string cuentaDestino = null)
    {
        CuentaOrigen = cuentaOrigen;
        Monto = monto;
        CuentaDestino = cuentaDestino;
    }

    public abstract void Ejecutar(Banco banco);
}

// Clases de operaciones
class Deposito : Operacion
{
    public Deposito(string cuenta, decimal monto) : base(cuenta, monto) { }
    public override void Ejecutar(Banco banco) => banco.Depositar(CuentaOrigen, Monto);
}

class Retiro : Operacion
{
    public Retiro(string cuenta, decimal monto) : base(cuenta, monto) { }
    public override void Ejecutar(Banco banco) => banco.Retirar(CuentaOrigen, Monto);
}

class Pago : Operacion
{
    public Pago(string cuenta, decimal monto) : base(cuenta, monto) { }
    public override void Ejecutar(Banco banco) => banco.Pagar(CuentaOrigen, Monto);
}

class Transferencia : Operacion
{
    public Transferencia(string cuentaOrigen, string cuentaDestino, decimal monto) : base(cuentaOrigen, monto, cuentaDestino) { }
    public override void Ejecutar(Banco banco) => banco.Transferir(CuentaOrigen, CuentaDestino, Monto);
}

// Clase abstracta Cuenta
abstract class Cuenta
{
    public string Numero { get; }
    public decimal Saldo { get; protected set; }
    public decimal Puntos { get; protected set; }

    protected Cuenta(string numero, decimal saldoInicial)
    {
        Numero = numero;
        Saldo = saldoInicial;
        Puntos = 0;
    }

    public void Depositar(decimal monto) => Saldo += monto;
    public bool Retirar(decimal monto) => monto <= Saldo ? (Saldo -= monto, true).Item2 : false;
    public abstract void Pagar(decimal monto);
}

// Tipos de cuentas
class CuentaOro : Cuenta
{
    public CuentaOro(string numero, decimal saldoInicial) : base(numero, saldoInicial) { }
    public override void Pagar(decimal monto)
    {
        if (Retirar(monto))
            Puntos += monto > 1000 ? monto * 0.05m : monto * 0.03m;
    }
}

class CuentaPlata : Cuenta
{
    public CuentaPlata(string numero, decimal saldoInicial) : base(numero, saldoInicial) { }
    public override void Pagar(decimal monto)
    {
        if (Retirar(monto))
            Puntos += monto * 0.02m;
    }
}

class CuentaBronce : Cuenta
{
    public CuentaBronce(string numero, decimal saldoInicial) : base(numero, saldoInicial) { }
    public override void Pagar(decimal monto)
    {
        if (Retirar(monto))
            Puntos += monto * 0.01m;
    }
}

// Clase Cliente
class Cliente
{
    public string Nombre { get; }
    public List<Cuenta> Cuentas { get; }

    public Cliente(string nombre)
    {
        Nombre = nombre;
        Cuentas = new List<Cuenta>();
    }

    public void Agregar(Cuenta cuenta) => Cuentas.Add(cuenta);
}

// Clase Banco
class Banco
{
    public string Nombre { get; }
    private List<Cliente> clientes;
    private List<Operacion> operaciones;

    public Banco(string nombre)
    {
        Nombre = nombre;
        clientes = new List<Cliente>();
        operaciones = new List<Operacion>();
    }

    public void Agregar(Cliente cliente) => clientes.Add(cliente);
    public void Registrar(Operacion operacion)
    {
        operaciones.Add(operacion);
        operacion.Ejecutar(this);
    }

    public void Depositar(string numeroCuenta, decimal monto) => BuscarCuenta(numeroCuenta)?.Depositar(monto);
    public void Retirar(string numeroCuenta, decimal monto) => BuscarCuenta(numeroCuenta)?.Retirar(monto);
    public void Pagar(string numeroCuenta, decimal monto) => BuscarCuenta(numeroCuenta)?.Pagar(monto);
    public void Transferir(string origen, string destino, decimal monto)
    {
        var cuentaOrigen = BuscarCuenta(origen);
        var cuentaDestino = BuscarCuenta(destino);
        if (cuentaOrigen?.Retirar(monto) == true) cuentaDestino?.Depositar(monto);
    }

    private Cuenta BuscarCuenta(string numero) =>
        clientes.SelectMany(cliente => cliente.Cuentas).FirstOrDefault(cuenta => cuenta.Numero == numero);

    public void Informe()
    {
        Console.WriteLine($"Banco: {Nombre} | Clientes: {clientes.Count}");
        foreach (var cliente in clientes)
        {
            decimal saldoTotal = cliente.Cuentas.Sum(c => c.Saldo);
            decimal puntosTotal = cliente.Cuentas.Sum(c => c.Puntos);
            Console.WriteLine($"\n  Cliente: {cliente.Nombre} | Saldo Total: ${saldoTotal} | Puntos Total: ${puntosTotal}");
            foreach (var cuenta in cliente.Cuentas)
                Console.WriteLine($"    Cuenta: {cuenta.Numero} | Saldo: ${cuenta.Saldo} | Puntos: ${cuenta.Puntos}");
        }
    }
}

// Menú de operaciones
class Program
{
    static void Main()
    {
        var banco = new Banco("Banco Central");

        var cliente1 = new Cliente("Juan Pérez");
        cliente1.Agregar(new CuentaOro("10001", 1000));
        cliente1.Agregar(new CuentaPlata("10002", 2000));

        var cliente2 = new Cliente("Maria Gómez");
        cliente2.Agregar(new CuentaBronce("10003", 1500));

        banco.Agregar(cliente1);
        banco.Agregar(cliente2);

        while (true)
        {
            Console.WriteLine("\n--- Menú de Operaciones Bancarias ---");
            Console.WriteLine("1. Depositar");
            Console.WriteLine("2. Retirar");
            Console.WriteLine("3. Pagar");
            Console.WriteLine("4. Transferir");
            Console.WriteLine("5. Mostrar informe");
            Console.WriteLine("6. Salir");
            Console.Write("Seleccione una opción: ");

            string opcion = Console.ReadLine();

            if (opcion == "6") break;

            Console.Write("Ingrese el número de cuenta: ");
            string cuentaOrigen = Console.ReadLine();
            decimal monto = 0;

            if (opcion != "5")
            {
                Console.Write("Ingrese el monto: ");
                if (!decimal.TryParse(Console.ReadLine(), out monto))
                {
                    Console.WriteLine("Monto inválido. Inténtelo nuevamente.");
                    continue;
                }
            }

            switch (opcion)
            {
                case "1":
                    banco.Registrar(new Deposito(cuentaOrigen, monto));
                    break;
                case "2":
                    banco.Registrar(new Retiro(cuentaOrigen, monto));
                    break;
                case "3":
                    banco.Registrar(new Pago(cuentaOrigen, monto));
                    break;
                case "4":
                    Console.Write("Ingrese el número de cuenta destino: ");
                    string cuentaDestino = Console.ReadLine();
                    banco.Registrar(new Transferencia(cuentaOrigen, cuentaDestino, monto));
                    break;
                case "5":
                    banco.Informe();
                    break;
                default:
                    Console.WriteLine("Opción inválida. Inténtelo nuevamente.");
                    break;
            }
        }
    }
}