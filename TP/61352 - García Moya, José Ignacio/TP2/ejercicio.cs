using System;
using System.Collections.Generic;
using System.Linq;

// Clase Banco
public class Banco
{
    public string Nombre { get; private set; }
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
        if (clientes.Any(c => c.PoseeCuenta(operacion.CuentaOrigen)))
        {
            operacion.Ejecutar();
            operaciones.Add(operacion);
        }
        else
        {
            Console.WriteLine("Error: La cuenta origen no pertenece a este banco.");
        }
    }

    public void Informe()
    {
        Console.WriteLine($"Banco: {Nombre} | Clientes: {clientes.Count}");
        foreach (var cliente in clientes)
        {
            cliente.MostrarInforme();
        }
    }
}

// Clase Cliente
public class Cliente
{
    public string Nombre { get; private set; }
    private List<Cuenta> cuentas = new List<Cuenta>();
    private List<Operacion> historialOperaciones = new List<Operacion>();

    public Cliente(string nombre)
    {
        Nombre = nombre;
    }

    public void Agregar(Cuenta cuenta)
    {
        cuentas.Add(cuenta);
    }

    public bool PoseeCuenta(string numero)
    {
        return cuentas.Any(c => c.Numero == numero);
    }

    public void RegistrarOperacion(Operacion operacion)
    {
        historialOperaciones.Add(operacion);
    }

    public void MostrarInforme()
    {
        Console.WriteLine($"  Cliente: {Nombre} | Saldo Total: ${cuentas.Sum(c => c.Saldo)},00 | Puntos Total: ${cuentas.Sum(c => c.Puntos)},00");
        foreach (var cuenta in cuentas)
        {
            cuenta.MostrarInforme();
        }
    }
}

// Clase abstracta Cuenta
public abstract class Cuenta
{
    public string Numero { get; private set; }
    public decimal Saldo { get; protected set; }
    public decimal Puntos { get; protected set; }

    public Cuenta(string numero, decimal saldoInicial)
    {
        Numero = numero;
        Saldo = saldoInicial;
    }

    public abstract void AcumularPuntos(decimal monto);
    
    public void MostrarInforme()
    {
        Console.WriteLine($"    Cuenta: {Numero} | Saldo: ${Saldo},00 | Puntos: ${Puntos},00");
    }
}

public class CuentaOro : Cuenta
{
    public CuentaOro(string numero, decimal saldo) : base(numero, saldo) { }
    public override void AcumularPuntos(decimal monto)
    {
        Puntos += monto > 1000 ? monto * 0.05m : monto * 0.03m;
    }
}

public class CuentaPlata : Cuenta
{
    public CuentaPlata(string numero, decimal saldo) : base(numero, saldo) { }
    public override void AcumularPuntos(decimal monto)
    {
        Puntos += monto * 0.02m;
    }
}

public class CuentaBronce : Cuenta
{
    public CuentaBronce(string numero, decimal saldo) : base(numero, saldo) { }
    public override void AcumularPuntos(decimal monto)
    {
        Puntos += monto * 0.01m;
    }
}

// Clase abstracta Operacion
public abstract class Operacion
{
    public string CuentaOrigen { get; protected set; }
    public decimal Monto { get; protected set; }

    protected Operacion(string cuentaOrigen, decimal monto)
    {
        CuentaOrigen = cuentaOrigen;
        Monto = monto;
    }

    public abstract void Ejecutar();
}

public class Deposito : Operacion
{
    public Deposito(string cuenta, decimal monto) : base(cuenta, monto) { }
    public override void Ejecutar()
    {
        // Implementar lógica de depósito
    }
}

public class Retiro : Operacion
{
    public Retiro(string cuenta, decimal monto) : base(cuenta, monto) { }
    public override void Ejecutar()
    {
        // Implementar lógica de retiro
    }
}

public class Pago : Operacion
{
    public Pago(string cuenta, decimal monto) : base(cuenta, monto) { }
    public override void Ejecutar()
    {
        // Implementar lógica de pago
    }
}

public class Transferencia : Operacion
{
    public string CuentaDestino { get; private set; }

    public Transferencia(string cuentaOrigen, string cuentaDestino, decimal monto) : base(cuentaOrigen, monto)
    {
        CuentaDestino = cuentaDestino;
    }

    public override void Ejecutar()
    {
        // Implementar lógica de transferencia
    }
}
