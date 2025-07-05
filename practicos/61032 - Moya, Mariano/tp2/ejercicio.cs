using System;
using System.Collections.Generic;

class Banco {
    private string nombre;
    private List<Cliente> clientes = new List<Cliente>();
    private List<Operacion> operaciones = new List<Operacion>();

    public Banco(string nombre) {
        this.nombre = nombre;
    }

    public void Agregar(Cliente cliente) {
        clientes.Add(cliente);
    }

    public void Registrar(Operacion operacion) {
        operaciones.Add(operacion);
        operacion.Ejecutar(clientes);
    }

    public void Informe() {
        Console.WriteLine($"Banco: {nombre} | Clientes: {clientes.Count}");
        foreach (var cliente in clientes) {
            cliente.Informe();
        }
    }
}

class Cliente {
    private string nombre;
    private List<Cuenta> cuentas = new List<Cuenta>();
    private List<Operacion> historial = new List<Operacion>();

    public Cliente(string nombre) {
        this.nombre = nombre;
    }

    public void Agregar(Cuenta cuenta) {
        cuentas.Add(cuenta);
    }

    public Cuenta ObtenerCuenta(string numero) {
        foreach (var cuenta in cuentas) {
            if (cuenta.Numero == numero) {
                return cuenta;
            }
        }
        return null;
    }

    public void RegistrarOperacion(Operacion operacion) {
        historial.Add(operacion);
    }

    public void Informe() {
        decimal saldoTotal = 0;
        decimal puntosTotal = 0;

        foreach (var cuenta in cuentas) {
            saldoTotal += cuenta.Saldo;
            puntosTotal += cuenta.Puntos;
        }

        Console.WriteLine($"  Cliente: {nombre} | Saldo Total: $ {saldoTotal:F2} | Puntos Total: {puntosTotal:F2}");
        foreach (var cuenta in cuentas) {
            cuenta.Informe();
        }
    }
}

abstract class Cuenta {
    public string Numero { get; }
    public decimal Saldo { get; protected set; }
    public decimal Puntos { get; protected set; }

    public Cuenta(string numero, decimal saldoInicial) {
        Numero = numero;
        Saldo = saldoInicial;
    }

    public abstract void AcumularPuntos(decimal monto);

    public void IncrementarSaldo(decimal monto) {
        Saldo += monto;
    }

    public bool DecrementarSaldo(decimal monto) {
        if (Saldo >= monto) {
            Saldo -= monto;
            return true;
        }
        return false;
    }

    public void Informe() {
        Console.WriteLine($"    Cuenta: {Numero} | Saldo: $ {Saldo:F2} | Puntos: {Puntos:F2}");
    }
}

class CuentaOro : Cuenta {
    public CuentaOro(string numero, decimal saldoInicial) : base(numero, saldoInicial) { }

    public override void AcumularPuntos(decimal monto) {
        Puntos += monto > 1000 ? monto * 0.05m : monto * 0.03m;
    }
}

class CuentaPlata : Cuenta {
    public CuentaPlata(string numero, decimal saldoInicial) : base(numero, saldoInicial) { }

    public override void AcumularPuntos(decimal monto) {
        Puntos += monto * 0.02m;
    }
}

class CuentaBronce : Cuenta {
    public CuentaBronce(string numero, decimal saldoInicial) : base(numero, saldoInicial) { }

    public override void AcumularPuntos(decimal monto) {
        Puntos += monto * 0.01m;
    }
}

abstract class Operacion {
    public abstract void Ejecutar(List<Cliente> clientes);
}

class Deposito : Operacion {
    private string cuentaDestino;
    private decimal monto;

    public Deposito(string cuentaDestino, decimal monto) {
        this.cuentaDestino = cuentaDestino;
        this.monto = monto;
    }

    public override void Ejecutar(List<Cliente> clientes) {
        foreach (var cliente in clientes) {
            var cuenta = cliente.ObtenerCuenta(cuentaDestino);
            if (cuenta != null) {
                cuenta.IncrementarSaldo(monto);
                return;
            }
        }
    }
}

class Retiro : Operacion {
    private string cuentaOrigen;
    private decimal monto;

    public Retiro(string cuentaOrigen, decimal monto) {
        this.cuentaOrigen = cuentaOrigen;
        this.monto = monto;
    }

    public override void Ejecutar(List<Cliente> clientes) {
        foreach (var cliente in clientes) {
            var cuenta = cliente.ObtenerCuenta(cuentaOrigen);
            if (cuenta != null) {
                cuenta.DecrementarSaldo(monto);
                return;
            }
        }
    }
}

class Transferencia : Operacion {
    private string cuentaOrigen;
    private string cuentaDestino;
    private decimal monto;

    public Transferencia(string cuentaOrigen, string cuentaDestino, decimal monto) {
        this.cuentaOrigen = cuentaOrigen;
        this.cuentaDestino = cuentaDestino;
        this.monto = monto;
    }

    public override void Ejecutar(List<Cliente> clientes) {
        Cuenta origen = null, destino = null;

        foreach (var cliente in clientes) {
            if (origen == null) {
                origen = cliente.ObtenerCuenta(cuentaOrigen);
            }
            if (destino == null) {
                destino = cliente.ObtenerCuenta(cuentaDestino);
            }
            if (origen != null && destino != null) {
                break;
            }
        }

        if (origen != null && destino != null && origen.DecrementarSaldo(monto)) {
            destino.IncrementarSaldo(monto);
        }
    }
}

class Pago : Operacion {
    private string cuentaOrigen;
    private decimal monto;

    public Pago(string cuentaOrigen, decimal monto) {
        this.cuentaOrigen = cuentaOrigen;
        this.monto = monto;
    }

    public override void Ejecutar(List<Cliente> clientes) {
        foreach (var cliente in clientes) {
            var cuenta = cliente.ObtenerCuenta(cuentaOrigen);
            if (cuenta != null && cuenta.DecrementarSaldo(monto)) {
                cuenta.AcumularPuntos(monto);
                return;
            }
        }
    }
}

/// EJEMPLO DE USO ///
/// 
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

nac.Registrar(new Deposito("10001", 100));
nac.Registrar(new Retiro("10002", 200));
nac.Registrar(new Transferencia("10001", "10002", 300));
nac.Registrar(new Transferencia("10003", "10004", 500));
nac.Registrar(new Pago("10002", 400));

tup.Registrar(new Deposito("10005", 100));
tup.Registrar(new Retiro("10005", 200));
tup.Registrar(new Transferencia("10005", "10002", 300));
tup.Registrar(new Pago("10005", 400));

nac.Informe();
tup.Informe();

