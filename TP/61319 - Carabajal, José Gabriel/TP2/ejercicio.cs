// TP2: Sistema de Cuentas Bancarias
using System;
using System.Collections.Generic;
using System.Linq;

class Banco
{
    public string Nombre { get; set; }
    public List<Cliente> Clientes { get; set; } = new List<Cliente>();
    public List<Operacion> HistorialGlobal { get; set; } = new List<Operacion>();

    public Banco(string nombre) => Nombre = nombre;

    public void Agregar(Cliente cliente) => Clientes.Add(cliente);

    public void Registrar(Operacion operacion)
    {
        operacion.Ejecutar();
        HistorialGlobal.Add(operacion);
    }

    public void Informe()
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\nBanco: {Nombre} | Clientes: {Clientes.Count}");
        Console.ResetColor();

        foreach (var cliente in Clientes)
        {
            double saldoTotal = cliente.Cuentas.Sum(c => c.Saldo);
            double puntosTotal = cliente.Cuentas.Sum(c => c.Puntos);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\n  Cliente: {cliente.Nombre} 👤 | Saldo Total: {saldoTotal.ToString("C2")} 💵 | Puntos Totales: {puntosTotal.ToString("F2")} ✔ ");
            Console.ResetColor();

            foreach (var cuenta in cliente.Cuentas)
            {
                Console.WriteLine($"\n    Cuenta: {cuenta.NumeroCuenta} | Saldo: {cuenta.Saldo.ToString("C2")} | Puntos: {cuenta.Puntos.ToString("F2")}");
                foreach (var mov in cuenta.Historial)
                    Console.WriteLine(mov);
            }
        }
    }
}

class Cliente
{
    public string Nombre { get; set; }
    public List<Cuenta> Cuentas { get; set; } = new List<Cuenta>();

    public Cliente(string nombre) => Nombre = nombre;

    public void Agregar(Cuenta cuenta)
    {
        cuenta.Titular = this;
        Cuentas.Add(cuenta);
    }
}

abstract class Cuenta
{
    public string NumeroCuenta { get; private set; }
    public double Saldo { get; protected set; }
    public double Puntos { get; protected set; }
    public Cliente Titular { get; set; }
    public List<string> Historial { get; set; } = new List<string>();

    public Cuenta(string numero, double saldoInicial)
    {
        NumeroCuenta = numero;
        Saldo = saldoInicial;
    }

    public void Depositar(double cantidad)
    {
        Saldo += cantidad;
    }

    public bool Retirar(double cantidad)
    {
        if (cantidad <= Saldo)
        {
            Saldo -= cantidad;
            return true;
        }
        return false;
    }

    public abstract void AcumularPuntos(double montoPago);
}

class CuentaOro : Cuenta
{
    public CuentaOro(string numero, double saldoInicial) : base(numero, saldoInicial) { }
    public override void AcumularPuntos(double montoPago) => Puntos += montoPago > 1000 ? montoPago * 0.05 : montoPago * 0.03;
}

class CuentaPlata : Cuenta
{
    public CuentaPlata(string numero, double saldoInicial) : base(numero, saldoInicial) { }
    public override void AcumularPuntos(double montoPago) => Puntos += montoPago * 0.02;
}

class CuentaBronce : Cuenta
{
    public CuentaBronce(string numero, double saldoInicial) : base(numero, saldoInicial) { }
    public override void AcumularPuntos(double montoPago) => Puntos += montoPago * 0.01;
}

abstract class Operacion
{
    public DateTime Fecha { get; set; } = DateTime.Now;
    public double Monto { get; set; }
    public abstract void Ejecutar();
}

class Deposito : Operacion
{
    private Cuenta cuenta;
    public Deposito(Cuenta cuenta, double monto)
    {
        this.cuenta = cuenta;
        Monto = monto;
    }
    public override void Ejecutar()
    {
        cuenta.Depositar(Monto);
        string descripcion = $"  -  Deposito {Monto.ToString("C2")} a [{cuenta.NumeroCuenta}/{cuenta.Titular.Nombre}]";
        cuenta.Historial.Add(descripcion);
    }
}

class Retiro : Operacion
{
    private Cuenta cuenta;
    public Retiro(Cuenta cuenta, double monto)
    {
        this.cuenta = cuenta;
        Monto = monto;
    }
    public override void Ejecutar()
    {
        if (cuenta.Retirar(Monto))
        {
            string descripcion = $"  -  Retiro {Monto.ToString("C2")} de [{cuenta.NumeroCuenta}/{cuenta.Titular.Nombre}]";
            cuenta.Historial.Add(descripcion);
        }
    }
}

class Transferencia : Operacion
{
    private Cuenta origen, destino;
    public Transferencia(Cuenta origen, Cuenta destino, double monto)
    {
        this.origen = origen;
        this.destino = destino;
        Monto = monto;
    }
    public override void Ejecutar()
    {
        if (origen.Retirar(Monto))
        {
            destino.Depositar(Monto);
            string desc = $"  -  Transferencia {Monto.ToString("C2")} de [{origen.NumeroCuenta}/{origen.Titular.Nombre}] a [{destino.NumeroCuenta}/{destino.Titular.Nombre}]";
            origen.Historial.Add(desc);
            destino.Historial.Add(desc);
        }
    }
}

class Pago : Operacion
{
    private Cuenta cuenta;
    public Pago(Cuenta cuenta, double monto)
    {
        this.cuenta = cuenta;
        Monto = monto;
    }
    public override void Ejecutar()
    {
        if (cuenta.Retirar(Monto))
        {
            cuenta.AcumularPuntos(Monto);
            string descripcion = $"  -  Pago {Monto.ToString("C2")} con [{cuenta.NumeroCuenta}/{cuenta.Titular.Nombre}]";
            cuenta.Historial.Add(descripcion);
        }
    }
}

class Program
{
    static void Main()
    {
        // Ingreso los clientes
        var cGabriel = new Cliente("Gabriel Carabajal");
        cGabriel.Agregar(new CuentaOro("00777", 10000));
        cGabriel.Agregar(new CuentaPlata("10456", 22000));

        var cMarta = new Cliente("Marta Espeche");
        cMarta.Agregar(new CuentaOro("67543", 18000));

        var cAgustin = new Cliente("Agustin Lobo");
        cAgustin.Agregar(new CuentaPlata("10443", 43000));
        cAgustin.Agregar(new CuentaPlata("10333", 50000));

        var cEnrique = new Cliente("Enrique Salinas");
        cEnrique.Agregar(new CuentaBronce("10805", 75000));
        cEnrique.Agregar(new CuentaPlata("12305", 10000));

        var cLaura = new Cliente("Laura Gomez");
        cLaura.Agregar(new CuentaOro("13406", 10000));

        var cGiuliana = new Cliente("Giuliana Avila");
        cGiuliana.Agregar(new CuentaPlata("00005", 100000));
        cGiuliana.Agregar(new CuentaOro("32034", 20000));

        // Asigno los clientes a los bancos
        var nac = new Banco("Banco Nacion");
        nac.Agregar(cGabriel);
        nac.Agregar(cAgustin);
        nac.Agregar(cGiuliana);

        var mac = new Banco("Banco Macro");
        mac.Agregar(cEnrique);
        mac.Agregar(cLaura);
        mac.Agregar(cMarta);
        
        // Operaciones en Banco Nacion
        nac.Registrar(new Deposito(cGabriel.Cuentas[0], 100));
        nac.Registrar(new Retiro(cGabriel.Cuentas[1], 200));
        nac.Registrar(new Transferencia(cGabriel.Cuentas[0], cGabriel.Cuentas[1], 300));
        nac.Registrar(new Transferencia(cAgustin.Cuentas[0], cAgustin.Cuentas[1], 500));
        nac.Registrar(new Pago(cGabriel.Cuentas[1], 400));
        nac.Registrar(new Pago(cMarta.Cuentas[0], 500));
        nac.Registrar(new Deposito(cMarta.Cuentas[0], 2500));
        nac.Registrar(new Pago(cGiuliana.Cuentas[0], 27000));
        nac.Registrar(new Deposito(cGiuliana.Cuentas[1], 5000));

        // Operaciones en Banco Macro
        mac.Registrar(new Deposito(cEnrique.Cuentas[0], 100));
        mac.Registrar(new Retiro(cEnrique.Cuentas[0], 200));
        mac.Registrar(new Transferencia(cEnrique.Cuentas[0], cGabriel.Cuentas[1], 300));
        mac.Registrar(new Pago(cEnrique.Cuentas[0], 400));
        mac.Registrar(new Pago(cLaura.Cuentas[0], 2000));
        mac.Registrar(new Deposito(cLaura.Cuentas[0], 8000));

        // Informe
        nac.Informe();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_");
        Console.ResetColor();
        mac.Informe();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_");
        Console.ResetColor();
    }
}


