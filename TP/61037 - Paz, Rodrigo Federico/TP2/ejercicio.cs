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
        Console.WriteLine($"\nBanco: {Nombre} | Total de Clientes: {Clientes.Count}");
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
        Console.WriteLine($"\n  Cliente: {Nombre} | Saldo Total: $ {Cuentas.Sum(c => c.Saldo):0.00} | Puntos: $ {Cuentas.Sum(c => c.Creditos):0.00}");
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
        return $"Depósito de $ {Monto:0.00} en cuenta {Origen.Numero}";
    }
}

class Retiro : Operacion {
    public Retiro(Cuenta origen, decimal monto) : base(origen, monto) { }

    public override bool Ejecutar() {
        return Origen.Retirar(Monto);
    }

    public override string Descripcion() {
        return $"Retiro de $ {Monto:0.00} de cuenta {Origen.Numero}";
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
        return $"Pago de $ {Monto:0.00} desde cuenta {Origen.Numero}";
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
        return $"Transferencia de $ {Monto:0.00} de {Origen.Numero} a {Destino.Numero}";
    }
}

class Program {
    static void Main(string[] args) {
        var andrea = new Cliente("Andrea Torres");
        andrea.Agregar(new CuentaOro("A101", 1500));
        andrea.Agregar(new CuentaPlata("A102", 1800));

        var mario = new Cliente("Mario Díaz");
        mario.Agregar(new CuentaPlata("M201", 3200));
        mario.Agregar(new CuentaPlata("M202", 2500));

        var claudia = new Cliente("Claudia Ríos");
        claudia.Agregar(new CuentaBronce("C301", 4200));

        var bancoUno = new Banco("Banco Patria");
        bancoUno.Agregar(andrea);
        bancoUno.Agregar(mario);

        var bancoDos = new Banco("Banco Regional");
        bancoDos.Agregar(claudia);

        bancoUno.Registrar(new Deposito(andrea.Cuentas[0], 150));
        bancoUno.Registrar(new Retiro(andrea.Cuentas[1], 250));
        bancoUno.Registrar(new Transferencia(andrea.Cuentas[0], andrea.Cuentas[1], 400));
        bancoUno.Registrar(new Transferencia(mario.Cuentas[0], mario.Cuentas[1], 350));
        bancoUno.Registrar(new Pago(andrea.Cuentas[1], 300));

        bancoDos.Registrar(new Deposito(claudia.Cuentas[0], 200));
        bancoDos.Registrar(new Retiro(claudia.Cuentas[0], 150));
        bancoDos.Registrar(new Transferencia(claudia.Cuentas[0], andrea.Cuentas[1], 250));
        bancoDos.Registrar(new Pago(claudia.Cuentas[0], 500));

        bancoUno.Informe();
        bancoDos.Informe();
    }
}
