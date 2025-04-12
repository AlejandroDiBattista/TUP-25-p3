

using System;
using System.Collections.Generic;

class Banco
{
    public string Nombre { get; }
    private List<Cliente> clientes = new();
    private List<Operacion> operaciones = new();

    public Banco(string nombre) => Nombre = nombre;

    public void Agregar(Cliente cliente) => clientes.Add(cliente);

    public void Registrar(Operacion operacion)
    {
        operaciones.Add(operacion);
        operacion.Ejecutar(clientes);
    }

    public void Informe()
    {
        Console.WriteLine($"\n--- Informe del banco {Nombre} ---");
        foreach (var cliente in clientes)
        {
            Console.WriteLine($"Cliente: {cliente.Nombre}");
            cliente.MostrarCuentas();
        }
    }
}

class Cliente
{
    public string Nombre { get; }
    private List<Cuenta> cuentas = new();

    public Cliente(string nombre) => Nombre = nombre;

    public void Agregar(Cuenta cuenta) => cuentas.Add(cuenta);

    public Cuenta ObtenerCuenta(string numero) => cuentas.Find(c => c.Numero == numero);

    public void MostrarCuentas()
    {
        foreach (var cuenta in cuentas)
        {
            Console.WriteLine($"  - Cuenta {cuenta.Numero} (${cuenta.Saldo}) [{cuenta.GetType().Name}]");
        }
    }

    public bool ContieneCuenta(string numero) => cuentas.Exists(c => c.Numero == numero);
}

abstract class Cuenta
{
    public string Numero { get; }
    public decimal Saldo { get; protected set; }

    protected Cuenta(string numero, decimal saldoInicial)
    {
        Numero = numero;
        Saldo = saldoInicial;
    }

    public virtual void Depositar(decimal monto) => Saldo += monto;

    public virtual bool Retirar(decimal monto)
    {
        if (Saldo >= monto)
        {
            Saldo -= monto;
            return true;
        }
        return false;
    }
}

class CuentaOro : Cuenta { public CuentaOro(string n, decimal s) : base(n, s) { } }
class CuentaPlata : Cuenta { public CuentaPlata(string n, decimal s) : base(n, s) { } }
class CuentaBronce : Cuenta { public CuentaBronce(string n, decimal s) : base(n, s) { } }

abstract class Operacion
{
    public abstract void Ejecutar(List<Cliente> clientes);
}

class Deposito : Operacion
{
    string Numero;
    decimal Monto;

    public Deposito(string numero, decimal monto) => (Numero, Monto) = (numero, monto);

    public override void Ejecutar(List<Cliente> clientes)
    {
        foreach (var cliente in clientes)
        {
            var cuenta = cliente.ObtenerCuenta(Numero);
            if (cuenta != null)
            {
                cuenta.Depositar(Monto);
                break;
            }
        }
    }
}

class Retiro : Operacion
{
    string Numero;
    decimal Monto;

    public Retiro(string numero, decimal monto) => (Numero, Monto) = (numero, monto);

    public override void Ejecutar(List<Cliente> clientes)
    {
        foreach (var cliente in clientes)
        {
            var cuenta = cliente.ObtenerCuenta(Numero);
            if (cuenta != null && cuenta.Retirar(Monto)) break;
        }
    }
}

class Transferencia : Operacion
{
    string Origen, Destino;
    decimal Monto;

    public Transferencia(string origen, string destino, decimal monto) => (Origen, Destino, Monto) = (origen, destino, monto);

    public override void Ejecutar(List<Cliente> clientes)
    {
        Cuenta cuentaOrigen = null, cuentaDestino = null;

        foreach (var cliente in clientes)
        {
            if (cuentaOrigen == null) cuentaOrigen = cliente.ObtenerCuenta(Origen);
            if (cuentaDestino == null) cuentaDestino = cliente.ObtenerCuenta(Destino);
        }

        if (cuentaOrigen != null && cuentaDestino != null && cuentaOrigen.Retirar(Monto))
            cuentaDestino.Depositar(Monto);
    }
}

class Pago : Operacion
{
    string Numero;
    decimal Monto;

    public Pago(string numero, decimal monto) => (Numero, Monto) = (numero, monto);

    public override void Ejecutar(List<Cliente> clientes)
    {
        foreach (var cliente in clientes)
        {
            var cuenta = cliente.ObtenerCuenta(Numero);
            if (cuenta != null && cuenta.Retirar(Monto)) break;
        }
    }
}


