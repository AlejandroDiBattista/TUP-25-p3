using System;
using System.Collections.Generic;
using System.Linq;

abstract class Cuenta
{
    public string Numero { get; }
    public double Saldo { get; protected set; }
    public double Puntos { get; protected set; }

    public Cuenta(string numero, double saldo)
    {
        Numero = numero;
        Saldo = saldo;
        Puntos = 0;
    }

    public virtual void Depositar(double monto) => Saldo += monto;

    public virtual bool Extraer(double monto)
    {
        if (Saldo >= monto)
        {
            Saldo -= monto;
            return true;
        }
        return false;
    }

    public virtual void Pagar(double monto)
    {
        Saldo -= monto;
        AcumularPuntos(monto);
    }

    public virtual void AcumularPuntos(double monto) { }
}

class CuentaOro : Cuenta
{
    public CuentaOro(string numero, double saldo) : base(numero, saldo) { }

    public override void AcumularPuntos(double monto)
    {
        Puntos += monto > 1000 ? monto * 0.05 : monto * 0.03;
    }
}

class CuentaPlata : Cuenta
{
    public CuentaPlata(string numero, double saldo) : base(numero, saldo) { }

    public override void AcumularPuntos(double monto)
    {
        Puntos += monto * 0.02;
    }
}

class CuentaBronce : Cuenta
{
    public CuentaBronce(string numero, double saldo) : base(numero, saldo) { }

    public override void AcumularPuntos(double monto)
    {
        Puntos += monto * 0.01;
    }
}

class Cliente
{
    public string Nombre { get; }
    public List<Cuenta> Cuentas { get; } = new();
    public List<Operacion> Operaciones { get; } = new();

    public Cliente(string nombre)
    {
        Nombre = nombre;
    }

    public void Agregar(Cuenta cuenta) => Cuentas.Add(cuenta);
    public Cuenta ObtenerCuenta(string numero) => Cuentas.FirstOrDefault(c => c.Numero == numero);
    public double SaldoTotal => Cuentas.Sum(c => c.Saldo);
    public double PuntosTotal => Cuentas.Sum(c => c.Puntos);
}

abstract class Operacion
{
    public double Monto { get; protected set; }
    public string CuentaOrigen { get; protected set; }
    public string CuentaDestino { get; protected set; }

    public abstract void Ejecutar(Banco banco);
    public abstract string Descripcion(Banco banco);
}

class Deposito : Operacion
{
    public Deposito(string cuenta, double monto)
    {
        CuentaOrigen = cuenta;
        Monto = monto;
    }

    public override void Ejecutar(Banco banco)
    {
        banco.BuscarCuenta(CuentaOrigen)?.Depositar(Monto);
    }

    public override string Descripcion(Banco banco)
    {
        var cliente = banco.BuscarClientePorCuenta(CuentaOrigen);
        return $"-  Deposito $ {Monto:N2} a [{CuentaOrigen}/{cliente?.Nombre}]";
    }
}

class Retiro : Operacion
{
    public Retiro(string cuenta, double monto)
    {
        CuentaOrigen = cuenta;
        Monto = monto;
    }

    public override void Ejecutar(Banco banco)
    {
        banco.BuscarCuenta(CuentaOrigen)?.Extraer(Monto);
    }

    public override string Descripcion(Banco banco)
    {
        var cliente = banco.BuscarClientePorCuenta(CuentaOrigen);
        return $"-  Retiro $ {Monto:N2} de [{CuentaOrigen}/{cliente?.Nombre}]";
    }
}

class Pago : Operacion
{
    public Pago(string cuenta, double monto)
    {
        CuentaOrigen = cuenta;
        Monto = monto;
    }

    public override void Ejecutar(Banco banco)
    {
        banco.BuscarCuenta(CuentaOrigen)?.Pagar(Monto);
    }

    public override string Descripcion(Banco banco)
    {
        var cliente = banco.BuscarClientePorCuenta(CuentaOrigen);
        return $"-  Pago $ {Monto:N2} con [{CuentaOrigen}/{cliente?.Nombre}]";
    }
}

class Transferencia : Operacion
{
    public Transferencia(string origen, string destino, double monto)
    {
        CuentaOrigen = origen;
        CuentaDestino = destino;
        Monto = monto;
    }

    public override void Ejecutar(Banco banco)
    {
        var origen = banco.BuscarCuenta(CuentaOrigen);
        var destino = banco.BuscarCuenta(CuentaDestino);
        if (origen != null && destino != null && origen.Extraer(Monto))
            destino.Depositar(Monto);
    }

    public override string Descripcion(Banco banco)
    {
        var clienteOrigen = banco.BuscarClientePorCuenta(CuentaOrigen);
        var clienteDestino = banco.BuscarClientePorCuenta(CuentaDestino);
        return $"-  Transferencia $ {Monto:N2} de [{CuentaOrigen}/{clienteOrigen?.Nombre}] a [{CuentaDestino}/{clienteDestino?.Nombre}]";
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

    public void Agregar(Cliente cliente) => clientes.Add(cliente);

    public Cuenta BuscarCuenta(string numero) =>
        clientes.SelectMany(c => c.Cuentas).FirstOrDefault(c => c.Numero == numero);

    public Cliente BuscarClientePorCuenta(string numero) =>
        clientes.FirstOrDefault(c => c.Cuentas.Any(cuenta => cuenta.Numero == numero));

    public void Registrar(Operacion op)
    {
        op.Ejecutar(this);
        operaciones.Add(op);
        var clienteOrigen = BuscarClientePorCuenta(op.CuentaOrigen);
        clienteOrigen?.Operaciones.Add(op);
        if (op is Transferencia && op.CuentaDestino != null)
        {
            var clienteDestino = BuscarClientePorCuenta(op.CuentaDestino);
            if (clienteDestino != null && clienteDestino != clienteOrigen)
                clienteDestino.Operaciones.Add(op);
        }
    }

    public void Informe()
    {
        Console.WriteLine($"\nBanco: {Nombre} | Clientes: {clientes.Count}");

        foreach (var cliente in clientes)
        {
            Console.WriteLine($"\n  Cliente: {cliente.Nombre} | Saldo Total: $ {cliente.SaldoTotal:N2} | Puntos Total: $ {cliente.PuntosTotal:N2}");
            foreach (var cuenta in cliente.Cuentas)
            {
                Console.WriteLine($"\n    Cuenta: {cuenta.Numero} | Saldo: $ {cuenta.Saldo:N2} | Puntos: $ {cuenta.Puntos:N2}");
                foreach (var op in cliente.Operaciones.Where(o => o.CuentaOrigen == cuenta.Numero || o.CuentaDestino == cuenta.Numero))
                {
                    Console.WriteLine("     " + op.Descripcion(this));
                }
            }
        }
    }
}

var raul = new Cliente("Raul Perez");
raul.Agregar(new CuentaOro("10001", 800));
raul.Agregar(new CuentaPlata("10002", 2000));

var sara = new Cliente("Sara Lopez");
sara.Agregar(new CuentaPlata("10003", 2500));
sara.Agregar(new CuentaPlata("10004", 4500));

var luis = new Cliente("Luis Gomez");
luis.Agregar(new CuentaBronce("10005", 4200));

var bancoNac = new Banco("Banco Nac");
bancoNac.Agregar(raul);
bancoNac.Agregar(sara);

var bancoTup = new Banco("Banco TUP");
bancoTup.Agregar(luis);

// Banco N1
bancoNac.Registrar(new Deposito("10001", 100));
bancoNac.Registrar(new Retiro("10002", 200));
bancoNac.Registrar(new Transferencia("10001", "10002", 300));
bancoNac.Registrar(new Transferencia("10003", "10004", 500));
bancoNac.Registrar(new Pago("10002", 400));

// Banco N2
bancoTup.Registrar(new Deposito("10005", 100));
bancoTup.Registrar(new Retiro("10005", 200));
bancoTup.Registrar(new Transferencia("10005", "10002", 300));
bancoTup.Registrar(new Pago("10005", 400));

bancoNac.Informe();
bancoTup.Informe();
