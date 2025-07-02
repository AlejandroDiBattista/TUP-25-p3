// TP2: Sistema de Cuentas Bancarias
//

// Implementar un sistema de cuentas bancarias que permita realizar operaciones como depósitos, retiros, transferencias y pagos.

using static System.Console;
using System.Collections.Generic;
using System.Linq;

class Banco {
    public string Nombre { get; private set; }
    private List<Cliente> clientes = new List<Cliente>();
    private List<Operacion> operaciones = new List<Operacion>();

    public Banco(string nombre) {
        Nombre = nombre;
    }

    public void Agregar(Cliente cliente) {
        clientes.Add(cliente);
    }

    public void Registrar(Operacion operacion) {
        var cuentaOrigen = clientes.SelectMany(c => c.Cuentas).FirstOrDefault(c => c.Numero == operacion.CuentaOrigen);
        if (cuentaOrigen == null) {
            WriteLine($"Error: Cuenta {operacion.CuentaOrigen} no encontrada.");
            return;
        }

        if (operacion is Transferencia transferencia) {
            var cuentaDestino = clientes.SelectMany(c => c.Cuentas).FirstOrDefault(c => c.Numero == transferencia.CuentaDestino);
            if (cuentaDestino == null) {
                WriteLine($"Error: Cuenta destino {transferencia.CuentaDestino} no encontrada.");
                return;
            }
            transferencia.Ejecutar(cuentaOrigen, cuentaDestino);
        } else {
            operacion.Ejecutar(cuentaOrigen);
        }

        operaciones.Add(operacion);
        cuentaOrigen.Cliente.AgregarOperacion(operacion);
    }

    public void Informe() {
        WriteLine($"\nBanco: {Nombre} | Clientes: {clientes.Count}\n");
        foreach (var cliente in clientes) {
            cliente.Informe();
        }
    }
}

class Cliente {
    public string Nombre { get; private set; }
    public List<Cuenta> Cuentas { get; private set; } = new List<Cuenta>();
    private List<Operacion> operaciones = new List<Operacion>();

    public Cliente(string nombre) {
        Nombre = nombre;
    }

    public void Agregar(Cuenta cuenta) {
        cuenta.AsignarCliente(this);
        Cuentas.Add(cuenta);
    }

    public void AgregarOperacion(Operacion operacion) {
        operaciones.Add(operacion);
    }

    public void Informe() {
        decimal saldoTotal = Cuentas.Sum(c => c.Saldo);
        decimal puntosTotal = Cuentas.Sum(c => c.Puntos);
        WriteLine($"  Cliente: {Nombre} | Saldo Total: $ {saldoTotal:F2} | Puntos Total: $ {puntosTotal:F2}\n");
        foreach (var cuenta in Cuentas) {
            WriteLine($"    Cuenta: {cuenta.Numero} | Saldo: $ {cuenta.Saldo:F2} | Puntos: $ {cuenta.Puntos:F2}");
            foreach (var op in operaciones) {
                if (op.InvolucraCuenta(cuenta.Numero)) {
                    WriteLine($"     -  {op.Descripcion(this)}");
                }
            }
            WriteLine();
        }
    }
}

abstract class Cuenta {
    public string Numero { get; private set; }
    public decimal Saldo { get; private set; }
    public decimal Puntos { get; protected set; }
    public Cliente Cliente { get; private set; }

    public Cuenta(string numero, decimal saldo) {
        Numero = numero;
        Saldo = saldo;
    }

    public void AsignarCliente(Cliente cliente) {
        Cliente = cliente;
    }

    public void Acreditar(decimal monto) {
        Saldo += monto;
    }

    public bool Debitar(decimal monto) {
        if (Saldo >= monto) {
            Saldo -= monto;
            return true;
        }
        return false;
    }

    public abstract void AplicarPuntos(decimal monto);
}

class CuentaOro : Cuenta {
    public CuentaOro(string numero, decimal saldo) : base(numero, saldo) { }

    public override void AplicarPuntos(decimal monto) {
        Puntos += monto >= 1000 ? monto * 0.05m : monto * 0.03m;
    }
}

class CuentaPlata : Cuenta {
    public CuentaPlata(string numero, decimal saldo) : base(numero, saldo) { }

    public override void AplicarPuntos(decimal monto) {
        Puntos += monto * 0.02m;
    }
}

class CuentaBronce : Cuenta {
    public CuentaBronce(string numero, decimal saldo) : base(numero, saldo) { }

    public override void AplicarPuntos(decimal monto) {
        Puntos += monto * 0.01m;
    }
}

abstract class Operacion {
    public string CuentaOrigen { get; protected set; }
    public decimal Monto { get; protected set; }

    public Operacion(string cuenta, decimal monto) {
        CuentaOrigen = cuenta;
        Monto = monto;
    }

    public abstract void Ejecutar(Cuenta cuenta);
    public virtual string Descripcion(Cliente cliente) {
        return $"{GetType().Name} $ {Monto:F2} de [{CuentaOrigen}/{cliente.Nombre}]";
    }

    public virtual bool InvolucraCuenta(string numero) {
        return CuentaOrigen == numero;
    }
}

class Deposito : Operacion {
    public Deposito(string cuenta, decimal monto) : base(cuenta, monto) { }

    public override void Ejecutar(Cuenta cuenta) {
        cuenta.Acreditar(Monto);
    }

    public override string Descripcion(Cliente cliente) {
        return $"Deposito $ {Monto:F2} a [{CuentaOrigen}/{cliente.Nombre}]";
    }
}

class Retiro : Operacion {
    public Retiro(string cuenta, decimal monto) : base(cuenta, monto) { }

    public override void Ejecutar(Cuenta cuenta) {
        if (!cuenta.Debitar(Monto)) {
            WriteLine("Fondos insuficientes para retiro.");
        }
    }

    public override string Descripcion(Cliente cliente) {
        return $"Retiro $ {Monto:F2} de [{CuentaOrigen}/{cliente.Nombre}]";
    }
}

class Pago : Operacion {
    public Pago(string cuenta, decimal monto) : base(cuenta, monto) { }

    public override void Ejecutar(Cuenta cuenta) {
        if (cuenta.Debitar(Monto)) {
            cuenta.AplicarPuntos(Monto);
        } else {
            WriteLine("Fondos insuficientes para pago.");
        }
    }

    public override string Descripcion(Cliente cliente) {
        return $"Pago $ {Monto:F2} con [{CuentaOrigen}/{cliente.Nombre}]";
    }
}

class Transferencia : Operacion {
    public string CuentaDestino { get; private set; }

    public Transferencia(string cuentaOrigen, string cuentaDestino, decimal monto)
        : base(cuentaOrigen, monto) {
        CuentaDestino = cuentaDestino;
    }

    public override void Ejecutar(Cuenta origen) {
        throw new NotImplementedException();
    }

    public void Ejecutar(Cuenta origen, Cuenta destino) {
        if (origen.Debitar(Monto)) {
            destino.Acreditar(Monto);
        } else {
            WriteLine("Fondos insuficientes para transferencia.");
        }
    }

    public override string Descripcion(Cliente cliente) {
        return $"Transferencia $ {Monto:F2} de [{CuentaOrigen}/{cliente.Nombre}] a [{CuentaDestino}]";
    }

    public override bool InvolucraCuenta(string numero) {
        return CuentaOrigen == numero || CuentaDestino == numero;
    }
}

///

// EJEMPLO DE USO ///

class Program {
    static void Main() {
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
