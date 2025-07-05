using System;
using System.Collections.Generic;


abstract class Cuenta
{
    public string Numero { get; }
    public double Saldo { get; set; }
    public double Puntos { get; set; }
    public Cliente Titular { get; }

    public Cuenta(string numero, double saldo, Cliente titular)
    {
        Numero = numero;
        Saldo = saldo;
        Titular = titular;
    }

    public abstract void AplicarPuntos(double monto);
}

class CuentaOro : Cuenta
{
    public CuentaOro(string numero, double saldo, Cliente titular) : base(numero, saldo, titular) { }

    public override void AplicarPuntos(double monto)
    {
        if (monto > 1000) Puntos += monto * 0.05;
        else Puntos += monto * 0.03;
    }
}

class CuentaPlata : Cuenta
{
    public CuentaPlata(string numero, double saldo, Cliente titular) : base(numero, saldo, titular) { }

    public override void AplicarPuntos(double monto)
    {
        Puntos += monto * 0.02;
    }
}

class CuentaBronce : Cuenta
{
    public CuentaBronce(string numero, double saldo, Cliente titular) : base(numero, saldo, titular) { }

    public override void AplicarPuntos(double monto)
    {
        Puntos += monto * 0.01;
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

    public double SaldoTotal => Cuentas.Sum(c => c.Saldo);
    public double PuntosTotal => Cuentas.Sum(c => c.Puntos);
}


abstract class Operacion
{
    public double Monto { get; }
    public abstract void Ejecutar(Banco banco);
    public abstract string Descripcion();

    public Operacion(double monto)
    {
        Monto = monto;
    }
}

class Deposito : Operacion
{
    string CuentaDestino;

    public Deposito(string cuentaDestino, double monto) : base(monto)
    {
        CuentaDestino = cuentaDestino;
    }

    public override void Ejecutar(Banco banco)
    {
        var cuenta = banco.BuscarCuenta(CuentaDestino);
        if (cuenta != null)
        {
            cuenta.Saldo += Monto;
            banco.RegistrarOperacion(this, cuenta.Titular);
        }
    }

    public override string Descripcion()
    {
        return $"Deposito $ {Monto:0.00} a [{CuentaDestino}]";
    }
}

class Retiro : Operacion
{
    string CuentaOrigen;

    public Retiro(string cuentaOrigen, double monto) : base(monto)
    {
        CuentaOrigen = cuentaOrigen;
    }

    public override void Ejecutar(Banco banco)
    {
        var cuenta = banco.BuscarCuenta(CuentaOrigen);
        if (cuenta != null && cuenta.Saldo >= Monto)
        {
            cuenta.Saldo -= Monto;
            banco.RegistrarOperacion(this, cuenta.Titular);
        }
    }

    public override string Descripcion()
    {
        return $"Retiro $ {Monto:0.00} de [{CuentaOrigen}]";
    }
}

class Pago : Operacion
{
    string Cuenta;

    public Pago(string cuenta, double monto) : base(monto)
    {
        Cuenta = cuenta;
    }

    public override void Ejecutar(Banco banco)
    {
        var c = banco.BuscarCuenta(Cuenta);
        if (c != null && c.Saldo >= Monto)
        {
            c.Saldo -= Monto;
            c.AplicarPuntos(Monto);
            banco.RegistrarOperacion(this, c.Titular);
        }
    }

    public override string Descripcion()
    {
        return $"Pago $ {Monto:0.00} con [{Cuenta}]";
    }
}

class Transferencia : Operacion
{
    string Origen, Destino;

    public Transferencia(string origen, string destino, double monto) : base(monto)
    {
        Origen = origen;
        Destino = destino;
    }

    public override void Ejecutar(Banco banco)
    {
        var origen = banco.BuscarCuenta(Origen);
        var destino = banco.BuscarCuenta(Destino);
        if (origen != null && destino != null && origen.Saldo >= Monto)
        {
            origen.Saldo -= Monto;
            destino.Saldo += Monto;
            banco.RegistrarOperacion(this, origen.Titular);
            if (origen.Titular != destino.Titular)
                banco.RegistrarOperacion(this, destino.Titular);
        }
    }

    public override string Descripcion()
    {
        return $"Transferencia $ {Monto:0.00} de [{Origen}] a [{Destino}]";
    }
}


class Banco
{
    public string Nombre { get; }
    public List<Cliente> Clientes { get; } = new List<Cliente>();
    public List<Operacion> HistorialGlobal { get; } = new List<Operacion>();

    public Banco(string nombre)
    {
        Nombre = nombre;
    }

    public void Agregar(Cliente cliente)
    {
        Clientes.Add(cliente);
    }

    public Cuenta BuscarCuenta(string numero)
    {
        return Clientes.SelectMany(c => c.Cuentas).FirstOrDefault(c => c.Numero == numero);
    }

    public void Registrar(Operacion operacion)
    {
        operacion.Ejecutar(this);
    }

    public void RegistrarOperacion(Operacion operacion, Cliente cliente)
    {
        cliente.Historial.Add(operacion);
        HistorialGlobal.Add(operacion);
    }

    public void Informe()
    {
        Console.WriteLine($"\nBanco: {Nombre} | Clientes: {Clientes.Count}\n");

        foreach (var cliente in Clientes)
        {
            Console.WriteLine($"  Cliente: {cliente.Nombre} | Saldo Total: $ {cliente.SaldoTotal:0.00} | Puntos Total: $ {cliente.PuntosTotal:0.00}");

            foreach (var cuenta in cliente.Cuentas)
            {
                Console.WriteLine($"\n    Cuenta: {cuenta.Numero} | Saldo: $ {cuenta.Saldo:0.00} | Puntos: $ {cuenta.Puntos:0.00}");

                foreach (var op in cliente.Historial.Where(op => op.Descripcion().Contains(cuenta.Numero)))
    Console.WriteLine($"     -  {op.Descripcion()}");

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
        raul.AgregarCuenta(new CuentaOro("10001", 1000, raul));
        raul.AgregarCuenta(new CuentaPlata("10002", 2000, raul));

        var sara = new Cliente("Sara Lopez");
        sara.AgregarCuenta(new CuentaPlata("10003", 3000, sara));
        sara.AgregarCuenta(new CuentaPlata("10004", 4000, sara));

        var luis = new Cliente("Luis Gomez");
        luis.AgregarCuenta(new CuentaBronce("10005", 5000, luis));

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
