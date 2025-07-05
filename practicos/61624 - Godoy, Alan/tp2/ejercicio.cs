// TP2: Sistema de Cuentas Bancarias
//

// Implementar un sistema de cuentas bancarias que permita realizar operaciones como depósitos, retiros, transferencias y pagos.

using System;
using System.Collections.Generic;

class Banco {
    public string Nombre;
    private List<Cliente> clientes = new List<Cliente>();
    private List<Operacion> operaciones = new List<Operacion>();

    public Banco(string nombre) {
        Nombre = nombre;
    }

    public void Agregar(Cliente cliente) {
        clientes.Add(cliente);
    }

    public void Registrar(Operacion op) {
        foreach (var cliente in clientes) {
            foreach (var cuenta in cliente.Cuentas) {
                if (cuenta.Numero == op.CuentaOrigen || cuenta.Numero == op.CuentaDestino) {
                    op.Ejecutar(clientes);
                    operaciones.Add(op);
                    return;
                }
            }
        }
    }

    public void Informe() {
        Console.WriteLine($"Banco: {Nombre} | Clientes: {clientes.Count}\n");
        foreach (var cliente in clientes) {
            cliente.Informe();
        }
    }
}

class Cliente {
    public string Nombre;
    public List<Cuenta> Cuentas = new List<Cuenta>();
    private List<Operacion> historial = new List<Operacion>();

    public Cliente(string nombre) {
        Nombre = nombre;
    }

    public void Agregar(Cuenta cuenta) {
        Cuentas.Add(cuenta);
    }

    public void AgregarOperacion(Operacion op) {
        historial.Add(op);
    }

    public void Informe() {
        double saldoTotal = 0;
        double puntosTotal = 0;
        foreach (var c in Cuentas) {
            saldoTotal += c.Saldo;
            puntosTotal += c.Puntos;
        }
        Console.WriteLine($"  Cliente: {Nombre} | Saldo Total: $ {saldoTotal:0.00} | Puntos Total: $ {puntosTotal:0.00}\n");
        foreach (var c in Cuentas) {
            c.Informe(Nombre);
        }
    }
}

abstract class Cuenta {
    public string Numero;
    public double Saldo;
    public double Puntos;
    public List<Operacion> historial = new List<Operacion>();

    public Cuenta(string numero, double saldo) {
        Numero = numero;
        Saldo = saldo;
        Puntos = 0;
    }

    public void AgregarOperacion(Operacion op) {
        historial.Add(op);
    }

    public abstract void SumarPuntos(double monto);

    public void Informe(string clienteNombre) {
        Console.WriteLine($"    Cuenta: {Numero} | Saldo: $ {Saldo:0.00} | Puntos: $ {Puntos:0.00}");
        foreach (var op in historial) {
            Console.WriteLine("     -  " + op.Descripcion(clienteNombre));
        }
        Console.WriteLine();
    }
}

class CuentaOro : Cuenta {
    public CuentaOro(string numero, double saldo) : base(numero, saldo) {}
    public override void SumarPuntos(double monto) {
        Puntos += monto > 1000 ? monto * 0.05 : monto * 0.03;
    }
}

class CuentaPlata : Cuenta {
    public CuentaPlata(string numero, double saldo) : base(numero, saldo) {}
    public override void SumarPuntos(double monto) {
        Puntos += monto * 0.02;
    }
}

class CuentaBronce : Cuenta {
    public CuentaBronce(string numero, double saldo) : base(numero, saldo) {}
    public override void SumarPuntos(double monto) {
        Puntos += monto * 0.01;
    }
}

abstract class Operacion {
    public string CuentaOrigen;
    public string CuentaDestino;
    public double Monto;
    public abstract void Ejecutar(List<Cliente> clientes);
    public abstract string Descripcion(string clienteNombre);
}

class Deposito : Operacion {
    public Deposito(string cuenta, double monto) {
        CuentaOrigen = cuenta;
        Monto = monto;
    }

    public override void Ejecutar(List<Cliente> clientes) {
        foreach (var cliente in clientes) {
            foreach (var cuenta in cliente.Cuentas) {
                if (cuenta.Numero == CuentaOrigen) {
                    cuenta.Saldo += Monto;
                    cuenta.AgregarOperacion(this);
                }
            }
        }
    }

    public override string Descripcion(string clienteNombre) {
        return $"Deposito $ {Monto:0.00} a [{CuentaOrigen}/{clienteNombre}]";
    }
}

class Retiro : Operacion {
    public Retiro(string cuenta, double monto) {
        CuentaOrigen = cuenta;
        Monto = monto;
    }

    public override void Ejecutar(List<Cliente> clientes) {
        foreach (var cliente in clientes) {
            foreach (var cuenta in cliente.Cuentas) {
                if (cuenta.Numero == CuentaOrigen && cuenta.Saldo >= Monto) {
                    cuenta.Saldo -= Monto;
                    cuenta.AgregarOperacion(this);
                }
            }
        }
    }

    public override string Descripcion(string clienteNombre) {
        return $"Retiro $ {Monto:0.00} de [{CuentaOrigen}/{clienteNombre}]";
    }
}

class Pago : Operacion {
    public Pago(string cuenta, double monto) {
        CuentaOrigen = cuenta;
        Monto = monto;
    }

    public override void Ejecutar(List<Cliente> clientes) {
        foreach (var cliente in clientes) {
            foreach (var cuenta in cliente.Cuentas) {
                if (cuenta.Numero == CuentaOrigen && cuenta.Saldo >= Monto) {
                    cuenta.Saldo -= Monto;
                    cuenta.SumarPuntos(Monto);
                    cuenta.AgregarOperacion(this);
                }
            }
        }
    }

    public override string Descripcion(string clienteNombre) {
        return $"Pago $ {Monto:0.00} con [{CuentaOrigen}/{clienteNombre}]";
    }
}

class Transferencia : Operacion {
    public Transferencia(string origen, string destino, double monto) {
        CuentaOrigen = origen;
        CuentaDestino = destino;
        Monto = monto;
    }

    public override void Ejecutar(List<Cliente> clientes) {
        Cuenta cuentaOrigen = null;
        Cuenta cuentaDestino = null;
        string nombreOrigen = "", nombreDestino = "";

        foreach (var cliente in clientes) {
            foreach (var cuenta in cliente.Cuentas) {
                if (cuenta.Numero == CuentaOrigen && cuenta.Saldo >= Monto) {
                    cuentaOrigen = cuenta;
                    nombreOrigen = cliente.Nombre;
                }
                if (cuenta.Numero == CuentaDestino) {
                    cuentaDestino = cuenta;
                    nombreDestino = cliente.Nombre;
                }
            }
        }

        if (cuentaOrigen != null && cuentaDestino != null) {
            cuentaOrigen.Saldo -= Monto;
            cuentaDestino.Saldo += Monto;
            cuentaOrigen.AgregarOperacion(this);
            cuentaDestino.AgregarOperacion(this);
        }
    }

    public override string Descripcion(string clienteNombre) {
        return $"Transferencia $ {Monto:0.00} de [{CuentaOrigen}/{clienteNombre}] a [{CuentaDestino}]";
    }
}


