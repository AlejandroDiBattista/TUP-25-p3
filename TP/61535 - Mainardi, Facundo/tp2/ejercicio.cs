using System;
using System.Collections.Generic;
using System.Linq;

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

public abstract class Operacion
{
    public double Monto { get; }
    public string CuentaInicio { get; }
    public string ClienteInicio { get; }

    protected Operacion(double monto, string cuentaInicio, string clienteInicio)
    {
        Monto = monto;
        CuentaInicio = cuentaInicio;
        ClienteInicio = clienteInicio;
    }

    public abstract void Ejecutar(Banco banco);
}

public class Deposito : Operacion
{
    public Deposito(string cuentaInicio, double monto) : base(monto, cuentaInicio, "") { }

    public override void Ejecutar(Banco banco)
    {
        var cuenta = banco.BuscarCuenta(CuentaInicio);
        if (cuenta != null)
        {
            cuenta.Saldo += Monto;
            cuenta.Historial.Add($"Depósito $ {Monto} a [{CuentaInicio}/{cuenta.Cliente.Nombre}]");
        }
    }
}

public class Retiro : Operacion
{
    public Retiro(string cuentaInicio, double monto) : base(monto, cuentaInicio, "") { }

    public override void Ejecutar(Banco banco)
    {
        var cuenta = banco.BuscarCuenta(CuentaInicio);
        if (cuenta != null && cuenta.Saldo >= Monto)
        {
            cuenta.Saldo -= Monto;
            cuenta.Historial.Add($"Retiro $ {Monto} de [{CuentaInicio}/{cuenta.Cliente.Nombre}]");
        }
        else
        {
            Console.WriteLine($"Fondos insuficientes en la cuenta {CuentaInicio}");
        }
    }
}

public class Pago : Operacion
{
    public Pago(string cuentaInicio, double monto) : base(monto, cuentaInicio, "") { }

    public override void Ejecutar(Banco banco)
    {
        var cuenta = banco.BuscarCuenta(CuentaInicio);
        if (cuenta != null && cuenta.Saldo >= Monto)
        {
            cuenta.Saldo -= Monto;
            cuenta.AcumularPuntos(Monto);
            cuenta.Historial.Add($"Pago $ {Monto} con [{CuentaInicio}/{cuenta.Cliente.Nombre}]");
        }
        else
        {
            Console.WriteLine($"Fondos insuficientes en la cuenta {CuentaInicio}");
        }
    }
}

public class Transferencia : Operacion
{
    public string CuentaDestino { get; }

    public Transferencia(string cuentaInicio, string cuentaDestino, double monto) : base(monto, cuentaInicio, "")
    {
        CuentaDestino = cuentaDestino;
    }

    public override void Ejecutar(Banco banco)
    {
        var origen = banco.BuscarCuenta(CuentaInicio);
        var destino = banco.BuscarCuenta(CuentaDestino);

        if (origen != null && destino != null && origen.Saldo >= Monto)
        {
            origen.Saldo -= Monto;
            destino.Saldo += Monto;
            origen.Historial.Add($"Transferencia $ {Monto} de [{CuentaInicio}/{origen.Cliente.Nombre}] a [{CuentaDestino}/{destino.Cliente.Nombre}]");
            destino.Historial.Add($"Transferencia $ {Monto} de [{CuentaInicio}/{origen.Cliente.Nombre}] a [{CuentaDestino}/{destino.Cliente.Nombre}]");
        }
        else
        {
            Console.WriteLine($"Fondos insuficientes o cuenta de destino no encontrada en la transferencia de [{CuentaInicio}] a [{CuentaDestino}]");
        }
    }
}

public abstract class Cuenta
{
    public string Numero { get; }
    public double Saldo { get; set; }
    public Cliente Cliente { get; }
    public List<string> Historial { get; }
    private double _puntos;

    public double Puntos
    {
        get { return _puntos; }
        set { _puntos = value; }
    }

    protected Cuenta(string numero, double saldo, Cliente cliente)
    {
        Numero = numero;
        Saldo = saldo;
        Cliente = cliente;
        Historial = new List<string>();
    }

    public abstract void AcumularPuntos(double monto);
}

public class CuentaOro : Cuenta
{
    public CuentaOro(string numero, double saldo, Cliente cliente) : base(numero, saldo, cliente) { }

    public override void AcumularPuntos(double monto)
    {
        double porcentaje = monto > 1000 ? 0.05 : 0.03;
        Puntos += monto * porcentaje;
    }
}

public class CuentaPlata : Cuenta
{
    public CuentaPlata(string numero, double saldo, Cliente cliente) : base(numero, saldo, cliente) { }

    public override void AcumularPuntos(double monto)
    {
        Puntos += monto * 0.02;
    }
}

public class CuentaBronce : Cuenta
{
    public CuentaBronce(string numero, double saldo, Cliente cliente) : base(numero, saldo, cliente) { }

    public override void AcumularPuntos(double monto)
    {
        Puntos += monto * 0.01;
    }
}

public class Cliente
{
    public string Nombre { get; }
    public List<Cuenta> Cuentas { get; }
    public Banco Banco { get; set; }

    public Cliente(string nombre)
    {
        Nombre = nombre;
        Cuentas = new List<Cuenta>();
    }

    public void Agregar(Cuenta cuenta)
    {
        Cuentas.Add(cuenta);
    }
}

public class Banco
{
    public string Nombre { get; }
    private List<Cliente> Clientes { get; }
    private List<Operacion> Operaciones { get; }

    public Banco(string nombre)
    {
        Nombre = nombre;
        Clientes = new List<Cliente>();
        Operaciones = new List<Operacion>();
    }

    public void Agregar(Cliente cliente)
    {
        Clientes.Add(cliente);
        cliente.Banco = this;
    }

    public Cuenta BuscarCuenta(string numeroCuenta)
    {
        foreach (var cliente in Clientes)
        {
            var cuenta = cliente.Cuentas.FirstOrDefault(c => c.Numero == numeroCuenta);
            if (cuenta != null)
                return cuenta;
        }
        return null;
    }

    public void Registrar(Operacion operacion)
    {
        Operaciones.Add(operacion);
        operacion.Ejecutar(this);
    }

    public void Informe()
    {
        Console.WriteLine($"Banco: {Nombre} | Clientes: {Clientes.Count}");
        foreach (var cliente in Clientes)
        {
            double saldoTotal = cliente.Cuentas.Sum(c => c.Saldo);
            double puntosTotales = cliente.Cuentas.Sum(c => c.Puntos);

            Console.WriteLine($"\n  Cliente: {cliente.Nombre} | Saldo Total: $ {saldoTotal:F2} | Puntos Total: $ {puntosTotales:F2}");
            foreach (var cuenta in cliente.Cuentas)
            {
                Console.WriteLine($"    Cuenta: {cuenta.Numero} | Saldo: $ {cuenta.Saldo:F2} | Puntos: $ {cuenta.Puntos:F2}");
                foreach (var operacion in cuenta.Historial)
                {
                    Console.WriteLine($"     -  {operacion}");
                }
            }
        }
    }
}
nac.Informe();
tup.Informe();