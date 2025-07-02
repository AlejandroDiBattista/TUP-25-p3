
using System;
using System.Collections.Generic;

abstract class Cuenta
{
    public int Numero { get; set; }
    public double Saldo { get; protected set; }
    public int Puntos { get; protected set; }

    public Cuenta(int numero, double saldoInicial = 0)
    {
        Numero = numero;
        Saldo = saldoInicial;
        Puntos = 0;
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

    public abstract void Pagar(double monto);
}

class CuentaOro : Cuenta
{
    public CuentaOro(int numero, double saldoInicial = 0) : base(numero, saldoInicial) { }

    public override void Pagar(double monto)
    {
        if (Extraer(monto))
        {
            Puntos += (int)(monto * 0.05);
        }
    }
}

class CuentaPlata : Cuenta
{
    public CuentaPlata(int numero, double saldoInicial = 0) : base(numero, saldoInicial) { }

    public override void Pagar(double monto)
    {
        if (Extraer(monto))
        {
            Puntos += (int)(monto * 0.03);
        }
    }
}

class CuentaBronce : Cuenta
{
    public CuentaBronce(int numero, double saldoInicial = 0) : base(numero, saldoInicial) { }

    public override void Pagar(double monto)
    {
        if (Extraer(monto))
        {
            Puntos += (int)(monto * 0.01);
        }
    }
}

abstract class Operacion
{
    public double Monto { get; set; }
    public abstract void Ejecutar(Banco banco);
}

class Deposito : Operacion
{
    public int NumeroCuenta { get; set; }

    public Deposito(int numeroCuenta, double monto)
    {
        NumeroCuenta = numeroCuenta;
        Monto = monto;
    }

    public override void Ejecutar(Banco banco)
    {
        var cuenta = banco.ObtenerCuenta(NumeroCuenta);
        cuenta?.Depositar(Monto);
    }
}

class Extraccion : Operacion
{
    public int NumeroCuenta { get; set; }

    public Extraccion(int numeroCuenta, double monto)
    {
        NumeroCuenta = numeroCuenta;
        Monto = monto;
    }

    public override void Ejecutar(Banco banco)
    {
        var cuenta = banco.ObtenerCuenta(NumeroCuenta);
        cuenta?.Extraer(Monto);
    }
}

class Pago : Operacion
{
    public int NumeroCuenta { get; set; }

    public Pago(int numeroCuenta, double monto)
    {
        NumeroCuenta = numeroCuenta;
        Monto = monto;
    }

    public override void Ejecutar(Banco banco)
    {
        var cuenta = banco.ObtenerCuenta(NumeroCuenta);
        cuenta?.Pagar(Monto);
    }
}

class Transferencia : Operacion
{
    public int CuentaOrigen { get; set; }
    public int CuentaDestino { get; set; }

    public Transferencia(int origen, int destino, double monto)
    {
        CuentaOrigen = origen;
        CuentaDestino = destino;
        Monto = monto;
    }

    public override void Ejecutar(Banco banco)
    {
        var origen = banco.ObtenerCuenta(CuentaOrigen);
        var destino = banco.ObtenerCuenta(CuentaDestino);
        if (origen != null && destino != null && origen.Extraer(Monto))
        {
            destino.Depositar(Monto);
        }
    }
}

class Banco
{
    public string Nombre { get; set; }
    private Dictionary<int, Cuenta> cuentas = new();

    public Banco(string nombre)
    {
        Nombre = nombre;
    }

    public void AgregarCuenta(Cuenta cuenta)
    {
        cuentas[cuenta.Numero] = cuenta;
    }

    public Cuenta? ObtenerCuenta(int numero)
    {
        cuentas.TryGetValue(numero, out var cuenta);
        return cuenta;
    }

    public void EjecutarOperacion(Operacion operacion)
    {
        operacion.Ejecutar(this);
    }

    public void MostrarResumen()
    {
        Console.WriteLine($"Banco: {Nombre}");
        foreach (var cuenta in cuentas.Values)
        {
            Console.WriteLine($"Cuenta {cuenta.Numero} - Saldo: ${cuenta.Saldo}, Puntos: {cuenta.Puntos}");
        }
    }
}

class Program
{
    static void Main()
    {
        Banco banco = new("Banco FullStack");

        banco.AgregarCuenta(new CuentaOro(1001, 5000));
        banco.AgregarCuenta(new CuentaPlata(1002, 3000));
        banco.AgregarCuenta(new CuentaBronce(1003, 2000));

        bool salir = false;
        while (!salir)
        {
            Console.WriteLine("\nMenú de operaciones:");
            Console.WriteLine("1. Depositar");
            Console.WriteLine("2. Extraer");
            Console.WriteLine("3. Pagar");
            Console.WriteLine("4. Transferir");
            Console.WriteLine("5. Ver cuentas");
            Console.WriteLine("6. Salir");
            Console.Write("Seleccione una opción: ");

            switch (Console.ReadLine())
            {
                case "1":
                    Console.Write("N° Cuenta: ");
                    int depCuenta = int.Parse(Console.ReadLine());
                    Console.Write("Monto: ");
                    double depMonto = double.Parse(Console.ReadLine());
                    banco.EjecutarOperacion(new Deposito(depCuenta, depMonto));
                    break;
                case "2":
                    Console.Write("N° Cuenta: ");
                    int extCuenta = int.Parse(Console.ReadLine());
                    Console.Write("Monto: ");
                    double extMonto = double.Parse(Console.ReadLine());
                    banco.EjecutarOperacion(new Extraccion(extCuenta, extMonto));
                    break;
                case "3":
                    Console.Write("N° Cuenta: ");
                    int pagCuenta = int.Parse(Console.ReadLine());
                    Console.Write("Monto: ");
                    double pagMonto = double.Parse(Console.ReadLine());
                    banco.EjecutarOperacion(new Pago(pagCuenta, pagMonto));
                    break;
                case "4":
                    Console.Write("Cuenta Origen: ");
                    int ori = int.Parse(Console.ReadLine());
                    Console.Write("Cuenta Destino: ");
                    int dest = int.Parse(Console.ReadLine());
                    Console.Write("Monto: ");
                    double transMonto = double.Parse(Console.ReadLine());
                    banco.EjecutarOperacion(new Transferencia(ori, dest, transMonto));
                    break;
                case "5":
                    banco.MostrarResumen();
                    break;
                case "6":
                    salir = true;
                    break;
                default:
                    Console.WriteLine("Opción inválida.");
                    break;
            }
        }
    }
}
