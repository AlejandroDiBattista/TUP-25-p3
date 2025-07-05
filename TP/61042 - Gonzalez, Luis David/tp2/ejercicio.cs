// TP2: Sistema de Cuentas Bancarias
//

// Implementar un sistema de cuentas bancarias que permita realizar operaciones como depósitos, retiros, transferencias y pagos.

// Sistema Bancario en C#

using System;
using System.Collections.Generic;

// =========================
// Clases base y jerarquía
// =========================

abstract class Cuenta
{
    public string Numero { get; }
    public decimal Saldo { get; protected set; }
    public decimal Puntos { get; protected set; }

    protected List<Operacion> historial = new List<Operacion>();

    public Cuenta(string numero, decimal saldo)
    {
        Numero = numero;
        Saldo = saldo;
        Puntos = 0;
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

    public void AgregarOperacion(Operacion operacion)
    {
        historial.Add(operacion);
    }

    public IEnumerable<Operacion> Historial => historial;

    public abstract void AcumularPuntos(decimal monto);
}

class CuentaOro : Cuenta
{
    public CuentaOro(string numero, decimal saldo) : base(numero, saldo) { }

    public override void AcumularPuntos(decimal monto)
    {
        if (monto > 1000)
            Puntos += monto * 0.05m;
        else
            Puntos += monto * 0.03m;
    }
}

class CuentaPlata : Cuenta
{
    public CuentaPlata(string numero, decimal saldo) : base(numero, saldo) { }

    public override void AcumularPuntos(decimal monto)
    {
        Puntos += monto * 0.02m;
    }
}

class CuentaBronce : Cuenta
{
    public CuentaBronce(string numero, decimal saldo) : base(numero, saldo) { }

    public override void AcumularPuntos(decimal monto)
    {
        Puntos += monto * 0.01m;
    }
}

// =========================
// Cliente
// =========================

class Cliente
{
    public string Nombre { get; }
    private List<Cuenta> cuentas = new List<Cuenta>();

    public Cliente(string nombre)
    {
        Nombre = nombre;
    }

    public void Agregar(Cuenta cuenta)
    {
        cuentas.Add(cuenta);
    }

    public IEnumerable<Cuenta> Cuentas => cuentas;

    public decimal SaldoTotal
    {
        get
        {
            decimal total = 0;
            foreach (var cuenta in cuentas)
            {
                total += cuenta.Saldo;
            }
            return total;
        }
    }

    public decimal PuntosTotal
    {
        get
        {
            decimal total = 0;
            foreach (var cuenta in cuentas)
            {
                total += cuenta.Puntos;
            }
            return total;
        }
    }
}

// =========================
// Operaciones
// =========================

abstract class Operacion
{
    public decimal Monto { get; }
    public string CuentaOrigen { get; }
    public string CuentaDestino { get; }

    protected Operacion(string cuentaOrigen, decimal monto, string cuentaDestino = null)
    {
        CuentaOrigen = cuentaOrigen;
        CuentaDestino = cuentaDestino;
        Monto = monto;
    }

    public abstract void Ejecutar(Banco banco);
    public abstract string Detalle(Banco banco);
}

class Deposito : Operacion
{
    public Deposito(string cuentaDestino, decimal monto) : base(null, monto, cuentaDestino) { }

    public override void Ejecutar(Banco banco)
    {
        var cuenta = banco.ObtenerCuenta(CuentaDestino);
        cuenta?.Depositar(Monto);
        cuenta?.AgregarOperacion(this);
    }

    public override string Detalle(Banco banco)
    {
        var titular = banco.ObtenerTitular(CuentaDestino);
        return $"-  Deposito $ {Monto:0.00} a [{CuentaDestino}/{titular}]";
    }
}

class Retiro : Operacion
{
    public Retiro(string cuentaOrigen, decimal monto) : base(cuentaOrigen, monto) { }

    public override void Ejecutar(Banco banco)
    {
        var cuenta = banco.ObtenerCuenta(CuentaOrigen);
        if (cuenta != null && cuenta.Extraer(Monto))
        {
            cuenta.AgregarOperacion(this);
        }
    }

    public override string Detalle(Banco banco)
    {
        var titular = banco.ObtenerTitular(CuentaOrigen);
        return $"-  Retiro $ {Monto:0.00} de [{CuentaOrigen}/{titular}]";
    }
}

class Pago : Operacion
{
    public Pago(string cuentaOrigen, decimal monto) : base(cuentaOrigen, monto) { }

    public override void Ejecutar(Banco banco)
    {
        var cuenta = banco.ObtenerCuenta(CuentaOrigen);
        if (cuenta != null && cuenta.Extraer(Monto))
        {
            cuenta.AcumularPuntos(Monto);
            cuenta.AgregarOperacion(this);
        }
    }

    public override string Detalle(Banco banco)
    {
        var titular = banco.ObtenerTitular(CuentaOrigen);
        return $"-  Pago $ {Monto:0.00} con [{CuentaOrigen}/{titular}]";
    }
}

class Transferencia : Operacion
{
    public Transferencia(string cuentaOrigen, string cuentaDestino, decimal monto) : base(cuentaOrigen, monto, cuentaDestino) { }

    public override void Ejecutar(Banco banco)
    {
        var origen = banco.ObtenerCuenta(CuentaOrigen);
        var destino = banco.ObtenerCuenta(CuentaDestino);
        if (origen != null && destino != null && origen.Extraer(Monto))
        {
            destino.Depositar(Monto);
            origen.AgregarOperacion(this);
            destino.AgregarOperacion(this);
        }
    }

    public override string Detalle(Banco banco)
    {
        var titularOrigen = banco.ObtenerTitular(CuentaOrigen);
        var titularDestino = banco.ObtenerTitular(CuentaDestino);
        return $"-  Transferencia $ {Monto:0.00} de [{CuentaOrigen}/{titularOrigen}] a [{CuentaDestino}/{titularDestino}]";
    }
}

// =========================
// Banco
// =========================

class Banco
{
    public string Nombre { get; }
    private List<Cliente> clientes = new List<Cliente>();
    private List<Operacion> operaciones = new List<Operacion>();

    private static List<Banco> todosLosBancos = new List<Banco>();

    public Banco(string nombre)
    {
        Nombre = nombre;
    }

    public void Agregar(Cliente cliente)
    {
        clientes.Add(cliente);
    }

    public void Registrar(Operacion operacion)
    {
        if (operacion is Transferencia trf)
        {
            var cuentaOrigen = ObtenerCuenta(trf.CuentaOrigen);
            if (cuentaOrigen != null)
            {
                operacion.Ejecutar(this);
                operaciones.Add(operacion);
            }
        }
        else
        {
            operacion.Ejecutar(this);
            operaciones.Add(operacion);
        }
    }

    public Cuenta ObtenerCuenta(string numero)
    {
        foreach (var cliente in clientes)
        {
            foreach (var cuenta in cliente.Cuentas)
            {
                if (cuenta.Numero == numero)
                {
                    return cuenta;
                }
            }
        }
        return null;
    }

    public string ObtenerTitular(string numeroCuenta)
    {
        foreach (var banco in todosLosBancos)
        {
            foreach (var cliente in banco.clientes)
            {
                foreach (var cuenta in cliente.Cuentas)
                {
                    if (cuenta.Numero == numeroCuenta)
                    {
                        return cliente.Nombre;
                    }
                }
            }
        }
        return "Desconocido";
    }

    public void Informe()
    {
        Console.WriteLine($"\nBanco: {Nombre} | Clientes: {clientes.Count}\n");

        foreach (var cliente in clientes)
        {
            Console.WriteLine($"  Cliente: {cliente.Nombre} | Saldo Total: $ {cliente.SaldoTotal:0.00} | Puntos Total: $ {cliente.PuntosTotal:0.00}");

            foreach (var cuenta in cliente.Cuentas)
            {
                Console.WriteLine($"\n    Cuenta: {cuenta.Numero} | Saldo: $ {cuenta.Saldo:0.00} | Puntos: $ {cuenta.Puntos:0.00}");

                foreach (var operacion in cuenta.Historial)
                {
                    Console.WriteLine("     " + operacion.Detalle(this));
                }
            }

            Console.WriteLine();
        }
    }

    public static void RegistrarBanco(Banco banco)
    {
        todosLosBancos.Add(banco);
    }
}

// =========================
// Ejecución del programa
// =========================

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
        Banco.RegistrarBanco(nac);

        var tup = new Banco("Banco TUP");
        tup.Agregar(luis);
        Banco.RegistrarBanco(tup);

        // Operaciones en Banco Nac
        nac.Registrar(new Deposito("10001", 100));
        nac.Registrar(new Retiro("10002", 200));
        nac.Registrar(new Transferencia("10001", "10002", 300));
        nac.Registrar(new Transferencia("10003", "10004", 500));
        nac.Registrar(new Pago("10002", 400));

        // Operaciones en Banco TUP
        tup.Registrar(new Deposito("10005", 100));
        tup.Registrar(new Retiro("10005", 200));
        tup.Registrar(new Transferencia("10005", "10002", 300));
        tup.Registrar(new Pago("10005", 400));

        // Informe final
        nac.Informe();
        tup.Informe();
