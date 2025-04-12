using System;
using System.Collections.Generic;
using System.Linq;

abstract class Cuenta {
    public string Numero { get; private set; }
    public decimal Saldo { get; protected set; }
    public decimal Puntos { get; protected set; }
    public Cliente Titular { get; set; }
    public List<Operacion> Operaciones { get; } = new();

    public Cuenta(string numero, decimal saldo) {
        Numero = numero;
        Saldo = saldo;
        Puntos = 0;
    }

    public void Depositar(decimal monto) => Saldo += monto;

    public bool Extraer(decimal monto) {
        if (Saldo >= monto) {
            Saldo -= monto;
            return true;
        }
        return false;
    }

    public void RegistrarOperacion(Operacion op) => Operaciones.Add(op);

    public abstract void Pagar(decimal monto);
}

class CuentaOro : Cuenta {
    public CuentaOro(string numero, decimal saldo) : base(numero, saldo) {}

    public override void Pagar(decimal monto) {
        if (Extraer(monto)) {
            Puntos += monto >= 1000 ? monto * 0.05m : monto * 0.03m;
        }
    }
}

class CuentaPlata : Cuenta {
    public CuentaPlata(string numero, decimal saldo) : base(numero, saldo) {}

    public override void Pagar(decimal monto) {
        if (Extraer(monto)) {
            Puntos += monto * 0.02m;
        }
    }
}

class CuentaBronce : Cuenta {
    public CuentaBronce(string numero, decimal saldo) : base(numero, saldo) {}

    public override void Pagar(decimal monto) {
        if (Extraer(monto)) {
            Puntos += monto * 0.01m;
        }
    }
}

class Cliente {
    public string Nombre { get; private set; }
    public List<Cuenta> Cuentas { get; } = new();

    public Cliente(string nombre) => Nombre = nombre;

    public void Agregar(Cuenta cuenta) {
        cuenta.Titular = this;
        Cuentas.Add(cuenta);
    }

    public decimal SaldoTotal() => Cuentas.Sum(c => c.Saldo);
    public decimal PuntosTotal() => Cuentas.Sum(c => c.Puntos);
    public Cuenta? BuscarCuenta(string numero) => Cuentas.FirstOrDefault(c => c.Numero == numero);
}

abstract class Operacion {
    public decimal Monto { get; protected set; }
    public abstract void Ejecutar(Banco banco);
    public abstract string Descripcion();
}

class Deposito : Operacion {
    string CuentaDestino;
    public Deposito(string cuentaDestino, decimal monto) {
        CuentaDestino = cuentaDestino;
        Monto = monto;
    }

    public override void Ejecutar(Banco banco) {
        var cuenta = banco.BuscarCuenta(CuentaDestino);
        if (cuenta != null) {
            cuenta.Depositar(Monto);
            cuenta.RegistrarOperacion(this);
        }
    }

    public override string Descripcion() => $"Deposito $ {Monto:N2} a [{CuentaDestino}/{Banco.NombreCliente(CuentaDestino)}]";
}

class Retiro : Operacion {
    string CuentaOrigen;
    public Retiro(string cuentaOrigen, decimal monto) {
        CuentaOrigen = cuentaOrigen;
        Monto = monto;
    }

    public override void Ejecutar(Banco banco) {
        var cuenta = banco.BuscarCuenta(CuentaOrigen);
        if (cuenta != null && cuenta.Extraer(Monto)) {
            cuenta.RegistrarOperacion(this);
        }
    }

    public override string Descripcion() => $"Retiro $ {Monto:N2} de [{CuentaOrigen}/{Banco.NombreCliente(CuentaOrigen)}]";
}

class Pago : Operacion {
    string CuentaOrigen;
    public Pago(string cuentaOrigen, decimal monto) {
        CuentaOrigen = cuentaOrigen;
        Monto = monto;
    }

    public override void Ejecutar(Banco banco) {
        var cuenta = banco.BuscarCuenta(CuentaOrigen);
        if (cuenta != null) {
            cuenta.Pagar(Monto);
            cuenta.RegistrarOperacion(this);
        }
    }

    public override string Descripcion() => $"Pago $ {Monto:N2} con [{CuentaOrigen}/{Banco.NombreCliente(CuentaOrigen)}]";
}

class Transferencia : Operacion {
    string CuentaOrigen, CuentaDestino;
    public Transferencia(string cuentaOrigen, string cuentaDestino, decimal monto) {
        CuentaOrigen = cuentaOrigen;
        CuentaDestino = cuentaDestino;
        Monto = monto;
    }

    public override void Ejecutar(Banco banco) {
        var origen = banco.BuscarCuenta(CuentaOrigen);
        var destino = banco.BuscarCuenta(CuentaDestino);
        if (origen != null && destino != null && origen.Extraer(Monto)) {
            destino.Depositar(Monto);
            origen.RegistrarOperacion(this);
            destino.RegistrarOperacion(this);
        }
    }

    public override string Descripcion() => $"Transferencia $ {Monto:N2} de [{CuentaOrigen}/{Banco.NombreCliente(CuentaOrigen)}] a [{CuentaDestino}/{Banco.NombreCliente(CuentaDestino)}]";
}

class Banco {
    public string Nombre { get; private set; }
    List<Cliente> Clientes = new();
    List<Operacion> Operaciones = new();

    public Banco(string nombre) => Nombre = nombre;

    public void Agregar(Cliente cliente) => Clientes.Add(cliente);

    public void Registrar(Operacion op) {
        op.Ejecutar(this);
        Operaciones.Add(op);
    }

    public Cuenta? BuscarCuenta(string numero) => Clientes.SelectMany(c => c.Cuentas).FirstOrDefault(c => c.Numero == numero);

    public static string NombreCliente(string cuentaNum) => bancos.SelectMany(b => b.Clientes)
        .SelectMany(c => c.Cuentas)
        .FirstOrDefault(c => c.Numero == cuentaNum)?.Titular.Nombre ?? "Desconocido";

    public void Informe() {
        Console.WriteLine($"Banco: {Nombre} | Clientes: {Clientes.Count}");
        foreach (var cliente in Clientes) {
            Console.WriteLine($"\n  Cliente: {cliente.Nombre} | Saldo Total: $ {cliente.SaldoTotal():N2} | Puntos Total: $ {cliente.PuntosTotal():N2}");
            foreach (var cuenta in cliente.Cuentas) {
                Console.WriteLine($"\n    Cuenta: {cuenta.Numero} | Saldo: $ {cuenta.Saldo:N2} | Puntos: $ {cuenta.Puntos:N2}");
                foreach (var op in cuenta.Operaciones) {
                    Console.WriteLine($"     -  {op.Descripcion()}");
                }
            }
        }
        Console.WriteLine();
    }

