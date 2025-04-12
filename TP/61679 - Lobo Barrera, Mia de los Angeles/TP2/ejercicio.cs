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
    public List<Operacion> Historial { get; } = new List<Operacion>();

    public Cuenta(string numero, decimal saldoInicial, Cliente titular)
    {
        Numero = numero;
        Saldo = saldoInicial;
        Titular = titular;
    }

    public abstract void AcumularPuntos(decimal monto);

    public void Depositar(decimal monto)
    {
        Saldo += monto;
        var deposito = new Deposito(Numero, monto);
        RegistrarOperacion(deposito);
    }

    public bool Extraer(decimal monto)
    {
        if (Saldo >= monto)
        {
            Saldo -= monto;
            var retiro = new Retiro(Numero, monto);
            RegistrarOperacion(retiro);
            return true;
        }
        return false;
    }

    public void RegistrarOperacion(Operacion operacion)
    {
        Historial.Add(operacion);
        Titular.AgregarOperacion(operacion);
        Titular.Banco?.RegistrarOperacion(operacion);
    }
}

class CuentaOro : Cuenta
{
    public CuentaOro(string numero, decimal saldoInicial, Cliente titular) : base(numero, saldoInicial, titular) { }
    public override void AcumularPuntos(decimal monto) => Puntos += monto >= 1000 ? monto * 0.05m : monto * 0.03m;
}

class CuentaPlata : Cuenta
{
    public CuentaPlata(string numero, decimal saldoInicial, Cliente titular) : base(numero, saldoInicial, titular) { }
    public override void AcumularPuntos(decimal monto) => Puntos += monto * 0.02m;
}

class CuentaBronce : Cuenta
{
    public CuentaBronce(string numero, decimal saldoInicial, Cliente titular) : base(numero, saldoInicial, titular) { }
    public override void AcumularPuntos(decimal monto) => Puntos += monto * 0.01m;
}

abstract class Operacion
{
    public decimal Monto { get; }
    public abstract string Descripcion { get; }
    public string NumeroCuentaOrigen { get; protected set; }
    public string NumeroCuentaDestino { get; protected set; }

    protected Operacion(decimal monto)
    {
        Monto = monto;
    }
}

class Deposito : Operacion
{
    public override string Descripcion => $"Deposito $ {Monto:F2} a [{NumeroCuentaOrigen}]";
    public Deposito(string numeroCuenta, decimal monto) : base(monto)
    {
        NumeroCuentaOrigen = numeroCuenta;
    }
}

class Retiro : Operacion
{
    public override string Descripcion => $"Retiro $ {Monto:F2} de [{NumeroCuentaOrigen}]";
    public Retiro(string numeroCuenta, decimal monto) : base(monto)
    {
        NumeroCuentaOrigen = numeroCuenta;
    }
}

class Pago : Operacion
{
    public override string Descripcion => $"Pago $ {Monto:F2} con [{NumeroCuentaOrigen}]";
    public Pago(string numeroCuenta, decimal monto) : base(monto)
    {
        NumeroCuentaOrigen = numeroCuenta;
    }
}

class Transferencia : Operacion
{
    public override string Descripcion => $"Transferencia $ {Monto:F2} de [{NumeroCuentaOrigen}] a [{NumeroCuentaDestino}]";
    public Transferencia(string origen, string destino, decimal monto) : base(monto)
    {
        NumeroCuentaOrigen = origen;
        NumeroCuentaDestino = destino;
    }
}

class Cliente
{
    public string Nombre { get; }
    public List<Cuenta> Cuentas { get; } = new List<Cuenta>();
    public List<Operacion> HistorialOperaciones { get; } = new List<Operacion>();
    public Banco Banco { get; set; }

    public Cliente(string nombre)
    {
        Nombre = nombre;
    }

    public void Agregar(Cuenta cuenta)
    {
        Cuentas.Add(cuenta);
    }

    public void AgregarOperacion(Operacion operacion)
    {
        HistorialOperaciones.Add(operacion);
    }

    public Cuenta ObtenerCuenta(string numero) => Cuentas.FirstOrDefault(c => c.Numero == numero);
}

class Banco
{
    public string Nombre { get; }
    private List<Cliente> Clientes { get; } = new List<Cliente>();
    private List<Operacion> HistorialGlobal { get; } = new List<Operacion>();

    public Banco(string nombre)
    {
        Nombre = nombre;
    }

    public void Agregar(Cliente cliente)
    {
        cliente.Banco = this;
        Clientes.Add(cliente);
    }

    public void RegistrarOperacion(Operacion operacion)
    {
        HistorialGlobal.Add(operacion);
    }

    public Cuenta BuscarCuenta(string numero)
    {
        foreach (var cliente in Clientes)
        {
            var cuenta = cliente.ObtenerCuenta(numero);
            if (cuenta != null) return cuenta;
        }
        return null;
    }

    public void Registrar(Deposito deposito)
    {
        var cuenta = BuscarCuenta(deposito.NumeroCuentaOrigen);
        cuenta?.Depositar(deposito.Monto);
    }

    public void Registrar(Retiro retiro)
    {
        var cuenta = BuscarCuenta(retiro.NumeroCuentaOrigen);
        cuenta?.Extraer(retiro.Monto);
    }

    public void Registrar(Pago pago)
    {
        var cuenta = BuscarCuenta(pago.NumeroCuentaOrigen);
        if (cuenta != null && cuenta.Extraer(pago.Monto))
            cuenta.AcumularPuntos(pago.Monto);
    }

    public void Registrar(Transferencia transferencia)
    {
        var origen = BuscarCuenta(transferencia.NumeroCuentaOrigen);
        var destino = BuscarCuenta(transferencia.NumeroCuentaDestino);
        if (origen != null && destino != null && origen.Extraer(transferencia.Monto))
            destino.Depositar(transferencia.Monto);
    }

    public void Informe()
    {
        Console.WriteLine($"Banco: {Nombre} | Clientes: {Clientes.Count}\n");
        foreach (var cliente in Clientes)
        {
            decimal saldoTotal = cliente.Cuentas.Sum(c => c.Saldo);
            decimal puntosTotal = cliente.Cuentas.Sum(c => c.Puntos);
            Console.WriteLine($"  Cliente: {cliente.Nombre} | Saldo Total: $ {saldoTotal:F2} | Puntos Total: $ {puntosTotal:F2}\n");
            foreach (var cuenta in cliente.Cuentas)
            {
                Console.WriteLine($"    Cuenta: {cuenta.Numero} | Saldo: $ {cuenta.Saldo:F2} | Puntos: $ {cuenta.Puntos:F2}");
                foreach (var operacion in cuenta.Historial)
                {
                    Console.WriteLine($"     -  {operacion.Descripcion}");
                }
                Console.WriteLine();
            }
        }
    }
}

// EJEMPLO DE USO

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