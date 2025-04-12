// TP2: Sistema de Cuentas Bancarias

// Implementar un sistema de cuentas bancarias que permita realizar operaciones como depósitos, retiros, transferencias y pagos.

using System;
using System.Collections.Generic;
using System.Linq;
using static System.Console;

public class Banco
{
    public string Nombre { get; }

    private List<Cliente> clientes = new();
    private static Dictionary<string, Cuenta> cuentas = new();

    public Banco(string nombre) => Nombre = nombre;

    public void AgregarCliente(Cliente cliente)
    {
        clientes.Add(cliente);
        foreach (var cuenta in cliente.Cuentas)
            RegistrarCuenta(cuenta);
    }

    public void RegistrarOperacion(Operacion operacion)
    {
        if (operacion.Ejecutar())
            WriteLine($"La operacion fue exitosa: {operacion.Descripcion}");
        else
            WriteLine($"Error en la operacion: {operacion.Descripcion}");
    }

    public void Informe()
    {
        WriteLine($"\n=== Informe del banco {Nombre} ===");
        foreach (var cliente in clientes)
            cliente.Mostrar();
    }

    private static void RegistrarCuenta(Cuenta cuenta)
    {
        if (!cuentas.ContainsKey(cuenta.Numero))
            cuentas[cuenta.Numero] = cuenta;
    }

    public static Cuenta BuscarCuenta(string numero) =>
        cuentas.TryGetValue(numero, out var cuenta) ? cuenta : null;
}

public class Cliente
{
    public string Nombre { get; }
    public List<Cuenta> Cuentas { get; } = new();

    public Cliente(string nombre) => Nombre = nombre;

    public void AgregarCuenta(Cuenta cuenta) => Cuentas.Add(cuenta);

    public void Mostrar()
    {
        WriteLine($"\n👤 Cliente: {Nombre} - Total: {Cuentas.Sum(c => c.Saldo):C0}");
        foreach (var cuenta in Cuentas)
            cuenta.Mostrar();
    }
}

public class Cuenta
{
    public string Numero { get; }
    public decimal Saldo { get; private set; }

    private List<Operacion> historial = new();

    public Cuenta(string numero, decimal saldoInicial)
    {
        Numero = numero;
        Saldo = saldoInicial;
    }

    public bool Depositar(decimal monto)
    {
        if (monto <= 0) return false;
        Saldo += monto;
        return true;
    }

    public bool Extraer(decimal monto)
    {
        if (monto <= 0 || monto > Saldo) return false;
        Saldo -= monto;
        return true;
    }

    public void Registrar(Operacion operacion) => historial.Add(operacion);

    public void Mostrar()
    {
        WriteLine($"   💳 Cuenta {Numero} - Saldo: {Saldo:C}");
        foreach (var op in historial)
            WriteLine($"      ↪ {op.Descripcion}");
    }
}

public abstract class Operacion
{
    public Cuenta Origen { get; protected set; }
    public decimal Monto { get; protected set; }

    public Operacion(string numeroCuenta, decimal monto)
    {
        Origen = Banco.BuscarCuenta(numeroCuenta);
        Monto = monto;
    }

    public abstract bool Ejecutar();
    public virtual string Descripcion => "";
}

public class Deposito : Operacion
{
    public Deposito(string numeroCuenta, decimal monto) : base(numeroCuenta, monto) { }

    public override bool Ejecutar()
    {
        if (Origen != null && Origen.Depositar(Monto))
        {
            Origen.Registrar(this);
            return true;
        }
        return false;
    }

    public override string Descripcion => $"Depósito de {Monto:C0} en cuenta {Origen?.Numero}";
}

public class Extraccion : Operacion
{
    public Extraccion(string numeroCuenta, decimal monto) : base(numeroCuenta, monto) { }

    public override bool Ejecutar()
    {
        if (Origen != null && Origen.Extraer(Monto))
        {
            Origen.Registrar(this);
            return true;
        }
        return false;
    }

    public override string Descripcion => $"Extracción de {Monto:C0} de cuenta {Origen?.Numero}";
}

public class Transferencia : Operacion
{
    public Cuenta Destino { get; private set; }

    public Transferencia(string origen, string destino, decimal monto) : base(origen, monto)
    {
        Destino = Banco.BuscarCuenta(destino);
    }

    public override bool Ejecutar()
    {
        if (Origen == null || Destino == null) return false;

        if (Origen.Extraer(Monto))
        {
            if (Destino.Depositar(Monto))
            {
                Origen.Registrar(this);
                Destino.Registrar(this);
                return true;
            }
            else
            {
                Origen.Depositar(Monto);
            }
        }
        return false;
    }

    public override string Descripcion => $"Transferencia de {Monto:C0} de {Origen?.Numero} a {Destino?.Numero}";
}

public class Program
{
    public static void Main()
    {
        var banco1 = new Banco("Banco Codificado");

        var ana = new Cliente("Ana López");
        ana.AgregarCuenta(new Cuenta("A100", 1500));
        ana.AgregarCuenta(new Cuenta("A101", 800));

        var tomas = new Cliente("Tomás Núñez");
        tomas.AgregarCuenta(new Cuenta("B200", 400));

        banco1.AgregarCliente(ana);
        banco1.AgregarCliente(tomas);

        banco1.RegistrarOperacion(new Deposito("A100", 300));
        banco1.RegistrarOperacion(new Extraccion("A101", 100));
        banco1.RegistrarOperacion(new Transferencia("A100", "B200", 200));
        banco1.RegistrarOperacion(new Transferencia("B200", "X999", 50));

        banco1.Informe();

        // -- Segundo caso --
        var banco2 = new Banco("Banco Central");

        var pedro = new Cliente("Pedro Páramo");
        pedro.AgregarCuenta(new Cuenta("P001", 5000));

        var laura = new Cliente("Laura Varela");
        laura.AgregarCuenta(new Cuenta("L010", 1500));
        laura.AgregarCuenta(new Cuenta("L011", 2000));

        var carlos = new Cliente("Carlos García");
        carlos.AgregarCuenta(new Cuenta("C020", 300));

        banco2.AgregarCliente(pedro);
        banco2.AgregarCliente(laura);
        banco2.AgregarCliente(carlos);

        banco2.RegistrarOperacion(new Deposito("P001", 500));
        banco2.RegistrarOperacion(new Extraccion("L010", 100));
        banco2.RegistrarOperacion(new Transferencia("P001", "L011", 1000));
        banco2.RegistrarOperacion(new Transferencia("L011", "C020", 500));
        banco2.RegistrarOperacion(new Transferencia("C020", "P001", 100));

        banco2.Informe();
    }
}

