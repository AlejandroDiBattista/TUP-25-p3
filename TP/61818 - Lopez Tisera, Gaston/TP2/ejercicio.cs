using System;
using System.Collections.Generic;
using System.Linq;

class Banco {
    public string Nombre { get; private set; }
    public List<Cliente> Clientes { get; private set; } = new List<Cliente>();
    public List<Operacion> Operaciones { get; private set; } = new List<Operacion>();

    public Banco(string nombre) {
        Nombre = nombre;
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
        Console.WriteLine($"\nBanco: {Nombre} | Clientes: {Clientes.Count}");
        foreach (var cliente in Clientes) {
            cliente.Resumen();
        }
    }
}

class Cliente {
    public string Nombre { get; private set; }
    public List<Cuenta> Cuentas { get; private set; } = new List<Cuenta>();

    public Cliente(string nombre) {
        Nombre = nombre;
    }

    public void Agregar(Cuenta cuenta) {
        Cuentas.Add(cuenta);
    }

    public void Resumen() {
        Console.WriteLine($"\n  Cliente: {Nombre} | Saldo Total: $ {Cuentas.Sum(c => c.Saldo):0.00} | Puntos Total: $ {Cuentas.Sum(c => c.Creditos):0.00}");
        foreach (var cuenta in Cuentas) {
            cuenta.Resumen();
        }
    }
}
abstract class Cuenta {
    public string Numero { get; private set; }
    public decimal Saldo { get; private set; }
    public decimal Creditos { get; private set; }

    public Cuenta(string numero, decimal saldo) {
        Numero = numero;
        Saldo = saldo;
    }

    public bool Depositar(decimal monto) {
        if (monto <= 0) return false;
        Saldo += monto;
        return true;
    }

    public bool Retirar(decimal monto) {
        if (monto <= 0 || monto > Saldo) return false;
        Saldo -= monto;
        return true;
    }

    public void AcumularCreditos(decimal monto, decimal porcentaje) {
        Creditos += monto * porcentaje;
    }
    public abstract void AcumularPuntos(decimal monto);
    public void Resumen() {
        Console.WriteLine($"    Cuenta: {Numero} | Saldo: $ {Saldo:0.00} | Puntos: $ {Creditos:0.00}");
    }
}

class CuentaOro : Cuenta {
    public CuentaOro(string numero, decimal saldo) : base(numero, saldo) { }

    public override void AcumularPuntos(decimal monto) {
        AcumularCreditos(monto, monto > 1000 ? 0.05m : 0.03m);
    }
}

class CuentaPlata : Cuenta {
    public CuentaPlata(string numero, decimal saldo) : base(numero, saldo) { }

    public override void AcumularPuntos(decimal monto) {
        AcumularCreditos(monto, 0.02m);
    }
}

class CuentaBronce : Cuenta {
    public CuentaBronce(string numero, decimal saldo) : base(numero, saldo) { }

    public override void AcumularPuntos(decimal monto) {
        AcumularCreditos(monto, 0.01m);
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

class Deposito : Operacion {
    public Deposito(Cuenta origen, decimal monto) : base(origen, monto) { }

    public override bool Ejecutar() {
        return Origen.Depositar(Monto);
    }

    public override string Descripcion() {
        return $"Deposito $ {Monto:0.00} a {Origen.Numero}";
    }
}

class Retiro : Operacion {
    public Retiro(Cuenta origen, decimal monto) : base(origen, monto) { }

    public override bool Ejecutar() {
        return Origen.Retirar(Monto);
    }

    public override string Descripcion() {
        return $"Retiro $ {Monto:0.00} de {Origen.Numero}";
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

nac.Registrar(new Deposito(raul.Cuentas[0], 100));
nac.Registrar(new Retiro(raul.Cuentas[1], 200));
nac.Registrar(new Transferencia(raul.Cuentas[0], raul.Cuentas[1], 300));
nac.Registrar(new Transferencia(sara.Cuentas[0], sara.Cuentas[1], 500));
nac.Registrar(new Pago(raul.Cuentas[1], 400));

tup.Registrar(new Deposito(luis.Cuentas[0], 100));
tup.Registrar(new Retiro(luis.Cuentas[0], 200));
tup.Registrar(new Transferencia(luis.Cuentas[0], raul.Cuentas[1], 300));
tup.Registrar(new Pago(luis.Cuentas[0], 400));

nac.Informe();
tup.Informe();

