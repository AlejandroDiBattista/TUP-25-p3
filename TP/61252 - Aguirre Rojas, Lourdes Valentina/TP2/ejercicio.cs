using System;
using System.Collections.Generic;

public class Banco
{
    private List<Cliente> clientes;
    private List<Operacion> historialOperaciones;

    public Banco()
    {
        clientes = new List<Cliente>();
        historialOperaciones = new List<Operacion>();
    }

    public void RegistrarCliente(Cliente cliente)
    {
        clientes.Add(cliente);
    }

    public void RealizarOperacion(Operacion operacion)
    {
        operacion.Ejecutar();
        historialOperaciones.Add(operacion);
        operacion.ClienteDestino?.RegistrarOperacion(operacion); 
        operacion.ClienteOrigen?.RegistrarOperacion(operacion); 
    }

    public void ImprimirInforme()
    {
        Console.WriteLine("Informe global de operaciones:");
        foreach (var operacion in historialOperaciones)
        {
            operacion.MostrarDetalles();
        }

        Console.WriteLine("\nEstado final de las cuentas:");
        foreach (var cliente in clientes)
        {
            cliente.MostrarEstadoCuentas();
        }

        Console.WriteLine("\nHistorial de operaciones por cliente:");
        foreach (var cliente in clientes)
        {
            cliente.MostrarHistorial();
        }
    }
}

public class Cliente
{
    public string Nombre { get; private set; }
    public List<Cuenta> Cuentas { get; private set; }
    public List<Operacion> HistorialOperaciones { get; private set; }

    public Cliente(string nombre)
    {
        Nombre = nombre;
        Cuentas = new List<Cuenta>();
        HistorialOperaciones = new List<Operacion>();
    }

    public void AgregarCuenta(Cuenta cuenta)
    {
        Cuentas.Add(cuenta);
    }

    public void RegistrarOperacion(Operacion operacion)
    {
        HistorialOperaciones.Add(operacion);
    }

    public void MostrarEstadoCuentas()
    {
        Console.WriteLine($"Estado de las cuentas de {Nombre}:");
        foreach (var cuenta in Cuentas)
        {
            cuenta.MostrarEstado();
        }
    }

    public void MostrarHistorial()
    {
        Console.WriteLine($"Historial de operaciones de {Nombre}:");
        foreach (var operacion in HistorialOperaciones)
        {
            operacion.MostrarDetalles();
        }
    }
}

public abstract class Cuenta
{
    public string NumeroCuenta { get; private set; }
    public double Saldo { get; protected set; }
    public double Puntos { get; protected set; }
    public Cliente Titular { get; private set; }

    public Cuenta(string numeroCuenta, Cliente titular)
    {
        NumeroCuenta = numeroCuenta;
        Titular = titular;
        Saldo = 0;
        Puntos = 0;
    }

    public abstract void AcumularPuntos(double monto);

    public void MostrarEstado()
    {
        Console.WriteLine($"Cuenta {NumeroCuenta}: Saldo: {Saldo}, Puntos: {Puntos}");
    }

    public bool TieneFondos(double monto)
    {
        return Saldo >= monto;
    }

    public void Depositar(double monto)
    {
        Saldo += monto;
    }

    public bool Extraer(double monto)
    {
        if (TieneFondos(monto))
        {
            Saldo -= monto;
            return true;
        }
        return false;
    }

    public void Pagar(double monto)
    {
        if (Extraer(monto))
        {
            AcumularPuntos(monto);
        }
    }
}

public class CuentaOro : Cuenta
{
    public CuentaOro(string numeroCuenta, Cliente titular) : base(numeroCuenta, titular) { }

    public override void AcumularPuntos(double monto)
    {
        double porcentaje = monto > 1000 ? 0.05 : 0.03;
        Puntos += monto * porcentaje;
    }
}

public class CuentaPlata : Cuenta
{
    public CuentaPlata(string numeroCuenta, Cliente titular) : base(numeroCuenta, titular) { }

    public override void AcumularPuntos(double monto)
    {
        Puntos += monto * 0.02;
    }
}

public class CuentaBronce : Cuenta
{
    public CuentaBronce(string numeroCuenta, Cliente titular) : base(numeroCuenta, titular) { }

    public override void AcumularPuntos(double monto)
    {
        Puntos += monto * 0.01;
    }
}

public abstract class Operacion
{
    public double Monto { get; protected set; }
    public Cuenta? CuentaOrigen { get; protected set; }
    public Cuenta? CuentaDestino { get; protected set; }
    public Cliente? ClienteOrigen => CuentaOrigen?.Titular;
    public Cliente? ClienteDestino => CuentaDestino?.Titular;

    protected Operacion(double monto, Cuenta? cuentaOrigen, Cuenta? cuentaDestino = null)
    {
        Monto = monto;
        CuentaOrigen = cuentaOrigen;
        CuentaDestino = cuentaDestino;
    }

    public abstract void Ejecutar();
    public abstract void MostrarDetalles();
}

public class Deposito : Operacion
{
    public Deposito(double monto, Cuenta cuentaDestino) : base(monto, null, cuentaDestino) { }

    public override void Ejecutar()
    {
        CuentaDestino!.Depositar(Monto);
    }

    public override void MostrarDetalles()
    {
        Console.WriteLine($"Depósito: Monto: {Monto}, Cuenta destino: {CuentaDestino!.NumeroCuenta}");
    }
}

public class Retiro : Operacion
{
    public Retiro(double monto, Cuenta cuentaOrigen) : base(monto, cuentaOrigen) { }

    public override void Ejecutar()
    {
        if (!CuentaOrigen!.Extraer(Monto))
        {
            Console.WriteLine("Error: Fondos insuficientes.");
        }
    }

    public override void MostrarDetalles()
    {
        Console.WriteLine($"Retiro: Monto: {Monto}, Cuenta origen: {CuentaOrigen!.NumeroCuenta}");
    }
}

public class Pago : Operacion
{
    public Pago(double monto, Cuenta cuentaOrigen) : base(monto, cuentaOrigen) { }

    public override void Ejecutar()
    {
        CuentaOrigen!.Pagar(Monto);
    }

    public override void MostrarDetalles()
    {
        Console.WriteLine($"Pago: Monto: {Monto}, Cuenta origen: {CuentaOrigen!.NumeroCuenta}");
    }
}

public class Transferencia : Operacion
{
    public Transferencia(double monto, Cuenta cuentaOrigen, Cuenta cuentaDestino)
        : base(monto, cuentaOrigen, cuentaDestino) { }

    public override void Ejecutar()
    {
        if (CuentaOrigen!.Extraer(Monto))
        {
            CuentaDestino!.Depositar(Monto);
        }
        else
        {
            Console.WriteLine("Error: Fondos insuficientes para la transferencia.");
        }
    }

    public override void MostrarDetalles()
    {
        Console.WriteLine($"Transferencia: Monto: {Monto}, Cuenta origen: {CuentaOrigen!.NumeroCuenta}, Cuenta destino: {CuentaDestino!.NumeroCuenta}");
    }
}

public class Program
{
    public static void Main()
    {
        Banco banco = new Banco();

        Cliente cliente1 = new Cliente("Ana");
        Cliente cliente2 = new Cliente("Luis");

        Cuenta cuenta1 = new CuentaOro("001", cliente1);
        Cuenta cuenta2 = new CuentaPlata("002", cliente2);

        cliente1.AgregarCuenta(cuenta1);
        cliente2.AgregarCuenta(cuenta2);

        banco.RegistrarCliente(cliente1);
        banco.RegistrarCliente(cliente2);

        banco.RealizarOperacion(new Deposito(2000, cuenta1));
        banco.RealizarOperacion(new Pago(500, cuenta1));
        banco.RealizarOperacion(new Transferencia(300, cuenta1, cuenta2));
        banco.RealizarOperacion(new Retiro(100, cuenta2));

        banco.ImprimirInforme();
    }
}
