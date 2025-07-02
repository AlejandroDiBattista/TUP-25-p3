using System;
using System.Collections.Generic;

// Metodos del Banco 
public partial class Banco {
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
        foreach (var c in Clientes) {
            c.Informe();
        }
    }

}

// Metodos de clase para controlar las cuentas
partial class Banco {
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

// Clientes con multiples cuentas
public class Cliente {
    public string Nombre { get; private set; }
    public List<Cuenta> Cuentas { get; private set; }

    public Cliente(string nombre) { 
        Nombre = nombre;
        Cuentas = new List<Cuenta>();
    }

    public void Agregar(Cuenta cuenta) { 
        Cuentas.Add(cuenta);
        cuenta.Cliente = this; // Asignar el cliente a la cuenta
        Banco.Registrar(cuenta);
    }

    public void Informe(){
        WriteLine($"\n  Cliente: {Nombre} Saldo Total: {Cuentas.Sum(c => c.Saldo):C0}");
        foreach(var c in Cuentas){
            c.Informe();
        }
    }
}

// Cuenta con historial de operaciones
public abstract class Cuenta {
    public string Numero { get; private set; }
    public decimal Saldo { get; set; }
    public decimal Puntos { get; set; } = 0;

    public List<Operacion> Historial { get; private set; } = new();
    public Cliente Cliente { get; set; } = null;

    public Cuenta(string numero, decimal saldo) { 
        Numero = numero;
        Saldo  = saldo;
    }

    public bool Acreditar(decimal cantidad) {
        if (cantidad <= 0) return false;
        Saldo += cantidad;
        return true;
    }

    public bool Debitar(decimal cantidad) {
        if (cantidad <= 0 || cantidad > Saldo) return false;
        Saldo -= cantidad;
        return true;
    }

    public abstract void Acumular(decimal cantidad);

    public void Registrar(Operacion operacion){
        Historial.Add(operacion);
    }

    public void Informe(){
        WriteLine($"   Cuenta: {Numero}  Saldo: {Saldo:C}");
        foreach(var o in Historial){
            WriteLine($"    - {o.Descripcion}");
        }
    }
}

public class CuentaOro : Cuenta {
    public CuentaOro(string numero, decimal saldo) : base(numero, saldo) { }

    public override void Acumular(decimal cantidad) {
        Puntos += cantidad * (cantidad > 1000 ? 0.05m : 0.03m);
    }
}

public class CuentaPlata : Cuenta {
    public CuentaPlata(string numero, decimal saldo) : base(numero, saldo) { }

    public override void Acumular(decimal cantidad) {
        Puntos += cantidad * 0.02m;
    }
}

public class CuentaBronce : Cuenta {
    public CuentaBronce(string numero, decimal saldo) : base(numero, saldo) { }

    public override void Acumular(decimal cantidad) {
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
    public abstract string Descripcion { get; }
}

public class Deposito : Operacion {
    public Deposito(string numero, decimal monto) : base(numero, monto) { }

    public override bool Ejecutar() { 
        return Origen.Acreditar(Monto);
    }

    public override string Descripcion => $"Deposito {Monto:C0} a [{Origen.Numero}|{Origen.Cliente.Nombre}]";
}

public class Retiro : Operacion {
    public Retiro(string numero, decimal monto) : base(numero, monto) { }

    public override bool Ejecutar() { 
        return Origen.Debitar(Monto);
    }

    public override string Descripcion => $"Retiro {Monto:C0} de [{Origen.Numero}|{Origen.Cliente.Nombre}]";                       
}

public class Pago: Operacion {
    public Pago(string numero, decimal monto) : base(numero, monto) { }

    public override bool Ejecutar() { 
        if(Origen.Debitar(Monto)){
            Origen.Acreditar(Monto);
            return true;
        }
        return false;
    }

    public override string Descripcion => $"Pago {Monto:C0} de [{Origen.Numero}|{Origen.Cliente.Nombre}]";                       
}

public class Transferencia : Operacion {
    public Cuenta Destino { get; set; }

    public Transferencia(string origen, string destino, decimal monto) : base(origen, monto) { 
        Destino = Banco.Buscar(destino);
    }

    public override bool Ejecutar() { 
        if (!Origen.Debitar(Monto)) return false;
        if (!Destino.Acreditar(Monto)) {
            Origen.Acreditar(Monto);
            return false;
        }
        return true;
    }

    public override string Descripcion => $"Transfiero {Monto:C0} de [{Origen.Numero}|{Origen.Cliente.Nombre}] a [{Destino.Numero}|{Destino.Cliente.Nombre}]";
}

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