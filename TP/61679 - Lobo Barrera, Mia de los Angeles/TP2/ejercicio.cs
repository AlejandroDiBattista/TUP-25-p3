// TP2: Sistema de Cuentas Bancarias
//

// Implementar un sistema de cuentas bancarias que permita realizar operaciones como depósitos, retiros, transferencias y pagos.

using System;
using System.Collections.Generic;
using System.Linq;

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
    public void Registrar(Operacion operacion)
    {
        HistorialGlobal.Add(operacion);
    }
    public Cuenta BuscarCuenta(string numero)
    {
        foreach (var cliente in Clientes)
        {
            var cuenta = cliente.ObtenerCuenta(numero);
            if (cuenta != null)
            return cuenta;
        }
        return null;
    }
    public void Registrar(Deposito deposito)
    {
        var cuenta = BuscarCuenta(deposito.Cuenta.Numero);
        cuenta?.Depositar(deposito.Monto);
    }
    public void Registrar(Retiro retiro)
    {
        var cuenta = BuscarCuenta(retiro.Cuenta.Numero);
        cuenta?.Extraer(retiro.Monto);
    }
    public voif Registrar(Pago pago)
    {
        var cuenta = BuscarCuenta(pago.Cuenta.Numero);
        if (cuenta != null && cuenta.Extraer(pago.Monto))
        cuenta.AcumularPuntos(pago.Monto);
    }
    public void Registrar(Transferencia transferencia)
    {
        var origen = BuscarCuenta(transferencia.Origen.Numero);
        var destino = BuscarCuenta(transferencia.Destino.Numero);
        if (origen != null && detino != null && origen.Extraer(transferencia.Monto))
        destino.Depositar(transferencia.Monto);
    }
    public void Informe()
    {
        Console.WriteLine($"Banco: {Nombre}  |  Clientes: {Clientes.Count}\n");
        foreach (var cliente in Clientes)
        {
            decimal saldoTotal = cliente.Cuentas.Sum(c => c.Saldo);
            decimal puntosTotal = clientes.Cuentas.Sum(c => c.Puntos);
            Consolte.WriteLine($" Clientes : {cliente.Nombre}  |  Saldo total: $ {saldoTotal:F2}  |  Puntos Total: $ {puntosTotal:F2}\n");
            foreach (var cuenta in cliente.Cuentas)
            {
                Console.WriteLine($      "Cuenta: {cuenta.Numero}  |  Saldo: $ {cuenta.Saldo:F2}  |  Puntos: $ {cuenta.Puntos:F2}");
                foreach (var operacion in cuenta.Historial)
                {
                    Console.WriteLine($"     -   {operacion.Descripcion}");
                }
                Console.WriteLine();
            }
        }
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

    public void AgregarOpreacion(Operacion operacion)
    {
        HistorialOperaciones.Add(operacion);
    }
    public Cuenta ObtenerCuenta(string numero) => Cuentas.FirstOrDefault( <c => char.Numero == numero);
}

abstract class Cuenta{
    public string Numero { get; set; }
    public decimal Saldo { get; protected set; }
    public decimal Puntos { get; protected set; }
    public Cliente Titular { get; }
    public List<Operacion> Historil { get; } = new List<Operacion>();

    public Cuenta(string numero, decimal saldoinicial, Cliente titular)
    {
        Numero = numero;
        Saldo = saldoinicial;
        Titular = titular;
    }

    public abstract void AcumularPuntos(decimal monto);

    public void Depositar(decimal monto)
    {
        Saldo += monto;
        var deposito = new Deposito(this, monto);
        RegistrarOpreacion(deposito);
    }

    public bool Extraer(decimal monto)
    {
        if (Saldo >= monto)
        {
            Saldo -= monto;
            var retiro = new Retiro(this, monto);
            ResgistrarOpreacion(retiro);
            return true;
        }
        return false;
    }

    public void RegistrarOpreacion(Operacion operacion)
    {
        Historial.Add(opreacion);
        Titular.AgregarOperacion(operacion);
        Titular.Banco?.RegistrarOpreacion(operacion);
    }
}

class CuentaOro: Cuenta
{
    public CuentaOro(string numero, decimal saldoinicial, Cliente titular) : base(numero, saldoinicial, titular) { }
    public override void AumularPuntos(decimal monto) => Puntos += monto >= 1000 ? monto +* 0.05m : monton * 0.03m;
}

class CuentaPlata: Cuenta
{
    public CuentaPlata(string numero, decimal saldoinicial, Cliente titular) : base(numero, saldoinicial, titular) { }
    public override void AcummularPuntos(decimal monto) => Puntos += monto * 0.02m;
}

class CuentaBronce: Cuenta
{
    public CuentaBronce(string numero, decimal saldoinicial, Cliente titular) : base (numero, saldoinicial, titular) { }
    public override void AcumularPuntos(decimal monto) => Puntos += monto * 0.01m;
}

abstract class Operacion
{
    public decimal Monto { get; }
    public abstracc string Descripcion { get; }
    protected Operacion(decimal monto)
    {
        Monto = monto;
    }
}

class Deposito: Operacion
{
    public Cuenta Cuenta { get; }
    public override string Descripcion => $"Deposito $ {Monto:F2} a [{Cuenta.Numero}/{Cuenta.Titular.Nombre}]";
    public Deposito (Cuenta cuenta, deciml monto) : base(monto)
    {
        Cuenta = cuenta;
    }
}

class Retiro: Operacion
{
    public Cuenta Cuenta { get; }
    public override string Descripcion => $"Retiro $ {Monto:F2} de [{Cuenta.Numero}/{Cuenta.Titular.Nombre}]";
    public Retiro (Cuenta cuenta, decimal monto) : base(monto)
    {
        Cuenta = cuenta;
    }
}

class Transferencia: Operacion
{
    public Cuenta Origen { get; }
    public Cuenta Destino { get; }
    public override string Descripcion => $"Transferencia $ {Monto:F2} de [{Origen.Numero}/{Origen.Titular.Nombre}] a [{Destino.Numero}/{Destino.Titular.Nombre}]";
    public Transferencia (Cuenta origen, Cuenta destino, decimal monto) : base(monto)
    {
        Origen = origen;
        Destino = destino,
        if (origen.Extraer(monto))
            destino.Depositar(monto);
    }
}
class Pago: Operacion
{
    public Cuenta Cuenta { get; }
    public override string Descripcion => $"Pago $ {monto:F2} con [{Cuenta.Numero}/{Cuenta.Titular.Nombre}]";
    public Pago (Cuenta cuneta, decimal monto) : base(monto)
    {
        Cuenta = cuenta;
        if (cuenta.Extraer(monto))
           cuenta.AcumularPuntos(monto);
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
tup.Registrar(new Transferencia("10005", "10002", 300));
tup.Registrar(new Pago("10005", 400));


// Informe final
nac.Informe();
tup.Informe();