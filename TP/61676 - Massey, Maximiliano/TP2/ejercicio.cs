using System;
using System.Collections.Generic;
using System.Linq;
using static System.Console;
class Banco
{

    public string Nombre { get; private set; }

    public List<Cliente> clientes { get; private set; } = new List<Cliente>();
    public List<Cuenta> Cuentas { get; private set; } = new List<Cuenta>();
    public List<Operacion> Operacion { get; private set; } = new List<Operacion>();
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
        if (operacion.Ejecutar())
        {
            Operacion.Add(operacion);

        }
    }
    public void Informe() {
        Console.WriteLine($"\nBanco: {Nombre} | Clientes: {clientes.Count} | Cuentas: {Cuentas.Count} | Operaciones: {Operacion.Count}");
        foreach (var cliente in clientes)
        {
            cliente.Resumen();
        }
    }

}
class Cliente { 
    public string Nombre{ get; private set; }

    public List<Cuenta> Cuenta { get; private set; } = new List<Cuenta>();
    
    public  Cliente(string nombre) {
        Nombre = nombre;
    }
    public void Agregar(Cuenta cuenta) {
        Cuenta.Add(cuenta);
    }
    public void Resumen() {
        Console.WriteLine($"\nCliente: {Nombre} | Cuentas: {Cuenta.Count}");
        foreach (var cuenta in Cuenta) {
            Console.WriteLine(cuenta.ToString());
        }
    }
}
public abstract class Cuenta
{
    public string Numero { get; private set; }
    public decimal Saldo { get; set; }
    public decimal puntos { get; set; }
    public Cuenta(string numero, decimal saldo )
    {
        Numero = numero;
        Saldo = saldo;
    }
    public bool Depositar(decimal monto)
    {
        if (monto <= 0) return false;
            Saldo += monto;
            return true;
    }
    public bool Retirar(decimal monto)
    {
        if (monto <=0  || monto >Saldo ) return false;
            Saldo -= monto;
            return true;
        
    }
    public void AcumularCreditos(decimal monto, decimal porcentaje){
        puntos += monto * porcentaje;
    }
    public abstract void AcumularPuntos(decimal monto);
    
    public void resumen(){
        Console.WriteLine($"Numero: {Numero} | Saldo: {Saldo} | Puntos: {puntos}");
    }
}
public class CuentaOro : Cuenta
{
    public CuentaOro(string numero, decimal saldo = 0) : base(numero, saldo) { }
    public override void AcumularPuntos(decimal monto)
    {
        if (monto > 1000)
        {
            puntos += monto * 0.05m;
        }
        else if (monto > 500)
        {
            puntos += monto * 0.03m;
        }
        else
        {
            puntos += monto * 0.01m;
        }
    }
}
public class CuentaPlata : Cuenta
{
    public CuentaPlata(string numero, decimal saldo = 0) : base(numero, saldo) { }
    public override void AcumularPuntos(decimal monto)
    {
        if (monto > 1000)
        {
            puntos += monto * 0.03m;
        }
        else
        {
            puntos += monto * 0.01m;
        }

    }
}
public class CuentaBronce : Cuenta
{
    public CuentaBronce(string numero, decimal saldo = 0) : base(numero, saldo) { }
    public override void AcumularPuntos(decimal monto)
    {
        if (monto > 1000)
        {
            puntos += monto * 0.02m;
        }
    }
}
abstract class Operacion { 
    public Cuenta Origen { get; private set; }
    public decimal Monto { get; private set; }

    public Operacion(Cuenta origen, decimal monto) {
        Origen = origen;
        Monto = monto;
    }
    public abstract bool Ejecutar();
    public abstract string Descripcion();
}

class Deposito: Operacion
{
    public Deposito(Cuenta origen, decimal monto) : base(origen, monto) { }
    public override bool Ejecutar()
    {
        return Origen.Depositar(Monto);
    }
    public override string Descripcion()
    {
        return $"Depositar: {Monto:0.00} en {Origen.Numero}";
    }
}
class Retiro: Operacion
{
    public Retiro(Cuenta origen, decimal monto) : base(origen, monto) { }
    public override bool Ejecutar()
    {
        return Origen.Retirar(Monto);
    }
    public override string Descripcion()
    {
        return $"Retirar: {Monto:0.00} de {Origen.Numero}";
    }
}
class Pago : Operacion {
    public Pago(Cuenta origen, decimal monto) : base(origen, monto) { }

    public override bool Ejecutar() {
        if (Origen.Retirar(Monto)) {
            Origen.AcumularPuntos(Monto);
            return true;
        }
        return false;
    }

    public override string Descripcion() {
        return $"Pago $ {Monto:0.00} con {Origen.Numero}";
    }
}
class Transferencia : Operacion {
    public Cuenta Destino { get; private set; }

    public Transferencia(Cuenta origen, Cuenta destino, decimal monto) : base(origen, monto) {
        Destino = destino;
    }

    public override bool Ejecutar() {
        if (Origen.Retirar(Monto)) {
            Destino.Depositar(Monto);
            return true;
        }
        return false;
    }

    public override string Descripcion() {
        return $"Transferencia $ {Monto:0.00} de {Origen.Numero} a {Destino.Numero}";
    }
}

class Program
{
    static void Main(string[] args)
    {

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

    var tup = new Banco("Banco TUP");
    tup.Agregar(luis);

nac.Registrar(new Deposito(raul.Cuenta[0], 100));
nac.Registrar(new Retiro(raul.Cuenta[1], 200));
nac.Registrar(new Transferencia(raul.Cuenta[0], raul.Cuenta[1], 300));
nac.Registrar(new Transferencia(sara.Cuenta[0], sara.Cuenta[1], 500));
nac.Registrar(new Pago(raul.Cuenta[1], 400));

tup.Registrar(new Deposito(luis.Cuenta[0], 100));
tup.Registrar(new Retiro(luis.Cuenta[0], 200));
tup.Registrar(new Transferencia(luis.Cuenta[0], raul.Cuenta[1], 300));
tup.Registrar(new Pago(luis.Cuenta[0], 400));

nac.Informe();
tup.Informe();
    }
}

