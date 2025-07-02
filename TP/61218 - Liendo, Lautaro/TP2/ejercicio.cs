using System;
using System.Collections.Generic;
using System.Linq;

abstract class Cuenta
{
    public string Numero { get; }
    public decimal Saldo { get; private set; }
    public decimal Puntos { get; protected set; }

    public Cliente Titular { get; }

    public Cuenta(string numero, decimal saldoInicial, Cliente titular)
    {
        Numero = numero;
        Saldo = saldoInicial;
        Titular = titular;
    }

    public void Depositar(decimal monto)
    {
        Saldo += monto;
    }

    public bool Extraer(decimal monto)
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
            AcumularPuntos(monto);
    }

    protected abstract void AcumularPuntos(decimal monto);

    public void Recibir(decimal monto)
    {
        Saldo += monto;
    }

    public override string ToString()
    {
        return $"Cuenta: {Numero} | Saldo: $ {Saldo:0.00} | Puntos: $ {Puntos:0.00}";
    }
}

class CuentaOro : Cuenta
{
    public CuentaOro(string numero, decimal saldoInicial, Cliente titular) : base(numero, saldoInicial, titular) { }

    protected override void AcumularPuntos(decimal monto)
    {
        if (monto > 1000)
            Puntos += monto * 0.05m;
        else
            Puntos += monto * 0.03m;
    }
}

class CuentaPlata : Cuenta
{
    public CuentaPlata(string numero, decimal saldoInicial, Cliente titular) : base(numero, saldoInicial, titular) { }

    protected override void AcumularPuntos(decimal monto)
    {
        Puntos += monto * 0.02m;
    }
}

class CuentaBronce : Cuenta
{
    public CuentaBronce(string numero, decimal saldoInicial, Cliente titular) : base(numero, saldoInicial, titular) { }

    protected override void AcumularPuntos(decimal monto)
    {
        Puntos += monto * 0.01m;
    }
}

class Cliente
{
    public string Nombre { get; }
    public List<Cuenta> Cuentas { get; }
    public List<Operacion> Historial { get; }

    public Cliente(string nombre)
    {
        Nombre = nombre;
        Cuentas = new List<Cuenta>();
        Historial = new List<Operacion>();
    }

    public void Agregar(Cuenta cuenta)
    {
        Cuentas.Add(cuenta);
    }

    public Cuenta BuscarCuenta(string numero)
    {
        return Cuentas.FirstOrDefault(c => c.Numero == numero);
    }

    public decimal SaldoTotal => Cuentas.Sum(c => c.Saldo);
    public decimal PuntosTotal => Cuentas.Sum(c => c.Puntos);
}

abstract class Operacion
{
    public decimal Monto { get; }
    public string CuentaOrigen { get; }
    public string CuentaDestino { get; }

    public Operacion(decimal monto, string origen, string destino = "")
    {
        Monto = monto;
        CuentaOrigen = origen;
        CuentaDestino = destino;
    }

    public abstract void Ejecutar(Banco banco);
    public abstract string Detalle(Banco banco);
}

class Deposito : Operacion
{
    public Deposito(string destino, decimal monto) : base(monto, "", destino) { }

    public override void Ejecutar(Banco banco)
    {
        var cuenta = banco.BuscarCuenta(CuentaDestino);
        cuenta?.Depositar(Monto);
        cuenta?.Titular.Historial.Add(this);
    }

    public override string Detalle(Banco banco)
    {
        var cuenta = banco.BuscarCuenta(CuentaDestino);
        return $"-  Deposito $ {Monto:0.00} a [{CuentaDestino}/{cuenta.Titular.Nombre}]";
    }
}

class Retiro : Operacion
{
    public Retiro(string origen, decimal monto) : base(monto, origen) { }

    public override void Ejecutar(Banco banco)
    {
        var cuenta = banco.BuscarCuenta(CuentaOrigen);
        if (cuenta != null && cuenta.Extraer(Monto))
            cuenta.Titular.Historial.Add(this);
    }

    public override string Detalle(Banco banco)
    {
        var cuenta = banco.BuscarCuenta(CuentaOrigen);
        return $"-  Retiro $ {Monto:0.00} de [{CuentaOrigen}/{cuenta.Titular.Nombre}]";
    }
}

class Pago : Operacion
{
    public Pago(string origen, decimal monto) : base(monto, origen) { }

    public override void Ejecutar(Banco banco)
    {
        var cuenta = banco.BuscarCuenta(CuentaOrigen);
        cuenta?.Pagar(Monto);
        cuenta?.Titular.Historial.Add(this);
    }

    public override string Detalle(Banco banco)
    {
        var cuenta = banco.BuscarCuenta(CuentaOrigen);
        return $"-  Pago $ {Monto:0.00} con [{CuentaOrigen}/{cuenta.Titular.Nombre}]";
    }
}

class Transferencia : Operacion
{
    public Transferencia(string origen, string destino, decimal monto) : base(monto, origen, destino) { }

    public override void Ejecutar(Banco banco)
    {
        var origen = banco.BuscarCuenta(CuentaOrigen);
        var destino = banco.BuscarCuenta(CuentaDestino);

        if (origen != null && destino != null && origen.Extraer(Monto))
        {
            destino.Recibir(Monto);
            origen.Titular.Historial.Add(this);
            destino.Titular.Historial.Add(this);
        }
    }

    public override string Detalle(Banco banco)
    {
        var origen = banco.BuscarCuenta(CuentaOrigen);
        var destino = banco.BuscarCuenta(CuentaDestino);
        return $"-  Transferencia $ {Monto:0.00} de [{CuentaOrigen}/{origen.Titular.Nombre}] a [{CuentaDestino}/{destino.Titular.Nombre}]";
    }
}

class Banco
{
    public string Nombre { get; }
    public List<Cliente> Clientes { get; }
    public List<Operacion> Registro { get; }

    public Banco(string nombre)
    {
        Nombre = nombre;
        Clientes = new List<Cliente>();
        Registro = new List<Operacion>();
    }

    public void Agregar(Cliente cliente)
    {
        Clientes.Add(cliente);
    }

    public Cuenta BuscarCuenta(string numero)
    {
        foreach (var cliente in Clientes)
        {
            var cuenta = cliente.BuscarCuenta(numero);
            if (cuenta != null)
                return cuenta;
        }
        return null;
    }

    public void Registrar(Operacion op)
    {
        var cuenta = BuscarCuenta(op.CuentaOrigen);
        if (op is Deposito || cuenta != null)
        {
            op.Ejecutar(this);
            Registro.Add(op);
        }
    }

    public void Informe()
    {
        Console.WriteLine($"\nBanco: {Nombre} | Clientes: {Clientes.Count}");

        foreach (var cliente in Clientes)
        {
            Console.WriteLine($"\n  Cliente: {cliente.Nombre} | Saldo Total: $ {cliente.SaldoTotal:0.00} | Puntos Total: $ {cliente.PuntosTotal:0.00}");

            foreach (var cuenta in cliente.Cuentas)
            {
                Console.WriteLine($"\n    {cuenta}");

                foreach (var op in cliente.Historial.Where(o => o.CuentaOrigen == cuenta.Numero || o.CuentaDestino == cuenta.Numero))
                    Console.WriteLine("     " + op.Detalle(this));
            }
        }
    }
}

// ---------------- EJEMPLO DE USO --------------------

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
