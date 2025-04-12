// TP2: Sistema de Cuentas Bancarias
//

// Implementar un sistema de cuentas bancarias que permita realizar operaciones como depósitos, retiros, transferencias y pagos.
using System;
using System.Collections.Generic;

public class Banco{
    public string Nombre { get; private set; }
    public List<Cliente> Clientes { get; private set; }
    public List<Operacion> Operaciones { get; private set; }

    public Banco(string nombre) { 
        Nombre = nombre;
        Clientes = new List<Cliente>();
        Operaciones = new List<Operacion>();
    }

    public void Agregar(Cliente cliente) { 
        Clientes.Add(cliente);
    }

    public void Registrar(Operacion operacion) { 
        if (operacion.Ejecutar()) {
            Operaciones.Add(operacion);
            operacion.Origen.RegistrarOperacion(operacion);
        }
    }

    public void Informe() { 
        foreach (var c in Clientes) {
            c.Informe();
        }
    }
    public static Dictionary<string, Cuenta> Cuentas = new Dictionary<string, Cuenta>();
    public static void Registrar(Cuenta cuenta) { 
        if (Cuentas.ContainsKey(cuenta.Numero)) {
            Console.WriteLine($"La cuenta {cuenta.Numero} ya existe");
        } else {
            Cuentas.Add(cuenta.Numero, cuenta);
        }
    }
    public static Cuenta Buscar(string numero) { 
        if (Cuentas.ContainsKey(numero)) {
            return Cuentas[numero];
        } else {
            return null;
        }
    }
}
public class Cliente{
    public string Nombre { get; private set; }
    public List<Cuenta> Cuentas { get; private set; }

    public Cliente(string nombre) { 
        Nombre = nombre;
        Cuentas = new List<Cuenta>();
    }

    public void Agregar(Cuenta cuenta) { 
        cuenta.Cliente = this;
        Cuentas.Add(cuenta);
        Banco.Registrar(cuenta);
    }
    
    public void Informe() { 
        Console.WriteLine($"Informe de {Nombre}: Saldo Total: {Cuentas.Sum(c => c.Saldo):C0}");
        foreach (var cuenta in Cuentas) {
            cuenta.Informe();
        }
    }   
}

public abstract class Cuenta{
 public string Numero { get; private set; }
 public decimal Saldo { get; set; } 
    public decimal Creditos { get; set; }
    public List<Operacion> Historia { get; set; } = new List<Operacion>();
 public Cliente Cliente { get; set; }

    public Cuenta(string numero, decimal saldo) {
        Numero = numero;
        Saldo = saldo;
    }
    public bool Poner(decimal cantidad) {
        if (cantidad <= 0) {
            return false;
        }
        Saldo += cantidad;
        return true;
    }
    public bool Quitar(decimal cantidad) {
        if (cantidad > Saldo || cantidad <= 0) {
            return false;
        }
        Saldo -= cantidad;
        return true;
    }

    
    public bool Pagar(decimal cantidad) {
        if (!Quitar(cantidad)) {
            return false;
        }
        AcumularCreditos(cantidad);
        return true;
    }

    public abstract void AcumularCreditos(decimal cantidad);

    public void RegistrarOperacion(Operacion operacion) {
        Historia.Add(operacion);
    }

    public void Informe() { 
        Console.WriteLine($"------------------");
        Console.WriteLine($"Informe de cuenta: ");
        Console.WriteLine($"  N° de Cuenta: {Numero}");
        Console.WriteLine($"  Tipo de Cuenta: {this.GetType().Name}");
        Console.WriteLine($"  Total de Saldo: {Saldo}");
        Console.WriteLine($"  Total de Creditos: {Creditos}");
        Console.WriteLine($"  Total de operaciones: {Historia.Count}");
     Console.WriteLine($"  Historial de operaciones: ");
        foreach (var operacion in Historia) {
            Console.WriteLine($"   - {operacion.Descripcion()}");
        }
        Console.WriteLine($"------------------");
    }
}
public class CuentaOro: Cuenta{
    public CuentaOro(string numero, decimal saldo) : base(numero, saldo) {}

    public override void AcumularCreditos(decimal cantidad) {
        if(cantidad>1000)
        {
            Creditos += cantidad * 0.05m; 
        }
        else{
           Creditos += cantidad * 0.03m;
    
        }
    }
}
public class CuentaPlata: Cuenta{
    public CuentaPlata(string numero, decimal saldo) : base(numero, saldo) {}

    public override void AcumularCreditos(decimal cantidad) {
        Creditos += cantidad * 0.02m;
    }
}
public class CuentaBronce: Cuenta{
    public CuentaBronce(string numero, decimal saldo) : base(numero, saldo) {}
    public override void AcumularCreditos(decimal cantidad) {
        Creditos += cantidad * 0.01m;
    }
}

public abstract class Operacion{
    public Cuenta Origen { get; private set; }
    public decimal Monto { get; private set; }
    public DateTime Fecha { get; set; } = DateTime.Now;
    public Operacion(string numero, decimal monto) { 
        Origen = Banco.Buscar(numero);
        Monto = monto;
    }

    public virtual bool Ejecutar(){
         Origen.RegistrarOperacion(this);
        return true;
    }

    public virtual string Descripcion() => string.Empty;
}
public class Deposito: Operacion{ 
    public Deposito(string numero, decimal monto) : base(numero, monto) { }

    public override bool Ejecutar() { 
        if(Origen.Poner(Monto)) {
          base.Ejecutar();
            return true;
    }
    else {
        return false;
    }
    }
    public override string Descripcion()
    {
        return  $" {Fecha}: Deposito de ${Monto} en la cuenta {Origen.Numero}";
}
}
public class Retiro: Operacion{
    public Retiro(string numero, decimal monto) : base(numero, monto) { }

    public override bool Ejecutar() { 
        if(Origen.Quitar(Monto)) {
            base.Ejecutar();
            return true;    
    }
    else {
        return false;
    }
    }

    public override string Descripcion()
    {
        return  $" {Fecha}: Retiro de ${Monto} de la cuenta {Origen.Numero}";
    }
}
public class Transferencia: Operacion{
    public Cuenta Destino { get; set; }

    public Transferencia(string origen, string destino, decimal monto) : base(origen, monto) { 
        Destino = Banco.Buscar(destino);
    }
 
       public override bool Ejecutar() {
    if (Origen.Quitar(Monto)) {
        if (Destino.Poner(Monto)) {
            base.Ejecutar();
            return true;
        } else {
            Origen.Poner(Monto); // rollback si falla
            return false;
        }
        }

        else {
            return false;
        }
       }

    public override string Descripcion()
    {
        return  $" {Fecha}: Transferencia de ${Monto} de la cuenta {Origen.Numero} a la cuenta {Destino.Numero}";
    }
}
public class Pago: Operacion{
    public Pago(string numero, decimal monto) : base(numero, monto) { }

    public override bool Ejecutar() { 
         if(Origen.Pagar(Monto)) {
            base.Ejecutar();
            return true;
         }
            else {
            return false;
            }
    }

    public override string Descripcion()
    {
        return  $" {Fecha}: Pago de ${Monto} de la cuenta {Origen.Numero}";
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

