// TP2: Sistema de Cuentas Bancarias
//
using System;
using System.Collections.Generic;
using System.Linq;

// === Abstracta Operacion ===
abstract class Operacion {
    public decimal Monto { get; protected set; }
    public string CuentaOrigen { get; protected set; }
    public string CuentaDestino { get; protected set; }

    public Operacion(decimal monto, string cuentaOrigen, string cuentaDestino = null) {
        Monto = monto;
        CuentaOrigen = cuentaOrigen;
        CuentaDestino = cuentaDestino;
    }

    public abstract void Ejecutar(Banco banco);
    public abstract string Descripcion(Banco banco);
}

class Deposito : Operacion {
    public Deposito(string cuentaDestino, decimal monto) : base(monto, null, cuentaDestino) { }

    public override void Ejecutar(Banco banco) {
        var cuenta = Banco.BuscarCuentaGlobal(CuentaDestino);
        cuenta.Depositar(Monto);
        cuenta.Cliente.Registrar(this);
        banco.RegistrarOperacion(this);
    }

    public override string Descripcion(Banco banco) {
        var cuenta = Banco.BuscarCuentaGlobal(CuentaDestino);
        return $"-  Deposito $ {Monto:0.00} a [{CuentaDestino}/{cuenta.Cliente.Nombre}]";
    }
}

class Retiro : Operacion {
    public Retiro(string cuentaOrigen, decimal monto) : base(monto, cuentaOrigen) { }

    public override void Ejecutar(Banco banco) {
        var cuenta = banco.BuscarCuenta(CuentaOrigen);
        cuenta.Extraer(Monto);
        cuenta.Cliente.Registrar(this);
        banco.RegistrarOperacion(this);
    }

    public override string Descripcion(Banco banco) {
        var cuenta = Banco.BuscarCuentaGlobal(CuentaOrigen);
        return $"-  Retiro $ {Monto:0.00} de [{CuentaOrigen}/{cuenta.Cliente.Nombre}]";
    }
}

class Pago : Operacion {
    public Pago(string cuentaOrigen, decimal monto) : base(monto, cuentaOrigen) { }

    public override void Ejecutar(Banco banco) {
        var cuenta = banco.BuscarCuenta(CuentaOrigen);
        cuenta.Pagar(Monto);
        cuenta.Cliente.Registrar(this);
        banco.RegistrarOperacion(this);
    }

    public override string Descripcion(Banco banco) {
        var cuenta = Banco.BuscarCuentaGlobal(CuentaOrigen);
        return $"-  Pago $ {Monto:0.00} con [{CuentaOrigen}/{cuenta.Cliente.Nombre}]";
    }
}

class Transferencia : Operacion {
    public Transferencia(string cuentaOrigen, string cuentaDestino, decimal monto)
        : base(monto, cuentaOrigen, cuentaDestino) { }

    public override void Ejecutar(Banco banco) {
        var origen = banco.BuscarCuenta(CuentaOrigen);
        var destino = Banco.BuscarCuentaGlobal(CuentaDestino);

        origen.Extraer(Monto);
        destino.Depositar(Monto);

        origen.Cliente.Registrar(this);
        destino.Cliente.Registrar(this);
        banco.RegistrarOperacion(this);
    }

    public override string Descripcion(Banco banco) {
        var origen = Banco.BuscarCuentaGlobal(CuentaOrigen);
        var destino = Banco.BuscarCuentaGlobal(CuentaDestino);
        return $"-  Transferencia $ {Monto:0.00} de [{CuentaOrigen}/{origen.Cliente.Nombre}] a [{CuentaDestino}/{destino.Cliente.Nombre}]";
    }
}

// === Abstracta Cuenta ===
abstract class Cuenta {
    public string Numero { get; private set; }
    public decimal Saldo { get; protected set; }
    public decimal Puntos { get; protected set; }
    public Cliente Cliente { get; set; }

    protected Cuenta(string numero, decimal saldoInicial) {
        Numero = numero;
        Saldo = saldoInicial;
        Puntos = 0;
    }

    public virtual void Depositar(decimal monto) => Saldo += monto;

    public virtual void Extraer(decimal monto) {
        if (Saldo < monto)
            throw new InvalidOperationException("Fondos insuficientes.");
        Saldo -= monto;
    }

    public virtual void Pagar(decimal monto) {
        Extraer(monto);
        Puntos += CalcularPuntos(monto);
    }

    public abstract decimal CalcularPuntos(decimal monto);
}

class CuentaOro : Cuenta {
    public CuentaOro(string numero, decimal saldo) : base(numero, saldo) { }

    public override decimal CalcularPuntos(decimal monto) =>
        monto > 1000 ? monto * 0.05m : monto * 0.03m;
}

class CuentaPlata : Cuenta {
    public CuentaPlata(string numero, decimal saldo) : base(numero, saldo) { }

    public override decimal CalcularPuntos(decimal monto) => monto * 0.02m;
}

class CuentaBronce : Cuenta {
    public CuentaBronce(string numero, decimal saldo) : base(numero, saldo) { }

    public override decimal CalcularPuntos(decimal monto) => monto * 0.01m;
}

class Cliente {
    public string Nombre { get; private set; }
    public List<Cuenta> Cuentas { get; private set; } = new();
    public List<Operacion> Historial { get; private set; } = new();

    public Cliente(string nombre) => Nombre = nombre;

    public void Agregar(Cuenta cuenta) {
        cuenta.Cliente = this;
        Cuentas.Add(cuenta);
    }

    public void Registrar(Operacion op) => Historial.Add(op);

    public decimal SaldoTotal => Cuentas.Sum(c => c.Saldo);
    public decimal PuntosTotal => Cuentas.Sum(c => c.Puntos);
}

class Banco {
    public string Nombre { get; private set; }
    public List<Cliente> Clientes { get; private set; } = new();
    public List<Operacion> HistorialGlobal { get; private set; } = new();
    public static List<Banco> Bancos = new();

    public Banco(string nombre) {
        Nombre = nombre;
        Bancos.Add(this);
    }

    public void Agregar(Cliente cliente) => Clientes.Add(cliente);

    public void Registrar(Operacion op) => op.Ejecutar(this);

    public void RegistrarOperacion(Operacion op) => HistorialGlobal.Add(op);

    public Cuenta BuscarCuenta(string numero) {
        foreach (var cliente in Clientes) {
            var cuenta = cliente.Cuentas.FirstOrDefault(c => c.Numero == numero);
            if (cuenta != null) return cuenta;
        }
        throw new InvalidOperationException($"Cuenta {numero} no encontrada en banco {Nombre}");
    }

    public static Cuenta BuscarCuentaGlobal(string numero) {
        foreach (var banco in Bancos) {
            try {
                return banco.BuscarCuenta(numero);
            } catch { }
        }
        throw new InvalidOperationException($"Cuenta {numero} no encontrada en ningún banco.");
    }

    public void Informe() {
        Console.WriteLine($"\nBanco: {Nombre} | Clientes: {Clientes.Count}\n");

        foreach (var cliente in Clientes) {
            Console.WriteLine($"  Cliente: {cliente.Nombre} | Saldo Total: $ {cliente.SaldoTotal:0.00} | Puntos Total: $ {cliente.PuntosTotal:0.00}\n");

// Implementar un sistema de cuentas bancarias que permita realizar operaciones como depósitos, retiros, transferencias y pagos.
            foreach (var cuenta in cliente.Cuentas) {
                Console.WriteLine($"    Cuenta: {cuenta.Numero} | Saldo: $ {cuenta.Saldo:0.00} | Puntos: $ {cuenta.Puntos:0.00}");
                foreach (var op in cliente.Historial.Where(o =>
                    o.CuentaOrigen == cuenta.Numero || o.CuentaDestino == cuenta.Numero)) {
                    Console.WriteLine("     " + op.Descripcion(this));
                }
                Console.WriteLine();
            }
        }
    }
}

class Banco{}
class Cliente{}

abstract class Cuenta{}
class CuentaOro: Cuenta{}
class CuentaPlata: Cuenta{}
class CuentaBronce: Cuenta{}

abstract class Operacion{}
class Deposito: Operacion{}
class Retiro: Operacion{}
class Transferencia: Operacion{}
class Pago: Operacion{}


/// EJEMPLO DE USO ///

// Definiciones 

var raul = new Cliente("Raul Perez");
    raul.Agregar(new CuentaOro("10001", 1000));
    raul.Agregar(new CuentaPlata("10002", 2000));
raul.Agregar(new CuentaOro("10001", 1000));
raul.Agregar(new CuentaPlata("10002", 2000));

var sara = new Cliente("Sara Lopez");
    sara.Agregar(new CuentaPlata("10003", 3000));
    sara.Agregar(new CuentaPlata("10004", 4000));
sara.Agregar(new CuentaPlata("10003", 3000));
sara.Agregar(new CuentaPlata("10004", 4000));

var luis = new Cliente("Luis Gomez");
    luis.Agregar(new CuentaBronce("10005", 5000));
luis.Agregar(new CuentaBronce("10005", 5000));

var nac = new Banco("Banco Nac");
nac.Agregar(raul);
nac.Agregar(sara);

var tup = new Banco("Banco TUP");
tup.Agregar(luis);


// Registrar Operaciones
// Operaciones en Banco Nac
nac.Registrar(new Deposito("10001", 100));
nac.Registrar(new Retiro("10002", 200));
nac.Registrar(new Transferencia("10001", "10002", 300));
nac.Registrar(new Transferencia("10003", "10004", 500));
nac.Registrar(new Pago("10002", 400));

// Operaciones en Banco TUP
tup.Registrar(new Deposito("10005", 100));
tup.Registrar(new Retiro("10005", 200));
tup.Registrar(new Transferencia("10005", "10002", 300));
tup.Registrar(new Pago("10005", 400));


// Informe final
nac.Informe();
tup.Informe();

