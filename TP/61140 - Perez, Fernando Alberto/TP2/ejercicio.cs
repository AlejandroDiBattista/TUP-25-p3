// TP2: Sistema de Cuentas Bancarias
//

// Implementar un sistema de cuentas bancarias que permita realizar operaciones como depósitos, retiros, transferencias y pagos.

using System;
using System.Collections.Generic;
using System.Linq;

abstract class Cuenta
{
    public string Numero { get; }
    public decimal Saldo { get; protected set; }
    public decimal Puntos { get; protected set; }

    public Cliente Titular { get; }

    protected Cuenta(string numero, decimal saldoInicial, Cliente titular)
    {
        Numero = numero;
        Saldo = saldoInicial;
        Titular = titular;
        Puntos = 0;
    }

    public virtual void Depositar(decimal monto) => Saldo += monto;

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
        if (Saldo >= monto)
        {
            Saldo -= monto;
            AcumularPuntos(monto);
            return true;
        }
        return false;
    }

    protected abstract void AcumularPuntos(decimal monto);
}

class CuentaOro : Cuenta
{
    public CuentaOro(string numero, decimal saldoInicial, Cliente titular)
        : base(numero, saldoInicial, titular) { }

    protected override void AcumularPuntos(decimal monto)
    {
        Puntos += monto > 1000 ? monto * 0.05m : monto * 0.03m;
    }
}

class CuentaPlata : Cuenta
{
    public CuentaPlata(string numero, decimal saldoInicial, Cliente titular)
        : base(numero, saldoInicial, titular) { }

    protected override void AcumularPuntos(decimal monto)
    {
        Puntos += monto * 0.02m;
    }
}

class CuentaBronce : Cuenta
{
    public CuentaBronce(string numero, decimal saldoInicial, Cliente titular)
        : base(numero, saldoInicial, titular) { }

    protected override void AcumularPuntos(decimal monto)
    {
        Puntos += monto * 0.01m;
    }
}

class Cliente
{
    public string Nombre { get; }
    private List<Cuenta> cuentas = new();
    public List<Operacion> Historial { get; } = new();

    public Cliente(string nombre)
    {
        Nombre = nombre;
    }

    public void Agregar(Cuenta cuenta)
    {
        cuentas.Add(cuenta);
    }

    public IEnumerable<Cuenta> Cuentas => cuentas;

    public decimal SaldoTotal => cuentas.Sum(c => c.Saldo);
    public decimal PuntosTotales => cuentas.Sum(c => c.Puntos);
}

abstract class Operacion
{
    public decimal Monto { get; protected set; }

    public abstract void Ejecutar(Banco banco);
    public abstract string Descripcion(Banco banco);
}

class Deposito : Operacion
{
    string cuenta;

    public Deposito(string cuenta, decimal monto)
    {
        this.cuenta = cuenta;
        this.Monto = monto;
    }

    public override void Ejecutar(Banco banco)
    {
        var c = banco.BuscarCuenta(cuenta);
        c?.Depositar(Monto);
        banco.RegistrarOperacion(this);
        c?.Titular.Historial.Add(this);
    }

    public override string Descripcion(Banco banco)
    {
        var c = banco.BuscarCuenta(cuenta);
        return $"-  Deposito $ {Monto:0.00} a [{cuenta}/{c?.Titular.Nombre}]";
    }
}

class Retiro : Operacion
{
    string cuenta;

    public Retiro(string cuenta, decimal monto)
    {
        this.cuenta = cuenta;
        this.Monto = monto;
    }

    public override void Ejecutar(Banco banco)
    {
        var c = banco.BuscarCuenta(cuenta);
        if (c != null && c.Extraer(Monto))
        {
            banco.RegistrarOperacion(this);
            c.Titular.Historial.Add(this);
        }
    }

    public override string Descripcion(Banco banco)
    {
        var c = banco.BuscarCuenta(cuenta);
        return $"-  Retiro $ {Monto:0.00} de [{cuenta}/{c?.Titular.Nombre}]";
    }
}

class Pago : Operacion
{
    string cuenta;

    public Pago(string cuenta, decimal monto)
    {
        this.cuenta = cuenta;
        this.Monto = monto;
    }

    public override void Ejecutar(Banco banco)
    {
        var c = banco.BuscarCuenta(cuenta);
        if (c != null && c.Pagar(Monto))
        {
            banco.RegistrarOperacion(this);
            c.Titular.Historial.Add(this);
        }
    }

    public override string Descripcion(Banco banco)
    {
        var c = banco.BuscarCuenta(cuenta);
        return $"-  Pago $ {Monto:0.00} con [{cuenta}/{c?.Titular.Nombre}]";
    }
}

class Transferencia : Operacion
{
    string origen, destino;

    public Transferencia(string origen, string destino, decimal monto)
    {
        this.origen = origen;
        this.destino = destino;
        this.Monto = monto;
    }

    public override void Ejecutar(Banco banco)
    {
        var c1 = banco.BuscarCuenta(origen);
        var c2 = banco.BuscarCuenta(destino);

        if (c1 != null && c2 != null && c1.Extraer(Monto))
        {
            c2.Depositar(Monto);
            banco.RegistrarOperacion(this);
            c1.Titular.Historial.Add(this);
            if (c1.Titular != c2.Titular) c2.Titular.Historial.Add(this);
        }
    }

    public override string Descripcion(Banco banco)
    {
        var c1 = banco.BuscarCuenta(origen);
        var c2 = banco.BuscarCuenta(destino);
        return $"-  Transferencia $ {Monto:0.00} de [{origen}/{c1?.Titular.Nombre}] a [{destino}/{c2?.Titular.Nombre}]";
    }
}

class Banco
{
    public string Nombre { get; }
    private List<Cliente> clientes = new();
    private List<Operacion> operaciones = new();

    public Banco(string nombre)
    {
        Nombre = nombre;
    }

    public void Agregar(Cliente cliente)
    {
        clientes.Add(cliente);
    }

    public void Registrar(Operacion op)
    {
        op.Ejecutar(this);
    }

    public Cuenta BuscarCuenta(string numero)
    {
        return clientes.SelectMany(c => c.Cuentas).FirstOrDefault(c => c.Numero == numero);
    }

    public void RegistrarOperacion(Operacion op)
    {
        operaciones.Add(op);
    }

    public void Informe()
    {
        Console.WriteLine($"\nBanco: {Nombre} | Clientes: {clientes.Count}\n");

        foreach (var cliente in clientes)
        {
            Console.WriteLine($"  Cliente: {cliente.Nombre} | Saldo Total: $ {cliente.SaldoTotal:0.00} | Puntos Total: $ {cliente.PuntosTotales:0.00}");
            foreach (var cuenta in cliente.Cuentas)
            {
                Console.WriteLine($"\n    Cuenta: {cuenta.Numero} | Saldo: $ {cuenta.Saldo:0.00} | Puntos: $ {cuenta.Puntos:0.00}");

                foreach (var op in cliente.Historial.Where(o =>
                    (o is Deposito d && d.Descripcion(this).Contains(cuenta.Numero)) ||
                    (o is Retiro r && r.Descripcion(this).Contains(cuenta.Numero)) ||
                    (o is Pago p && p.Descripcion(this).Contains(cuenta.Numero)) ||
                    (o is Transferencia t && t.Descripcion(this).Contains(cuenta.Numero))))
                {
                    Console.WriteLine("     " + op.Descripcion(this));
                }
            }
            Console.WriteLine();
        }
    }
}
class Program
{
    static void Main()
    {
        var raul = new Cliente("Raul Perez");
        raul.Agregar(new CuentaOro("10001", 1000, raul));
        raul.Agregar(new CuentaPlata("10002", 2000, raul));

        var sara = new Cliente("Sara Lopez");
        sara.Agregar(new CuentaPlata("10003", 3000, sara));
        sara.Agregar(new CuentaPlata("10004", 4000, sara));

        var luis = new Cliente("Luis Gomez");
        luis.Agregar(new CuentaBronce("10005", 5000, luis));

        var nac = new Banco("Banco Nac");
        nac.Agregar(raul);
        nac.Agregar(sara);

        var tup = new Banco("Banco TUP");
        tup.Agregar(luis);

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
