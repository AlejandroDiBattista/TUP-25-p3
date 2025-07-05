using System;
using System.Collections.Generic;
// TP2: Sistema de Cuentas Bancarias
//

// Implementar un sistema de cuentas bancarias que permita realizar operaciones como depósitos, retiros, transferencias y pagos.

class Banco
{
    public List<Cliente> Clientes { get; set; } = new List<Cliente>();
    string Nombre { get; set; }
    public List<Operacion> Historial { get; set; }
    public Banco(string nombre)
    {
        this.Nombre = nombre;
        this.Historial = new List<Operacion>();
    }

    public void Agregar(Cliente data)
    {
        Clientes.Add(data);
    }
    public void Registrar(Operacion operacion)
    {


        Historial.Add(operacion);
        operacion.EjecutarAccion(Clientes);
    }

    public void Informe()
    {

        Console.WriteLine($"Banco {this.Nombre} | N° clientes: {Clientes.Count}");

        foreach (var cliente in Clientes)
        {
            Console.WriteLine($"Cliente: {cliente.Nombre} ${GeneralDatosClientes(cliente.Cuentas)}\n");
            foreach (var cuenta in cliente.Cuentas)
            {
                Console.WriteLine($"Cuenta {cuenta.NumeroCuenta} | Saldo: ${cuenta.Saldo} | Puntos Total: ${cuenta.Creditos}\n");
                RecorrerHistoria(cuenta.Historia);
            }
            Console.WriteLine();
        }
    }


    public string GeneralDatosClientes(List<Cuenta> data)
    {
        var saldoTotal = 0m;
        var Puntos = 0m;
        foreach (var item in data)
        {
            saldoTotal += item.Saldo;
            Puntos += item.Creditos;
        }

        return $"Saldo: ${saldoTotal} | Puntos: ${Puntos}";
    }

    public static Cuenta BuscarCuenta(string numeroCuenta, List<Cliente> clientes)
    {
        foreach (var cliente in clientes)
        {
            foreach (var cuenta in cliente.Cuentas)
            {
                if (cuenta.NumeroCuenta == numeroCuenta)
                {
                    return cuenta;
                }
            }
        }
        return null;
    }

    public void RecorrerHistoria(List<Operacion> operaciones)
    {
        foreach (var operacion in operaciones)
        {
            Console.WriteLine(operacion.Informe());
        }
    }

}
class Cliente
{
    public string Nombre { get; set; }
    public List<Cuenta> Cuentas { get; set; }

    public Cliente(string nombre)
    {
        this.Nombre = nombre;
        Cuentas = new List<Cuenta>();
    }

    public void Agregar(Cuenta data)
    {
        this.Cuentas.Add(data);
    }

}

abstract class Cuenta
{
    public string NumeroCuenta { get; set; }
    public decimal Saldo { get; set; }
    public decimal Creditos { get; set; }
    // public Cliente Cliente { get; set; }
    public List<Operacion> Historia { get; set; } = new List<Operacion>();
    public Banco banco { get; set; }

    public void GuardarHistorial(Operacion operacion)
    {
        Historia.Add(operacion);
    }

    public Cuenta(string numeroCuenta, decimal saldo)
    {
        this.NumeroCuenta = numeroCuenta;
        this.Saldo = saldo;
    }

    public void Depositar(decimal cantidad)
    {
        this.Saldo += cantidad;
    }
    public bool Retirar(decimal cantidad)
    {
        if (cantidad > Saldo)
            return false;
        this.Saldo -= cantidad;
        return true;
    }

    public bool Transferir(decimal cantidad, Cuenta cuentaDestino)
    {
        if (cantidad > Saldo)
        {
            return false;
        }
        Saldo -= cantidad;
        cuentaDestino.Depositar(cantidad);
        return true;
    }

    public bool Pagar(decimal cantidad)
    {
        if (!Retirar(cantidad)) return false;

        AcumularCreditos(cantidad);
        return false;
    }
    public abstract void AcumularCreditos(decimal cantidad);

}
class CuentaOro : Cuenta
{
    public CuentaOro(string numeroCuenta, decimal saldo) : base(numeroCuenta, saldo)
    {
    }

    public override void AcumularCreditos(decimal cantidad)
    {
        if (cantidad > 1000)
            Creditos += cantidad * 0.05m;

        else
            Creditos += cantidad * 0.03m;
    }
}
class CuentaPlata : Cuenta
{
    public CuentaPlata(string numeroCuenta, decimal saldo) : base(numeroCuenta, saldo)
    {
    }
    public override void AcumularCreditos(decimal cantidad)
    {
        Creditos += cantidad * 0.02m;
    }
}
class CuentaBronce : Cuenta
{
    public CuentaBronce(string numeroCuenta, decimal saldo) : base(numeroCuenta, saldo)
    {
    }
    public override void AcumularCreditos(decimal cantidad)
    {
        Creditos += cantidad * 0.01m;
    }
}



abstract class Operacion
{
    public string NumeroCuenta { get; }
    public decimal Monto { get; }
    public Cuenta Cuenta { get; set; }

    public Operacion(string numeroCuenta, decimal monto)
    {
        this.NumeroCuenta = numeroCuenta;
        this.Monto = monto;
    }

    public virtual void EjecutarAccion(List<Cliente> clientes)
    {
        Cuenta.GuardarHistorial(this);
    }

    public abstract string Informe();
}
class Deposito : Operacion
{

    public Deposito(string numeroCuenta, decimal monto) : base(numeroCuenta, monto)
    {
    }
    public override void EjecutarAccion(List<Cliente> clientes)
    {

        this.Cuenta = Banco.BuscarCuenta(NumeroCuenta, clientes); // Buscar la cuenta
        if (this.Cuenta == null)
        {
            throw new Exception("La cuenta especificada no existe.");
        }

        // operacion.Cuenta = cuentaEncontrada; 

        Cuenta.Depositar(Monto);
        base.EjecutarAccion(clientes);
    }

    public override string Informe()
    {
        return $"Depósito de {Monto} en la cuenta {NumeroCuenta}";
    }
}
class Retiro : Operacion
{

    public Retiro(string numeroCuenta, decimal monto) : base(numeroCuenta, monto)
    {
    }
    public override void EjecutarAccion(List<Cliente> clientes)
    {
        this.Cuenta = Banco.BuscarCuenta(NumeroCuenta, clientes); // Buscar la cuenta
        if (this.Cuenta == null)
        {
            throw new Exception("La cuenta especificada no existe.");
        }
        Cuenta.Retirar(Monto);
        base.EjecutarAccion(clientes);
    }

    public override string Informe()
    {
        return $"Retiro de {Monto} en la cuenta {NumeroCuenta}";
    }
}
class Transferencia : Operacion
{
    private string NumeroCuentaDestino { get; }
    private Cuenta Destino { get; set; }

    public Transferencia(string numeroCuentaOrigen, string numeroCuentaDestino, decimal monto)
        : base(numeroCuentaOrigen, monto)
    {
        this.NumeroCuentaDestino = numeroCuentaDestino;
    }

    public override void EjecutarAccion(List<Cliente> clientes)
    {
        // Buscar la cuenta de origen
        this.Cuenta = Banco.BuscarCuenta(NumeroCuenta, clientes);
        if (this.Cuenta == null)
        {
            throw new Exception($"Error: La cuenta origen {NumeroCuenta} no existe.");
        }

        // Buscar la cuenta de destino
        this.Destino = Banco.BuscarCuenta(NumeroCuentaDestino, clientes);
        if (this.Destino == null)
        {
            throw new Exception($"Error: La cuenta destino {NumeroCuentaDestino} no existe.");
        }

        // Realizar la transferencia
        if (Cuenta.Transferir(Monto, Destino))
        {
            base.EjecutarAccion(clientes);
        }
        else
        {
            Console.WriteLine($"Error: Saldo insuficiente en la cuenta {Cuenta.NumeroCuenta} para transferir {Monto}");
        }


    }

    public override string Informe()
    {
        return $"Trasferencaia de {Monto} de la cuenta {Destino.NumeroCuenta} a {NumeroCuenta}";
    }
}
class Pago : Operacion
{
    public Pago(string numeroCuenta, decimal monto) : base(numeroCuenta, monto)
    {
    }
    public override void EjecutarAccion(List<Cliente> clientes)
    {
        this.Cuenta = Banco.BuscarCuenta(NumeroCuenta, clientes); // Buscar la cuenta
        if (this.Cuenta == null)
        {
            throw new Exception("La cuenta especificada no existe.");
        }
        Cuenta.Pagar(Monto);
        base.EjecutarAccion(clientes);
    }

    public override string Informe()
    {
        return $"Pago de {Monto} en la cuenta {NumeroCuenta}";
    }
}


/// EJEMPLO DE USO ///

// Definiciones 

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


// Registrar Operaciones
nac.Registrar(new Deposito("10001", 100));
nac.Registrar(new Retiro("10002", 200));
nac.Registrar(new Transferencia("10001", "10002", 300));
nac.Registrar(new Transferencia("10003", "10004", 500));
nac.Registrar(new Pago("10002", 400));

tup.Registrar(new Deposito("10005", 100));
tup.Registrar(new Retiro("10005", 200));
// //no funciona, si no tiene una lista de todos los bancos arreglar
// tup.Registrar(new Transferencia("10005", "10002", 300)); 
tup.Registrar(new Pago("10005", 400));


// Informe final
nac.Informe();
tup.Informe();

