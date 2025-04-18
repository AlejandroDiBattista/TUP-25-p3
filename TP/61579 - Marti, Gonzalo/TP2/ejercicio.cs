using System;
using System.Collections.Generic;
using System.Linq;

class Banco {
    public string Nombre { get; }
    private List<Cliente> clientes = new();
    private List<Operacion> operaciones = new();

    public Banco(string nombre) {
        Nombre = nombre;
    }

    public void Agregar(Cliente cliente) {
        clientes.Add(cliente);
    }

    public void Registrar(Operacion op) {
        Cuenta origen = BuscarCuenta(op.Origen);
        if (origen == null)
            return;

        if (!clientes.Any(c => c.TieneCuenta(origen.Numero)))
            return; // Verifica que la cuenta origen sea del banco

        Cuenta destino = op is Transferencia t ? BuscarCuenta(t.Destino) : null;
        if (!op.Ejecutar(origen, destino)) return;

        operaciones.Add(op);
        Cliente clienteOrigen = clientes.FirstOrDefault(c => c.TieneCuenta(origen.Numero));
        clienteOrigen?.AgregarOperacion(op);
        if (destino != null) {
            Cliente clienteDestino = clientes.FirstOrDefault(c => c.TieneCuenta(destino.Numero));
            clienteDestino?.AgregarOperacion(op);
        }
    }

    private Cuenta BuscarCuenta(string numero) {
        return clientes.SelectMany(c => c.Cuentas).FirstOrDefault(cta => cta.Numero == numero);
    }

    public void Informe() {
        Console.WriteLine($"\nBanco: {Nombre} | Clientes: {clientes.Count}");

        foreach (var cliente in clientes) {
            Console.WriteLine($"\n  Cliente: {cliente.Nombre} | Saldo Total: $ {cliente.TotalSaldo():N2} | Puntos Total: $ {cliente.TotalPuntos():N2}");

            foreach (var cuenta in cliente.Cuentas) {
                Console.WriteLine($"\n    Cuenta: {cuenta.Numero} | Saldo: $ {cuenta.Saldo:N2} | Puntos: $ {cuenta.Puntos:N2}");
                foreach (var op in cliente.Operaciones.Where(o => o.InvolucraCuenta(cuenta.Numero))) {
                    Console.WriteLine($"     -  {op}");
                }
            }
        }
    }
}

class Cliente {
    public string Nombre { get; }
    public List<Cuenta> Cuentas { get; } = new();
    public List<Operacion> Operaciones { get; } = new();

    public Cliente(string nombre) {
        Nombre = nombre;
    }

    public void Agregar(Cuenta cuenta) {
        Cuentas.Add(cuenta);
    }

    public bool TieneCuenta(string numero) {
        return Cuentas.Any(c => c.Numero == numero);
    }

    public void AgregarOperacion(Operacion op) {
        Operaciones.Add(op);
    }

    public decimal TotalSaldo() => Cuentas.Sum(c => c.Saldo);
    public decimal TotalPuntos() => Cuentas.Sum(c => c.Puntos);
}

abstract class Cuenta {
    public string Numero { get; }
    public decimal Saldo { get; protected set; }
    public decimal Puntos { get; protected set; }

    protected Cuenta(string numero, decimal saldo) {
        Numero = numero;
        Saldo = saldo;
        Puntos = 0;
    }

    public virtual void Depositar(decimal monto) {
        Saldo += monto;
    }

    public virtual bool Extraer(decimal monto) {
        if (Saldo >= monto) {
            Saldo -= monto;
            return true;
        }
        return false;
    }

    public abstract void Pagar(decimal monto);
}

class CuentaOro : Cuenta {
    public CuentaOro(string numero, decimal saldo) : base(numero, saldo) { }

    public override void Pagar(decimal monto) {
        if (Extraer(monto)) {
            Puntos += monto >= 1000 ? monto * 0.05m : monto * 0.03m;
        }
    }
}

class CuentaPlata : Cuenta {
    public CuentaPlata(string numero, decimal saldo) : base(numero, saldo) { }

    public override void Pagar(decimal monto) {
        if (Extraer(monto)) {
            Puntos += monto * 0.02m;
        }
    }
}

class CuentaBronce : Cuenta {
    public CuentaBronce(string numero, decimal saldo) : base(numero, saldo) { }

    public override void Pagar(decimal monto) {
        if (Extraer(monto)) {
            Puntos += monto * 0.01m;
        }
    }
}

abstract class Operacion {
    public string Origen { get; }
    public decimal Monto { get; }

    protected Operacion(string origen, decimal monto) {
        Origen = origen;
        Monto = monto;
    }

    public abstract bool Ejecutar(Cuenta origen, Cuenta destino);
    public abstract override string ToString();
    public virtual bool InvolucraCuenta(string numero) => Origen == numero;
}

class Deposito : Operacion {
    public Deposito(string origen, decimal monto) : base(origen, monto) { }

    public override bool Ejecutar(Cuenta origen, Cuenta destino) {
        origen.Depositar(Monto);
        return true;
    }

    public override string ToString() => $"Deposito $ {Monto:N2} a [{Origen}]";
}

class Retiro : Operacion {
    public Retiro(string origen, decimal monto) : base(origen, monto) { }

    public override bool Ejecutar(Cuenta origen, Cuenta destino) {
        return origen.Extraer(Monto);
    }

    public override string ToString() => $"Retiro $ {Monto:N2} de [{Origen}]";
}

class Pago : Operacion {
    public Pago(string origen, decimal monto) : base(origen, monto) { }

    public override bool Ejecutar(Cuenta origen, Cuenta destino) {
        origen.Pagar(Monto);
        return true;
    }

    public override string ToString() => $"Pago $ {Monto:N2} con [{Origen}]";
}

class Transferencia : Operacion {
    public string Destino { get; }

    public Transferencia(string origen, string destino, decimal monto) : base(origen, monto) {
        Destino = destino;
    }

    public override bool Ejecutar(Cuenta origen, Cuenta destino) {
        if (origen.Extraer(Monto)) {
            destino.Depositar(Monto);
            return true;
        }
        return false;
    }

    public override bool InvolucraCuenta(string numero) => base.InvolucraCuenta(numero) || Destino == numero;

    public override string ToString() => $"Transferencia $ {Monto:N2} de [{Origen}] a [{Destino}]";
}