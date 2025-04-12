using System;
using System.Collections.Generic;
using System.Linq;

abstract class Operacion
{
    public decimal Monto { get; protected set; }
    public abstract void Ejecutar(Banco banco);
    public abstract string Detalle();
}

abstract class Cuenta
{
    public string Numero { get; }
    public decimal Saldo { get; protected set; }
    public decimal Puntos { get; protected set; }
    public Cliente Titular { get; }

    private List<Operacion> historial = new List<Operacion>();

    public Cuenta(string numero, decimal saldo, Cliente titular)
    {
        Numero = numero;
        Saldo = saldo;
        Titular = titular;
        Puntos = 0;
    }

    public void Depositar(decimal monto) => Saldo += monto;

    public bool Extraer(decimal monto)
    {
        if (Saldo >= monto)
        {
            Saldo -= monto;
            return true;
        }
        return false;
    }

    public void RegistrarOperacion(Operacion op) => historial.Add(op);

    public abstract void Pagar(decimal monto);

    public void Transferir(decimal monto, Cuenta destino)
    {
        if (Extraer(monto))
        {
            destino.Depositar(monto);
        }
    }

    public IEnumerable<Operacion> Historial => historial;

    public override string ToString() => $"Cuenta: {Numero} | Saldo: $ {Saldo:0.00} | Puntos: $ {Puntos:0.00}";
}

class CuentaOro : Cuenta
{
    public CuentaOro(string numero, decimal saldo, Cliente titular) : base(numero, saldo, titular) { }
    public override void Pagar(decimal monto)
    {
        if (Extraer(monto))
            Puntos += monto > 1000 ? monto * 0.05m : monto * 0.03m;
    }
}

class CuentaPlata : Cuenta
{
    public CuentaPlata(string numero, decimal saldo, Cliente titular) : base(numero, saldo, titular) { }
    public override void Pagar(decimal monto)
    {
        if (Extraer(monto)) Puntos += monto * 0.02m;
    }
}

class CuentaBronce : Cuenta
{
    public CuentaBronce(string numero, decimal saldo, Cliente titular) : base(numero, saldo, titular) { }
    public override void Pagar(decimal monto)
    {
        if (Extraer(monto)) Puntos += monto * 0.01m;
    }
}

class Cliente
{
    public string Nombre { get; }
    private List<Cuenta> cuentas = new List<Cuenta>();

    public Cliente(string nombre) => Nombre = nombre;

    public void Agregar(Cuenta cuenta) => cuentas.Add(cuenta);

    public Cuenta ObtenerCuenta(string numero) => cuentas.FirstOrDefault(c => c.Numero == numero);

    public IEnumerable<Cuenta> Cuentas => cuentas;
    public decimal SaldoTotal => cuentas.Sum(c => c.Saldo);
    public decimal PuntosTotal => cuentas.Sum(c => c.Puntos);

    public IEnumerable<Operacion> Historial => cuentas.SelectMany(c => c.Historial);

    public override string ToString() => $"Cliente: {Nombre} | Saldo Total: $ {SaldoTotal:0.00} | Puntos Total: $ {PuntosTotal:0.00}";
}

class Banco
{
    public string Nombre { get; }
    private List<Cliente> clientes = new List<Cliente>();
    private List<Operacion> operaciones = new List<Operacion>();

    public Banco(string nombre) => Nombre = nombre;

    public void Agregar(Cliente cliente) => clientes.Add(cliente);

    public void Registrar(Operacion operacion)
    {
        operacion.Ejecutar(this);
        operaciones.Add(operacion);
    }

    public Cuenta BuscarCuenta(string numero)
    {
        return clientes.SelectMany(c => c.Cuentas).FirstOrDefault(cta => cta.Numero == numero);
    }

    public void Informe()
    {
        Console.WriteLine($"\nBanco: {Nombre} | Clientes: {clientes.Count}");
        foreach (var cliente in clientes)
        {
            Console.WriteLine($"\n  {cliente}");
            foreach (var cuenta in cliente.Cuentas)
            {
                Console.WriteLine($"\n    {cuenta}");
                foreach (var op in cuenta.Historial)
                {
                    Console.WriteLine($"     -  {op.Detalle()}");
                }
            }
        }
    }
}

class Deposito : Operacion
{
    private string numeroCuenta;
    public Deposito(string numeroCuenta, decimal monto) { this.numeroCuenta = numeroCuenta; Monto = monto; }
    public override void Ejecutar(Banco banco)
    {
        var cuenta = banco.BuscarCuenta(numeroCuenta);
        cuenta?.Depositar(Monto);
        cuenta?.RegistrarOperacion(this);
    }
    public override string Detalle() => $"Deposito $ {Monto:0.00} a [{numeroCuenta}]";
}

class Retiro : Operacion
{
    private string numeroCuenta;
    public Retiro(string numeroCuenta, decimal monto) { this.numeroCuenta = numeroCuenta; Monto = monto; }
    public override void Ejecutar(Banco banco)
    {
        var cuenta = banco.BuscarCuenta(numeroCuenta);
        if (cuenta?.Extraer(Monto) == true)
            cuenta.RegistrarOperacion(this);
    }
    public override string Detalle() => $"Retiro $ {Monto:0.00} de [{numeroCuenta}]";
}

class Pago : Operacion
{
    private string numeroCuenta;
    public Pago(string numeroCuenta, decimal monto) { this.numeroCuenta = numeroCuenta; Monto = monto; }
    public override void Ejecutar(Banco banco)
    {
        var cuenta = banco.BuscarCuenta(numeroCuenta);
        cuenta?.Pagar(Monto);
        cuenta?.RegistrarOperacion(this);
    }
    public override string Detalle() => $"Pago $ {Monto:0.00} con [{numeroCuenta}]";
}

class Transferencia : Operacion
{
    private string origen;
    private string destino;
    public Transferencia(string origen, string destino, decimal monto)
    {
        this.origen = origen;
        this.destino = destino;
        Monto = monto;
    }
    public override void Ejecutar(Banco banco)
    {
        var ctaOrigen = banco.BuscarCuenta(origen);
        var ctaDestino = banco.BuscarCuenta(destino);
        if (ctaOrigen?.Extraer(Monto) == true)
        {
            ctaDestino?.Depositar(Monto);
            ctaOrigen.RegistrarOperacion(this);
            ctaDestino?.RegistrarOperacion(this);
        }
    }
    public override string Detalle() => $"Transferencia $ {Monto:0.00} de [{origen}] a [{destino}]";
}

class Program
{
    static void Main()
    {
        var anto = new Cliente("Antonella Sotto");
        anto.Agregar(new CuentaOro("10001", 1000, anto));
        anto.Agregar(new CuentaPlata("10002", 2000, anto));

        var fran = new Cliente("Franchesco Ramos");
        fran.Agregar(new CuentaPlata("10003", 3000, fran));
        fran.Agregar(new CuentaPlata("10004", 4000, fran));

        var julieta = new Cliente("Julieta Arias");
        julieta.Agregar(new CuentaBronce("10005", 5000, julieta));

        var nac = new Banco("Banco Nac");
        nac.Agregar(anto);
        nac.Agregar(fran);

        var tup = new Banco("Banco TUP");
        tup.Agregar(julieta);

        nac.Registrar(new Deposito("10001", 100));
        nac.Registrar(new Retiro("10002", 200));
        nac.Registrar(new Transferencia("10001", "10002", 300));
        nac.Registrar(new Transferencia("10003", "10004", 500));
        nac.Registrar(new Pago("10002", 400));

        tup.Registrar(new Deposito("10005", 100));
        tup.Registrar(new Retiro("10005", 200));
        tup.Registrar(new Transferencia("10005", "10002", 300));
        tup.Registrar(new Pago("10005", 400));

        nac.Informe();
        tup.Informe();
    }
}