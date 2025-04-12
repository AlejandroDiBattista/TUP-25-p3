// TP2: Sistema de Cuentas Bancarias
//

// Implementar un sistema de cuentas bancarias que permita realizar operaciones como depósitos, retiros, transferencias y pagos.

class Banco { }
class Cliente { }

abstract class Cuenta { }
class CuentaOro : Cuenta { }
class CuentaPlata : Cuenta { }
class CuentaBronce : Cuenta { }

abstract class Operacion { }
class Deposito : Operacion { }
class Retiro : Operacion { }
class Transferencia : Operacion { }
class Pago : Operacion { }


/// EJEMPLO DE USO ///

// Definiciones 

using System;
using System.Collections.Generic;

abstract class Operacion
{
    public DateTime Fecha { get; set; }
    public decimal Monto { get; set; }
    public string CuentaOrigen { get; set; }
    public string? CuentaDestino { get; set; }

    public Operacion(decimal monto, string cuentaOrigen, string cuentaDestino = "")
    {
        Fecha = DateTime.Now;
        Monto = monto;
        CuentaOrigen = cuentaOrigen;
        CuentaDestino = cuentaDestino;
    }

    public abstract string Describir(Banco banco);
    public abstract bool Ejecutar(Cuenta origen, Cuenta? destino = null);
}

abstract class Cuenta
{
    public string Numero { get; private set; }
    public decimal Saldo { get; protected set; }
    public decimal Puntos { get; protected set; }
    public Cliente? Titular { get; set; }
    public List<Operacion> Operaciones { get; private set; }

    public Cuenta(string numero, decimal saldoInicial)
    {
        Numero = numero;
        Saldo = saldoInicial;
        Puntos = 0;
        Operaciones = new List<Operacion>();
    }

    public void Depositar(decimal monto)
    {
        Saldo += monto;
        AcumularPuntos(monto);
    }

    public bool Extraer(decimal monto)
    {
        if (Saldo >= monto)
        {
            Saldo -= monto;
            AcumularPuntos(monto);
            return true;
        }
        return false;
    }

    public bool Pagar(decimal monto)
    {
        if (Saldo >= monto)
        {
            Saldo -= monto;
            AcumularPuntos(monto);
            return true;
        }
        return false;
    }

    public abstract void AcumularPuntos(decimal monto);

    public void AgregarOperacion(Operacion op)
    {
        Operaciones.Add(op);
    }
}

class Banco
{
    public List<Cuenta> Cuentas { get; private set; }

    public Banco()
    {
        Cuentas = new List<Cuenta>();
    }

    public void AgregarCuenta(Cuenta cuenta)
    {
        Cuentas.Add(cuenta);
    }

    public Cuenta? BuscarCuenta(string numero)
    {
        return Cuentas.Find(c => c.Numero == numero);
    }

    public void RegistrarOperacion(Operacion op)
    {
        var origen = BuscarCuenta(op.CuentaOrigen);
        var destino = string.IsNullOrEmpty(op.CuentaDestino) ? null : BuscarCuenta(op.CuentaDestino);

        if (origen == null)
        {
            Console.WriteLine($" Cuenta origen {op.CuentaOrigen} no encontrada.");
            return;
        }

        if (op is Transferencia && destino == null)
        {
            Console.WriteLine($" Cuenta destino {op.CuentaDestino} no encontrada.");
            return;
        }

        if (op.Ejecutar(origen, destino))
        {
            origen.AgregarOperacion(op);
            if (destino != null && destino != origen)
                destino.AgregarOperacion(op);

            Console.WriteLine($"se realizo {op.Describir(this)}");
        }
        else
        {
            Console.WriteLine($" Falló: {op.Describir(this)}");
        }
    }
}

class CuentaOro : Cuenta
{
    public CuentaOro(string numero, decimal saldoInicial) : base(numero, saldoInicial) { }

    public override void AcumularPuntos(decimal monto)
    {
        Puntos += monto > 1000 ? monto * 0.05m : monto * 0.03m;
    }
}

class CuentaPlata : Cuenta
{
    public CuentaPlata(string numero, decimal saldoInicial) : base(numero, saldoInicial) { }

    public override void AcumularPuntos(decimal monto)
    {
        Puntos += monto * 0.02m;
    }
}

class CuentaBronce : Cuenta
{
    public CuentaBronce(string numero, decimal saldoInicial) : base(numero, saldoInicial) { }

    public override void AcumularPuntos(decimal monto)
    {
        Puntos += monto * 0.01m;
    }
}

class Cliente
{
    public string Nombre { get; private set; }
    public List<Cuenta> Cuentas { get; private set; }

    public Cliente(string nombre)
    {
        Nombre = nombre;
        Cuentas = new List<Cuenta>();
    }

    public void Agregar(Cuenta cuenta)
    {
        cuenta.Titular = this;
        Cuentas.Add(cuenta);
    }
}


class Deposito : Operacion
{
    public Deposito(string cuenta, decimal monto) : base(monto, cuenta) { }

    public override bool Ejecutar(Cuenta origen, Cuenta? destino = null)
    {
        origen.Depositar(Monto);
        return true;
    }

    public override string Describir(Banco banco)
    {
        return $"Depósito de ${Monto:N2} en cuenta {CuentaOrigen}";
    }
}

class Retiro : Operacion
{
    public Retiro(string cuenta, decimal monto) : base(monto, cuenta) { }

    public override bool Ejecutar(Cuenta origen, Cuenta? destino = null)
    {
        return origen.Extraer(Monto);
    }

    public override string Describir(Banco banco)
    {
        return $"Retiro de ${Monto:N2} de cuenta {CuentaOrigen}";
    }
}

class Pago : Operacion
{
    public Pago(string cuenta, decimal monto) : base(monto, cuenta) { }

    public override bool Ejecutar(Cuenta origen, Cuenta? destino = null)
    {
        return origen.Pagar(Monto);
    }

    public override string Describir(Banco banco)
    {
        return $"Pago de ${Monto:N2} desde cuenta {CuentaOrigen}";
    }
}

class Transferencia : Operacion
{
    public Transferencia(string origen, string destino, decimal monto) : base(monto, origen, destino) { }

    public override bool Ejecutar(Cuenta origen, Cuenta? destino)
    {
        if (destino == null) return false;

        if (origen.Extraer(Monto))
        {
            destino.Depositar(Monto);
            return true;
        }
        return false;
    }

    public override string Describir(Banco banco)
    {
        return $"Transferencia de ${Monto:N2} de {CuentaOrigen} a {CuentaDestino}";
    }
}



class Program
{
    static void Main()
    {
        Banco banco = new Banco();

        Cliente cliente1 = new Cliente("Juan Pérez");
        Cliente cliente2 = new Cliente("Ana Gómez");

        Cuenta cuentaOro = new CuentaOro("123", 5000);
        Cuenta cuentaPlata = new CuentaPlata("456", 2000);
        Cuenta cuentaBronce = new CuentaBronce("789", 1000);

        cliente1.Agregar(cuentaOro);
        cliente2.Agregar(cuentaPlata);
        cliente2.Agregar(cuentaBronce);

        banco.AgregarCuenta(cuentaOro);
        banco.AgregarCuenta(cuentaPlata);
        banco.AgregarCuenta(cuentaBronce);

        banco.RegistrarOperacion(new Deposito("123", 1000));
        banco.RegistrarOperacion(new Retiro("456", 500));
        banco.RegistrarOperacion(new Pago("789", 200));
        banco.RegistrarOperacion(new Transferencia("123", "456", 700));

        Console.WriteLine("\n=== RESUMEN DE CUENTAS ===");
        foreach (var cuenta in banco.Cuentas)
        {
            Console.WriteLine($"Cuenta {cuenta.Numero}: Saldo = ${cuenta.Saldo}, Puntos = {cuenta.Puntos}, Titular = {cuenta.Titular?.Nombre}");
            foreach (var op in cuenta.Operaciones)
            {
                Console.WriteLine($"  - {op.Fecha:T} | {op.Describir(banco)}");
            }
        }
    }
}