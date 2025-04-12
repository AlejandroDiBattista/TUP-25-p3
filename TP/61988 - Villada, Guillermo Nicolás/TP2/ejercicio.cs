using System;
using System.Collections.Generic;
using System.Linq;




var raul = new Cliente("Raul Perez");
raul.Agregar(new CuentaOro("10001", 1000));
raul.Agregar(new CuentaPlata("10002", 2000));

var sara = new Cliente("Sara Lopez");
sara.Agregar(new CuentaPlata("10003", 3000));
sara.Agregar(new CuentaPlata("10004", 4000));

var luis = new Cliente("Luis Gomez");
luis.Agregar(new CuentaBronce("10005", 5000));

var nico = new Cliente("Nicolas Villada");
nico.Agregar(new CuentaOro("10006", 10000000));


// Crear banco y agregar clientes
var banco = new Banco("Banco Villada");
banco.Agregar(raul);
banco.Agregar(sara);
banco.Agregar(luis);
banco.Agregar(nico);



banco.Registrar(new Deposito("10001", 100));
banco.Registrar(new Retiro("10002", 200));
banco.Registrar(new Transferencia("10001", "10002", 300));
banco.Registrar(new Transferencia("10003", "10004", 500));
banco.Registrar(new Pago("10002", 400));
banco.Registrar(new Deposito("10005", 100));
banco.Registrar(new Retiro("10005", 200));
banco.Registrar(new Transferencia("10005", "10002", 300));
banco.Registrar(new Pago("10005", 400));
banco.Registrar(new Deposito("10006", 500000));
banco.Registrar(new Retiro("10006", 200000));
banco.Registrar(new Transferencia("10006", "10002", 30000));
// Mostrar informe final

banco.Informe();

class Banco
{
    public string Nombre { get; }
    private List<Cliente> clientes = new List<Cliente>();
    private List<Operacion> operaciones = new List<Operacion>();

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
        operaciones.Add(operacion);
        operacion.Ejecutar(clientes);
    }

    public void Informe()
    {
        Console.WriteLine($"Informe del Banco: {Nombre}");
        foreach (var cliente in clientes)
        {
            cliente.Informe();
        }
        Console.WriteLine();
    }
}

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

    public Cuenta ObtenerCuenta(string numero)
    {
        return cuentas.Find(c => c.Numero == numero);
    }

    public void Informe()
    {
        Console.WriteLine($"Cliente: {Nombre}");
        foreach (var cuenta in cuentas)
        {
            cuenta.Informe();
        }
    }
}

abstract class Cuenta
{
    public string Numero { get; }
    public decimal Saldo { get; protected set; }

    public Cuenta(string numero, decimal saldoInicial)
    {
        Numero = numero;
        Saldo = saldoInicial;
    }

    public virtual void Depositar(decimal monto)
    {
        Saldo += monto;
    }

    public virtual bool Retirar(decimal monto)
    {
        if (Saldo >= monto)
        {
            Saldo -= monto;
            return true;
        }
        return false;
    }

    public void Informe()
    {
        Console.WriteLine($"Cuenta {Numero}: Saldo = {Saldo:C}");
    }
}

class CuentaOro : Cuenta
{
    public CuentaOro(string numero, decimal saldoInicial) : base(numero, saldoInicial) { }
}

class CuentaPlata : Cuenta
{
    public CuentaPlata(string numero, decimal saldoInicial) : base(numero, saldoInicial) { }
}

class CuentaBronce : Cuenta
{
    public CuentaBronce(string numero, decimal saldoInicial) : base(numero, saldoInicial) { }
}

abstract class Operacion
{
    public abstract void Ejecutar(List<Cliente> clientes);
}

class Deposito : Operacion
{
    private string numeroCuenta;
    private decimal monto;

    public Deposito(string numeroCuenta, decimal monto)
    {
        this.numeroCuenta = numeroCuenta;
        this.monto = monto;
    }

    public override void Ejecutar(List<Cliente> clientes)
    {
        foreach (var cliente in clientes)
        {
            var cuenta = cliente.ObtenerCuenta(numeroCuenta);
            if (cuenta != null)
            {
                cuenta.Depositar(monto);
                Console.WriteLine($" -Depósito de {monto:C} realizado en la cuenta {numeroCuenta}");
                Console.WriteLine();
            
                return;
            }
        }
        Console.WriteLine($"Error: No se encontró la cuenta {numeroCuenta}");
    }
}

class Pago : Operacion
{
    private string numeroCuenta;
    private decimal monto;

    public Pago(string numeroCuenta, decimal monto)
    {
        this.numeroCuenta = numeroCuenta;
        this.monto = monto;
    }

    public override void Ejecutar(List<Cliente> clientes)
    {
        foreach (var cliente in clientes)
        {
            var cuenta = cliente.ObtenerCuenta(numeroCuenta);
            if (cuenta != null && cuenta.Retirar(monto))
            {
                Console.WriteLine($" -Pago de {monto:C} realizado desde la cuenta {numeroCuenta}");
                Console.WriteLine();
                return;
            }
        }
        Console.WriteLine($"Error: Fondos insuficientes o cuenta no encontrada: {numeroCuenta}");
    }
}

class Retiro : Operacion
{
    private string numeroCuenta;
    private decimal monto;

    public Retiro(string numeroCuenta, decimal monto)
    {
        this.numeroCuenta = numeroCuenta;
        this.monto = monto;
    }

    public override void Ejecutar(List<Cliente> clientes)
    {
        foreach (var cliente in clientes)
        {
            var cuenta = cliente.ObtenerCuenta(numeroCuenta);
            if (cuenta != null && cuenta.Retirar(monto))
            {
                Console.WriteLine($" -Retiro de {monto:C} realizado desde la cuenta {numeroCuenta}");
                Console.WriteLine();
                return;
            }
        }
        Console.WriteLine($"Error: Fondos insuficientes o cuenta no encontrada: {numeroCuenta}");
    }
}

class Transferencia : Operacion
{
    private string origen;
    private string destino;
    private decimal monto;

    public Transferencia(string origen, string destino, decimal monto)
    {
        this.origen = origen;
        this.destino = destino;
        this.monto = monto;
    }

    public override void Ejecutar(List<Cliente> clientes)
    {
        Cuenta cuentaOrigen = null, cuentaDestino = null;
        foreach (var cliente in clientes)
        {
            if (cuentaOrigen == null) cuentaOrigen = cliente.ObtenerCuenta(origen);
            if (cuentaDestino == null) cuentaDestino = cliente.ObtenerCuenta(destino);
        }
        if (cuentaOrigen != null && cuentaDestino != null && cuentaOrigen.Retirar(monto))
        {
            cuentaDestino.Depositar(monto);
            Console.WriteLine($" -Transferencia de {monto:C} desde {origen} a {destino} realizada exitosamente");
            Console.WriteLine();
        }
        else
        {
            Console.WriteLine($"Error: No se pudo realizar la transferencia de {monto:C} desde {origen} a {destino}");
        }
    }
}

