using System;
using System.Collections.Generic;
using System.Globalization;

abstract class Cuenta
{
    public string Numero { get; }
    public decimal Saldo { get; protected set; }
    public decimal Puntos { get; protected set; }

    public Cliente Titular { get; }

    public List<Operacion> Operaciones { get; } = new();

    public Cuenta(string numero, decimal saldoInicial, Cliente titular)
    {
        Numero = numero;
        Saldo = saldoInicial;
        Titular = titular;
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

    public virtual void Pagar(decimal monto)
    {
        if (Extraer(monto))
        {
            var puntos = CalcularPuntos(monto);
            Puntos += puntos;
        }
    }

    protected abstract decimal CalcularPuntos(decimal monto);
}

class CuentaOro : Cuenta
{
    public CuentaOro(string numero, decimal saldoInicial, Cliente titular) : base(numero, saldoInicial, titular) { }

    protected override decimal CalcularPuntos(decimal monto)
    {
        return monto > 1000 ? monto * 0.05m : monto * 0.03m;
    }
}

class CuentaPlata : Cuenta
{
    public CuentaPlata(string numero, decimal saldoInicial, Cliente titular) : base(numero, saldoInicial, titular) { }

    protected override decimal CalcularPuntos(decimal monto)
    {
        return monto * 0.02m;
    }
}

class CuentaBronce : Cuenta
{
    public CuentaBronce(string numero, decimal saldoInicial, Cliente titular) : base(numero, saldoInicial, titular) { }

    protected override decimal CalcularPuntos(decimal monto)
    {
        return monto * 0.01m;
    }
}

class Cliente
{
    public string Nombre { get; }
    public List<Cuenta> Cuentas { get; } = new();
    public List<Operacion> Historial { get; } = new();

    public Cliente(string nombre)
    {
        Nombre = nombre;
    }

    public void Agregar(Cuenta cuenta)
    {
        Cuentas.Add(cuenta);
    }

    public decimal TotalSaldo => Sumar(c => c.Saldo);
    public decimal TotalPuntos => Sumar(c => c.Puntos);

    private decimal Sumar(Func<Cuenta, decimal> selector)
    {
        decimal total = 0;
        foreach (var cuenta in Cuentas)
            total += selector(cuenta);
        return total;
    }

    public Cuenta BuscarCuenta(string numero)
    {
        foreach (var c in Cuentas)
            if (c.Numero == numero) return c;
        return null;
    }
}

abstract class Operacion
{
    public decimal Monto { get; }
    public string CuentaOrigen { get; }
    public string CuentaDestino { get; }

    protected Operacion(string origen, decimal monto, string destino = null)
    {
        CuentaOrigen = origen;
        CuentaDestino = destino;
        Monto = monto;
    }

    public abstract void Ejecutar(Banco banco);
    public abstract string Descripcion(Dictionary<string, Cuenta> mapaCuentas);
}

class Deposito : Operacion
{
    public Deposito(string cuenta, decimal monto) : base(cuenta, monto) { }

    public override void Ejecutar(Banco banco)
    {
        var cuenta = banco.ObtenerCuenta(CuentaOrigen);
        cuenta?.Depositar(Monto);
        cuenta?.Operaciones.Add(this);
        cuenta?.Titular.Historial.Add(this);
        banco.Operaciones.Add(this);
    }

    public override string Descripcion(Dictionary<string, Cuenta> mapa)
    {
        var c = mapa[CuentaOrigen];
        return $" -  Deposito $ {Monto:N2} a [{CuentaOrigen}/{c.Titular.Nombre}]";
    }
}

class Retiro : Operacion
{
    public Retiro(string cuenta, decimal monto) : base(cuenta, monto) { }

    public override void Ejecutar(Banco banco)
    {
        var cuenta = banco.ObtenerCuenta(CuentaOrigen);
        if (cuenta != null && cuenta.Extraer(Monto))
        {
            cuenta.Operaciones.Add(this);
            cuenta.Titular.Historial.Add(this);
            banco.Operaciones.Add(this);
        }
    }

    public override string Descripcion(Dictionary<string, Cuenta> mapa)
    {
        var c = mapa[CuentaOrigen];
        return $" -  Retiro $ {Monto:N2} de [{CuentaOrigen}/{c.Titular.Nombre}]";
    }
}

class Pago : Operacion
{
    public Pago(string cuenta, decimal monto) : base(cuenta, monto) { }

    public override void Ejecutar(Banco banco)
    {
        var cuenta = banco.ObtenerCuenta(CuentaOrigen);
        if (cuenta != null)
        {
            var saldoAntes = cuenta.Saldo;
            cuenta.Pagar(Monto);
            if (saldoAntes != cuenta.Saldo)
            {
                cuenta.Operaciones.Add(this);
                cuenta.Titular.Historial.Add(this);
                banco.Operaciones.Add(this);
            }
        }
    }

    public override string Descripcion(Dictionary<string, Cuenta> mapa)
    {
        var c = mapa[CuentaOrigen];
        return $" -  Pago $ {Monto:N2} con [{CuentaOrigen}/{c.Titular.Nombre}]";
    }
}

class Transferencia : Operacion
{
    public Transferencia(string origen, string destino, decimal monto) : base(origen, monto, destino) { }

    public override void Ejecutar(Banco banco)
    {
        var origen = banco.ObtenerCuenta(CuentaOrigen);
        var destino = banco.ObtenerCuenta(CuentaDestino);
        if (origen != null && destino != null && origen.Extraer(Monto))
        {
            destino.Depositar(Monto);
            origen.Operaciones.Add(this);
            destino.Operaciones.Add(this);
            origen.Titular.Historial.Add(this);
            destino.Titular.Historial.Add(this);
            banco.Operaciones.Add(this);
        }
    }

    public override string Descripcion(Dictionary<string, Cuenta> mapa)
    {
        var o = mapa[CuentaOrigen];
        var d = mapa[CuentaDestino];
        return $" -  Transferencia $ {Monto:N2} de [{CuentaOrigen}/{o.Titular.Nombre}] a [{CuentaDestino}/{d.Titular.Nombre}]";
    }
}

class Banco
{
    public string Nombre { get; }
    private List<Cliente> clientes = new();
    public List<Operacion> Operaciones { get; } = new();

    private Dictionary<string, Cuenta> mapaCuentas = new();

    public Banco(string nombre)
    {
        Nombre = nombre;
    }

    public void Agregar(Cliente cliente)
    {
        clientes.Add(cliente);
        foreach (var c in cliente.Cuentas)
            mapaCuentas[c.Numero] = c;
    }

    public void Registrar(Operacion op)
    {
        if (op is Transferencia t && !mapaCuentas.ContainsKey(t.CuentaOrigen)) return;
        if (!mapaCuentas.ContainsKey(op.CuentaOrigen)) return;

        op.Ejecutar(this);
    }

    public Cuenta ObtenerCuenta(string numero)
    {
        return mapaCuentas.TryGetValue(numero, out var cuenta) ? cuenta : null;
    }

    public void Informe()
    {
        Console.WriteLine($"\nBanco: {Nombre} | Clientes: {clientes.Count}");
        foreach (var cliente in clientes)
        {
            Console.WriteLine($"\n  Cliente: {cliente.Nombre} | Saldo Total: $ {cliente.TotalSaldo:N2} | Puntos Total: $ {cliente.TotalPuntos:N2}");
            foreach (var cuenta in cliente.Cuentas)
            {
                Console.WriteLine($"\n    Cuenta: {cuenta.Numero} | Saldo: $ {cuenta.Saldo:N2} | Puntos: $ {cuenta.Puntos:N2}");
                foreach (var op in cuenta.Operaciones)
                    Console.WriteLine(op.Descripcion(mapaCuentas));
            }
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