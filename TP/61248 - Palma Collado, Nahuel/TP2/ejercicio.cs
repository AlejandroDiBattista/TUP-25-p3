using System;
using System.Collections.Generic;

public class Banco {
    public string Nombre { get; private set; }
    public List<Cliente> Clientes { get; private set; }
    public List<Operacion> Operaciones;

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
            operacion.Origen.Registrar(operacion);
        }
    }

    public void Informe() { 
        Console.WriteLine($"Banco: {Nombre} | Clientes: {Clientes.Count}\n");

        foreach (var cliente in Clientes)
        {
            decimal saldoTotal = cliente.Cuentas.Sum(c => c.Saldo);
            decimal puntosTotal = cliente.Cuentas.Sum(c => c.Puntos);

            Console.WriteLine($"  Cliente: {cliente.Nombre} | Saldo Total: {saldoTotal:C} | Puntos Total: {puntosTotal:C}\n");

            foreach (var cuenta in cliente.Cuentas)
            {
            Console.WriteLine($"    Cuenta: {cuenta.Numero} | Saldo: {cuenta.Saldo:C} | Puntos: {cuenta.Puntos:C}");
            foreach (var operacion in cuenta.Historial)
            {
                Console.WriteLine($"     - {operacion.Descripcion}");
            }
            Console.WriteLine();
            }
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

public class Cliente {
    public string Nombre { get; private set; }
    public List<Cuenta> Cuentas { get; private set; }
    

    public Cliente(string nombre) { 
        Nombre = nombre;
        Cuentas = new List<Cuenta>();
    }

    public void Agregar(Cuenta cuenta) { 
        Cuentas.Add(cuenta);
        Banco.Registrar(cuenta);
    }

    public void Informe(){
        WriteLine($"  Cliente: {Nombre} Saldo Total: {Cuentas.Sum(c => c.Saldo):C0}");
        foreach(var c in Cuentas){
            c.Informe();
        }
    }
}

public class Cuenta {
    public string Numero { get; private set; }
    public decimal Saldo { get; set; }
    public decimal Puntos { get; set; } = 0;
    public List<Operacion> Historial { get; private set; } = new();

    public Cuenta(string numero, decimal saldo) { 
        Numero = numero;
        Saldo = saldo;
    }

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
        WriteLine($"   Cuenta: {Numero}  Saldo: {Saldo:C}");
        foreach(var o in Historial){
            WriteLine($"    -{o.Descripcion}");
        }
    }
public virtual void AcumularPuntos(decimal cantidad){}
}

class CuentaOro : Cuenta{
    public CuentaOro(string numero, decimal saldo) : base(numero, saldo){}
    
    public override void AcumularPuntos(decimal cantidad){
        decimal tasa = cantidad > 1000 ? 0.05m : 0.03m;
        Puntos += cantidad * tasa;
    }
}
class CuentaPlata : Cuenta{
    public CuentaPlata(string numero, decimal saldo) : base(numero, saldo){}
    
    public override void AcumularPuntos(decimal cantidad){
        Puntos += cantidad * 0.02m;
    }
}
class CuentaBronce : Cuenta{
    public CuentaBronce(string numero, decimal saldo) : base(numero, saldo){}
    
    public override void AcumularPuntos(decimal cantidad){
        Puntos += cantidad * 0.01m;
    }
}

public abstract class Operacion {
    public Cuenta Origen { get; private set; }
    public decimal Monto { get; private set; }

    public Operacion(string numero, decimal monto) { 
        Origen = Banco.Buscar(numero);
        Monto = monto;
    }

    public abstract bool Ejecutar();

    public virtual string Descripcion => string.Empty;
}

public class Depositar : Operacion {
    public Depositar(string numero, decimal monto) : base(numero, monto) { }

    public override bool Ejecutar() { 
        return Origen.Poner(Monto);
    }

    public override string Descripcion => $"Deposito {Monto:C0} a {Origen.Numero}";
}

public class Extraer : Operacion {
    public Extraer(string numero, decimal monto) : base(numero, monto) { }

    public override bool Ejecutar() { 
        return Origen.Sacar(Monto);
    }

    public override string Descripcion => $"Extrado {Monto:C0} de {Origen.Numero}";                       
}

public class Transferencia : Operacion {
    public Cuenta Destino { get; set; }

    public Transferencia(string origen, string destino, decimal monto) : base(origen, monto) { 
        Destino = Banco.Buscar(destino);
    }

    public override bool Ejecutar() { 
        if (!Origen.Sacar(Monto)) return false;
        if (!Destino.Poner(Monto)) {
            Origen.Poner(Monto); // Devolver el monto a la cuenta de origen
            return false;
        }
        return true;
    }

    public override string Descripcion {
        get => $"Transfiero {Monto:C0} de {Origen.Numero} a {Destino.Numero}";
    }
}

public class Pago : Operacion
{
    public Pago(string numero, decimal monto) : base(numero, monto) { }

    public override bool Ejecutar()
    {
        if (Origen.Sacar(Monto))
        {
            Origen.AcumularPuntos(Monto);
            return true;
        }
        return false;
    }

    public override string Descripcion => $"Pago de {Monto:C0} desde {Origen.Numero}";
}



var Juan = new Cliente("Juan Perez");
    Juan.Agregar(new CuentaOro("10001", 1000));
    Juan.Agregar(new CuentaPlata("10002", 2000));

var Maria = new Cliente("Maria Lopez");
    Maria.Agregar(new CuentaPlata("10003", 3000));
    Maria.Agregar(new CuentaPlata("10004", 4000));

var Pedro = new Cliente("Pedro Gomez");
    Pedro.Agregar(new CuentaBronce("10005", 5000));

var BancoNacional = new Banco("Banco Nacional");
BancoNacional.Agregar(Juan);
BancoNacional.Agregar(Maria);

var BancoTecnologico = new Banco("Banco Tecnologico");
BancoTecnologico.Agregar(Pedro);      

BancoNacional.Registrar(new Depositar("10001", 100));
BancoNacional.Registrar(new Extraer("10002", 200));
BancoNacional.Registrar(new Transferencia("10001", "10002", 300));
BancoNacional.Registrar(new Transferencia("10003", "10004", 500));
BancoNacional.Registrar(new Pago("10002", 400));

BancoTecnologico.Registrar(new Depositar("10005", 100));
BancoTecnologico.Registrar(new Extraer("10005", 200));
BancoTecnologico.Registrar(new Transferencia("10005", "10002", 300));
BancoTecnologico.Registrar(new Pago("10005", 400));

BancoNacional.Informe();
BancoTecnologico.Informe();

