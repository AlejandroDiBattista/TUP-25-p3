using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable



abstract class Cuenta
{
    public string Numero { get; }
    public decimal Saldo { get; protected set; }
    public decimal Puntos { get; protected set; }
    public Cliente? Titular { get; set; }

    protected Cuenta(string numero, decimal saldoInicial)
    {
        Numero = numero;
        Saldo = saldoInicial;
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

    public abstract void Pagar(decimal monto);
}

class CuentaOro : Cuenta
{
    public CuentaOro(string numero, decimal saldoInicial) : base(numero, saldoInicial) { }

    public override void Pagar(decimal monto)
    {
        if (Saldo >= monto)
        {
            Saldo -= monto;
            Puntos += monto > 1000 ? monto * 0.05m : monto * 0.03m;
        }
    }
}

class CuentaPlata : Cuenta
{
    public CuentaPlata(string numero, decimal saldoInicial) : base(numero, saldoInicial) { }

    public override void Pagar(decimal monto)
    {
        if (Saldo >= monto)
        {
            Saldo -= monto;
            Puntos += monto * 0.02m;
        }
    }
}

class CuentaBronce : Cuenta
{
    public CuentaBronce(string numero, decimal saldoInicial) : base(numero, saldoInicial) { }

    public override void Pagar(decimal monto)
    {
        if (Saldo >= monto)
        {
            Saldo -= monto;
            Puntos += monto * 0.01m;
        }
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
        cuenta.Titular = this;
        Cuentas.Add(cuenta);
    }

    public decimal SaldoTotal => Cuentas.Sum(c => c.Saldo);
    public decimal PuntosTotal => Cuentas.Sum(c => c.Puntos);
}

abstract class Operacion
{
    public decimal Monto { get; }
    public string Descripcion { get; protected set; } = "";

    protected Operacion(decimal monto)
    {
        Monto = monto;
    }

    public abstract void Ejecutar(Banco banco);
}

class Deposito : Operacion
{
    public string CuentaDestino { get; }

    public Deposito(string cuentaDestino, decimal monto) : base(monto)
    {
        CuentaDestino = cuentaDestino;
    }

    public override void Ejecutar(Banco banco)
    {
        var cuenta = banco.BuscarCuenta(CuentaDestino);
        if (cuenta != null)
        {
            cuenta.Depositar(Monto);
            Descripcion = $"Deposito $ {Monto:F2} a [{cuenta.Numero}/{cuenta.Titular?.Nombre}]";
            banco.RegistrarOperacion(this, cuenta.Titular);
        }
    }
}

class Retiro : Operacion
{
    public string CuentaOrigen { get; }

    public Retiro(string cuentaOrigen, decimal monto) : base(monto)
    {
        CuentaOrigen = cuentaOrigen;
    }

    public override void Ejecutar(Banco banco)
    {
        var cuenta = banco.BuscarCuenta(CuentaOrigen);
        if (cuenta != null && cuenta.Extraer(Monto))
        {
            Descripcion = $"Retiro $ {Monto:F2} de [{cuenta.Numero}/{cuenta.Titular?.Nombre}]";
            banco.RegistrarOperacion(this, cuenta.Titular);
        }
    }
}

class Pago : Operacion
{
    public string CuentaOrigen { get; }

    public Pago(string cuentaOrigen, decimal monto) : base(monto)
    {
        CuentaOrigen = cuentaOrigen;
    }

    public override void Ejecutar(Banco banco)
    {
        var cuenta = banco.BuscarCuenta(CuentaOrigen);
        if (cuenta != null && cuenta.Saldo >= Monto)
        {
            cuenta.Pagar(Monto);
            Descripcion = $"Pago $ {Monto:F2} con [{cuenta.Numero}/{cuenta.Titular?.Nombre}]";
            banco.RegistrarOperacion(this, cuenta.Titular);
        }
    }
}

class Transferencia : Operacion
{
    public string CuentaOrigen { get; }
    public string CuentaDestino { get; }

    public Transferencia(string cuentaOrigen, string cuentaDestino, decimal monto) : base(monto)
    {
        CuentaOrigen = cuentaOrigen;
        CuentaDestino = cuentaDestino;
    }

    public override void Ejecutar(Banco banco)
    {
        var origen = banco.BuscarCuenta(CuentaOrigen);
        var destino = banco.BuscarCuenta(CuentaDestino);
        if (origen != null && destino != null && origen.Extraer(Monto))
        {
            destino.Depositar(Monto);
            Descripcion = $"Transferencia $ {Monto:F2} de [{origen.Numero}/{origen.Titular?.Nombre}] a [{destino.Numero}/{destino.Titular?.Nombre}]";
            banco.RegistrarOperacion(this, origen.Titular);
        }
    }
}

class Banco
{
    public string Nombre { get; }
    public List<Cliente> Clientes { get; }
    private List<Operacion> HistorialGlobal { get; }
    private Dictionary<string, Cuenta> TodasLasCuentas { get; }

    public Banco(string nombre)
    {
        Nombre = nombre;
        Clientes = new List<Cliente>();
        HistorialGlobal = new List<Operacion>();
        TodasLasCuentas = new Dictionary<string, Cuenta>();
    }

    public void Agregar(Cliente cliente)
    {
        Clientes.Add(cliente);
        foreach (var cuenta in cliente.Cuentas)
        {
            TodasLasCuentas[cuenta.Numero] = cuenta;
        }
    }

    public Cuenta? BuscarCuenta(string numero)
    {
        return TodasLasCuentas.TryGetValue(numero, out var cuenta) ? cuenta : null;
    }

    public void Registrar(Operacion operacion)
    {
        operacion.Ejecutar(this);
    }

    public void RegistrarOperacion(Operacion operacion, Cliente? cliente)
    {
        HistorialGlobal.Add(operacion);
        cliente?.Historial.Add(operacion);
    }

    public void Informe()
    {
        Console.WriteLine($"\nBanco: {Nombre} | Clientes: {Clientes.Count}\n");

        foreach (var cliente in Clientes)
        {
            Console.WriteLine($"  Cliente: {cliente.Nombre} | Saldo Total: $ {cliente.SaldoTotal:F2} | Puntos Total: $ {cliente.PuntosTotal:F2}\n");

            foreach (var cuenta in cliente.Cuentas)
            {
                Console.WriteLine($"    Cuenta: {cuenta.Numero} | Saldo: $ {cuenta.Saldo:F2} | Puntos: $ {cuenta.Puntos:F2}");
                foreach (var op in cliente.Historial.Where(o => o.Descripcion.Contains(cuenta.Numero)))
                {
                    Console.WriteLine($"     -  {op.Descripcion}");
                }
                Console.WriteLine();
            }
        }
    }
}



var raul = new Cliente("Raul Perez");
raul.Agregar(new CuentaOro("10001", 1000));
raul.Agregar(new CuentaPlata("10002", 2000));

var sara = new Cliente("Sara Lopez");
sara.Agregar(new CuentaPlata("10003", 3000));
sara.Agregar(new CuentaPlata("10004", 4000));

var luis = new Cliente("Luis Gomez");
luis.Agregar(new CuentaBronce("10005", 5000));

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
