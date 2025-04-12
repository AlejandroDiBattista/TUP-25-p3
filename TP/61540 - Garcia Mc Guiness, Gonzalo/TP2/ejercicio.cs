// TP2: Sistema de Cuentas Bancarias
//

// Implementar un sistema de cuentas bancarias que permita realizar operaciones como depósitos, retiros, transferencias y pagos.

using static System.Console;
using System.Collections.Generic;
using System.Security.Cryptography;

//BANCO
public class Banco
{
    public string Nombre { get; private set; }
    public List<Cliente> Clientes { get; private set; } 
    public List<Operacion> Operaciones;

    public Banco(string nombre)
    {
        Nombre = nombre;
        Clientes = new List<Cliente>();
        Operaciones = new List<Operacion>();
    }
    public void Agregar(Cliente agregar)
    {
        Clientes.Add(agregar);
    }
    

//REGISTRAR CUENTA
    public static Dictionary<string, Cuenta> Cuentas = new Dictionary<string, Cuenta>();

    public static void Registrar(Cuenta cuenta)
    {
        if (Cuentas.ContainsKey(cuenta.ClaveBancaria))
        {
            WriteLine($"La cuenta {cuenta.ClaveBancaria} ya existe");
        }
        else{
            Cuentas.Add(cuenta.ClaveBancaria, cuenta);
        }
    }
    public static Cuenta Buscar(string claveBancaria)
    {
        if (Cuentas.ContainsKey(claveBancaria))
        {
            return Cuentas[claveBancaria];
        }
        else
        {
            return null;
        }
    }
    public void Registrar(Operacion operacion)
    {
        if(operacion.Ejecutar())
        {
            Operaciones.Add(operacion);
            operacion.Origen.Registrar(operacion);
        }
    }
    public void Informe() { 
        foreach (var c in Clientes) {
            c.Informe();
        }
    }
    
}
//CLIENTE
public class Cliente 
{
    public string Nombre { get; private set;}
    public List<Cuenta> Cuentas { get; private set; }

    public Cliente(string nombre)
    {
        Nombre = nombre;
        Cuentas = new List<Cuenta>();
    }
    public void Agregar(Cuenta cuenta)
    {
        Cuentas.Add(cuenta);
        Banco.Registrar(cuenta);
    }
    public void Informe(){
        WriteLine($"  Cliente:| {Nombre} | Saldo Total:|  {Cuentas.Sum(c => c.Saldo):C0} | Puntos:| {Cuentas.Sum(c => c.Puntos):C0}");
        foreach(var c in Cuentas){
            c.Informe();
        }
    }

}
//CUENTA
public abstract class Cuenta
{
    public string ClaveBancaria { get; private set; }
    public decimal Saldo { get; private set; }
    public decimal Puntos { get; set; } = 0; 
    public List<Operacion> Historial { get; private set; } = new();

    public Cuenta (string claveBancaria, decimal saldo)
    {
        ClaveBancaria = claveBancaria;
        Saldo = saldo;
    }
    public abstract void AcumularPuntos(decimal cantidad);
    public bool Poner(decimal cantidad) {
        if (cantidad <= 0) return false;
        Saldo += cantidad;
        return true;
    }

    public bool Sacar(decimal cantidad) {
        if (cantidad <= 0 || cantidad > Saldo) return false;
        Saldo -= cantidad;
        return true;
    }

    public void Registrar(Operacion operacion){
        Historial.Add(operacion);
    }

    public void Informe(){
        WriteLine($"   Cuenta:| {ClaveBancaria} | Tipo:| {GetType().Name} | Saldo:| {Saldo:C}");
        foreach (var o in Historial)
        {
            WriteLine($"    -{o.Descripcion}");
        }
    }
}
//TIPOS DE CUENTA
class CuentaOro : Cuenta
{
    public CuentaOro ( string claveBancaria, decimal saldo) : base(claveBancaria, saldo) { }

    public override void AcumularPuntos(decimal cantidad)
    {
        if(cantidad >= 1000)
        {
            Puntos += cantidad * 0.05m;
        }
        else{
            Puntos += cantidad * 0.03m;
        }
    }
}
class CuentaPlata : Cuenta
{
    public CuentaPlata ( string claveBancaria, decimal saldo) : base(claveBancaria, saldo) { }

    public override void AcumularPuntos(decimal cantidad)
    {
        Puntos += cantidad * 0.02m;
    }
}
class CuentaBronce : Cuenta
{
    public CuentaBronce ( string claveBancaria, decimal saldo) : base(claveBancaria, saldo) { }
    
    public override void AcumularPuntos(decimal cantidad)
    {
        Puntos += cantidad * 0.01m;
    }
}
//OPERACIONES
public abstract class Operacion
{
    public Cuenta Origen {get; private set;}
    public decimal Monto {get; private set;}

    public Operacion(string claveBancaria, decimal monto)
    {
        Origen = Banco.Buscar(claveBancaria);
        Monto = monto;
    }
    public abstract bool Ejecutar();
    public virtual string Descripcion {get;}

}
public class Deposito : Operacion
{
    public Deposito(string claveBancaria, decimal monto) : base(claveBancaria, monto) { }

    public override bool Ejecutar()
    {
        return Origen.Poner(Monto);
    }
    public override string Descripcion => $"Deposito:| {Monto:C} | a la cuenta {Origen.ClaveBancaria}";
}
public class Retiro : Operacion
{
    public Retiro(string claveBancaria, decimal monto) : base(claveBancaria, monto){}
    
    public override bool Ejecutar()
    {
        return Origen.Sacar(Monto);
    }
    public override string Descripcion => $"Retiro:| {Monto:C} | de la cuenta {Origen.ClaveBancaria}";
}
public class Transferencia : Operacion
{
    public Cuenta Destino {get;set;}
    public Transferencia(string claveBancariaOrigen, string claveBancariaDestino, decimal monto) : base(claveBancariaOrigen, monto)
    {
        Destino = Banco.Buscar(claveBancariaDestino);
    }
    public override bool Ejecutar() 
    { 
        if (!Origen.Sacar(Monto)) return false;
        if (!Destino.Poner(Monto)) {
            Origen.Poner(Monto);
            return false;
        }
        return true;
    }
    public override string Descripcion {get => $"Transfiero:| {Monto:C0} | de {Origen.ClaveBancaria} | a {Destino.ClaveBancaria}";}
}
public class Pago : Operacion
{
    public Pago(string claveBancaria, decimal monto) : base(claveBancaria, monto) { }

    public override bool Ejecutar()
    {
        if(Origen.Sacar(Monto))
        {
            Origen.AcumularPuntos(Monto);
            return true;
        }
        return false;
    }
    public override string Descripcion => $"Pago:| {Monto:C} | de la cuenta {Origen.ClaveBancaria} ";
}

public class Program
{
    public static void Main(string[] args)
    {
        Clear();
        WriteLine("Banco TUP y Banco Nacion");
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
    }
}

