using System;
using System.Collections.Generic;//esta libreria es necesaria para usar List<T>
using System.Globalization;//se usa para formatear los montos en dinero con separadores y decimales correctos.
using System.Linq;//se usa para usar la función Sum que suma los saldos y puntos de todas las cuentas del cliente. se esta usando en la clase Cliente. lin 52 y 53.


class Banco {
    public string Nombre { get; private set; }
    public List<Cliente> Clientes { get; private set; }
    public List<Operacion> Operaciones { get; private set; }
    public static Dictionary<string, Cuenta> Cuentas = new Dictionary<string, Cuenta>();//diccionario para almacenar las cuentas de los clientes, donde la clave es el número de cuenta y el valor es la cuenta en sí.

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
        }
    }

    public void Informe() {
        Console.WriteLine($"Banco: {Nombre} | Clientes: {Clientes.Count}\n");
        foreach (var cliente in Clientes) {
            cliente.MostrarResumen();
        }
    }
}

class Cliente {
    public string Nombre { get; private set; }
    public List<Cuenta> Cuentas { get; private set; }

    public Cliente(string nombre) {
        Nombre = nombre;
        Cuentas = new List<Cuenta>();
    }

    public void Agregar(Cuenta cuenta) {
        Cuentas.Add(cuenta);
        Banco.Cuentas[cuenta.Numero] = cuenta;
    }

    public void MostrarResumen() {
        decimal saldoTotal = Cuentas.Sum(c => c.Saldo);
        decimal puntosTotal = Cuentas.Sum(c => c.Puntos);//se usa la función Sum para sumar los saldos y puntos de todas las cuentas del cliente.
        Console.WriteLine($"  Cliente: {Nombre} | Saldo Total: $ {saldoTotal:F2} | Puntos Total: $ {puntosTotal:F2}\n");//f2 es para formatear el número a dos decimales.
        foreach (var cuenta in Cuentas) {
            cuenta.MostrarDetalle();
        }
    }
}

abstract class Cuenta {
    public string Numero { get; private set; }
    public decimal Saldo { get; protected set; }
    public decimal Puntos { get; protected set; }

    public Cuenta(string numero, decimal saldo) {
        Numero = numero;
        Saldo = saldo;
        Puntos = 0;
    }

    public bool Depositar(decimal cantidad) {
        if (cantidad <= 0) return false;
        Saldo += cantidad;
        return true;
    }

    public bool Extraer(decimal cantidad) {
        if (cantidad <= 0 || cantidad > Saldo) return false;
        Saldo -= cantidad;
        return true;
    }

    public abstract void AcumularPuntos(decimal monto);//método abstracto que se implementará en las clases derivadas para acumular puntos según el tipo de cuenta.

    public void MostrarDetalle() {
        Console.WriteLine($"    Cuenta: {Numero} | Saldo: $ {Saldo:F2} | Puntos: $ {Puntos:F2}");// Formato de salida para mostrar el saldo y los puntos de la cuenta.
    }//método que muestra el detalle de la cuenta, incluyendo el número de cuenta, saldo y puntos acumulados.
}

class CuentaOro : Cuenta {
    public CuentaOro(string numero, decimal saldo) : base(numero, saldo) { }// constructor que inicializa el número de cuenta y el saldo inicial.
    public override void AcumularPuntos(decimal monto) {
        Puntos += monto >= 1000 ? monto * 0.05m : monto * 0.03m;// Acumula puntos según el monto gastado, con un bonus si es mayor a 1000.
    }
}

class CuentaPlata : Cuenta {
    public CuentaPlata(string numero, decimal saldo) : base(numero, saldo) { }
    public override void AcumularPuntos(decimal monto) {
        Puntos += monto * 0.02m;// Acumula puntos según el monto gastado.
    }
}

class CuentaBronce : Cuenta {
    public CuentaBronce(string numero, decimal saldo) : base(numero, saldo) { }
    public override void AcumularPuntos(decimal monto) {
        Puntos += monto * 0.01m;
    }
}

abstract class Operacion {
    protected Cuenta Origen;// Cuenta de origen para la operación.
    public decimal Monto { get; private set; }

    public Operacion(string numero, decimal monto) {
        Origen = Banco.Cuentas.ContainsKey(numero) ? Banco.Cuentas[numero] : null;
        Monto = monto;
    }

    public abstract bool Ejecutar();
    public abstract string Descripcion { get; }
}

class Deposito : Operacion {
    public Deposito(string numero, decimal monto) : base(numero, monto) { }
    public override bool Ejecutar() => Origen != null && Origen.Depositar(Monto);
    public override string Descripcion => $"     - Deposito $ {Monto:F2} a [{Origen.Numero}]";
}

class Retiro : Operacion {
    public Retiro(string numero, decimal monto) : base(numero, monto) { }
    public override bool Ejecutar() => Origen != null && Origen.Extraer(Monto);
    public override string Descripcion => $"     - Retiro $ {Monto:F2} de [{Origen.Numero}]";
}

class Pago : Operacion {
    public Pago(string numero, decimal monto) : base(numero, monto) { }
    public override bool Ejecutar() {
        if (Origen != null && Origen.Extraer(Monto)) {
            Origen.AcumularPuntos(Monto);
            return true;
        }
        return false;
    }
    public override string Descripcion => $"     - Pago $ {Monto:F2} con [{Origen.Numero}]";//origen es la cuenta de origen, que se obtiene del diccionario de cuentas del banco.
    // Descripción del pago
}

class Transferencia : Operacion {
    private Cuenta Destino;// Cuenta de destino para la transferencia.

    public Transferencia(string origen, string destino, decimal monto) : base(origen, monto) {
        Destino = Banco.Cuentas.ContainsKey(destino) ? Banco.Cuentas[destino] : null;
    }

    public override bool Ejecutar() {
        if (Origen == null || Destino == null || !Origen.Extraer(Monto)) return false;// Verifica si la cuenta de origen y destino son válidas y si hay suficiente saldo
        Destino.Depositar(Monto);
        return true;
    }
    public override string Descripcion => $"     - Transferencia $ {Monto:F2} de [{Origen.Numero}] a [{Destino.Numero}]";// Descripción de la transferencia 
}

// Prueba del sistema
var nac = new Banco("Banco Nac");// Crear una instancia del banco
// Crear un cliente y agregar cuentas
var Manuel = new Cliente("Manuel Perez");
Manuel.Agregar(new CuentaOro("10001", 1000));
Manuel.Agregar(new CuentaPlata("10002", 2000));
nac.Agregar(Manuel);
// Registrar operaciones
nac.Registrar(new Deposito("10001", 100));
nac.Registrar(new Retiro("10002", 200));
nac.Registrar(new Transferencia("10001", "10002", 300));
nac.Registrar(new Pago("10002", 1000));

// Crear clientes y cuentas
var raul = new Cliente("Raul Perez");
    raul.Agregar(new CuentaOro("10001", 1000));
    raul.Agregar(new CuentaPlata("10002", 2000));

var sara = new Cliente("Sara Lopez");
    sara.Agregar(new CuentaPlata("10003", 3000));
    sara.Agregar(new CuentaPlata("10004", 4000));

var luis = new Cliente("Luis Gomez");
    luis.Agregar(new CuentaBronce("10005", 5000));

// Agregar clientes al banco
nac.Agregar(raul);
nac.Agregar(sara);

var tup = new Banco("Banco TUP");// Crear una instancia del banco TUP
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
