// TP2: Sistema de Cuentas Bancarias
//

// Implementar un sistema de cuentas bancarias que permita realizar operaciones como depósitos, retiros, transferencias y pagos.


class Banco {
    public string Nombre { get; set; }
    public List<Cliente> Clientes { get; set; }
    public static Dictionary<string, Cuenta> Cuentas = new Dictionary<string, Cuenta>();
    public List<Operacion> HistorialGlobal { get; set; } = new List<Operacion>();

    public Banco(string nombre) {
        Nombre = nombre;
        Clientes = new List<Cliente>();
    }

    public void Agregar(Cliente cliente) {
        Clientes.Add(cliente);
    }

    public static void Registrar(Cuenta cuenta) {
        if (Cuentas.ContainsKey(cuenta.Numero)) {
            Console.WriteLine($"La cuenta {cuenta.Numero} ya existe.");
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

    public void Registrar(Operacion operacion) {
        if (operacion.Ejecutar()) {
            HistorialGlobal.Add(operacion);

           
            var cliente = Clientes.FirstOrDefault(c => c.Cuentas.Contains(operacion.Cuenta));
            if (cliente != null) {
                cliente.Historial.Add(operacion);
            }

           
            if (operacion is Transferencia t && t.Destino != null) {
                var destinoCliente = Clientes.FirstOrDefault(c => c.Cuentas.Contains(t.Destino));
                if (destinoCliente != null) {
                    destinoCliente.Historial.Add(operacion);
                }
            }
        } else {
            Console.WriteLine($"La operación falló: {operacion.GetType().Name}");
        }
    }

    public void Informe() {
        Console.WriteLine($"\n=== Informe del Banco {Nombre} ===");

        Console.WriteLine("\n> Operaciones globales:");
        foreach (var op in HistorialGlobal) {
            Console.WriteLine($" - {op}");
        }

        Console.WriteLine("\n> Estado de cuentas:");
        foreach (var cuenta in Cuentas.Values) {
            Console.WriteLine($" - {cuenta.Numero}: Saldo = {cuenta.Saldo}, Puntos = {cuenta.Puntos}");
        }

        Console.WriteLine("\n> Historial por cliente:");
        foreach (var cliente in Clientes) {
            Console.WriteLine($"\nCliente: {cliente.Nombre}");
            foreach (var op in cliente.Historial) {
                Console.WriteLine($" - {op}");
            }
        }
    }
}//fin Banco

class Cliente {
    public string Nombre { get; set; }
    public List<Cuenta> Cuentas { get; set; } = new List<Cuenta>();
    public List<Operacion> Historial { get; set; } = new List<Operacion>();

    public Cliente(string nombre) {
        Nombre = nombre;
    }

    public void Agregar(Cuenta cuenta) {
        Cuentas.Add(cuenta);
        Banco.Registrar(cuenta);
    }
}

abstract class Cuenta {
    public string Numero { get; set; }
    public decimal Saldo { get; set; }
    public int Puntos { get; set; }

    public Cuenta(string numero, decimal saldo) {
        Numero = numero;
        Saldo = saldo;
        Puntos = 0;
    }

    public virtual bool Depositar(decimal cantidad) {
        if (cantidad <= 0) return false;
        Saldo += cantidad;
        return true;
    }

    public virtual bool Extraer(decimal cantidad) {
        if (cantidad <= 0 || cantidad > Saldo) return false;
        Saldo -= cantidad;
        return true;
    }

    public virtual void AcumularPuntos(decimal monto) {
        
    }

    public override string ToString() {
        return $"{Numero} - Saldo: {Saldo} - Puntos: {Puntos}";
    }
}

class CuentaOro : Cuenta {
    public CuentaOro(string numero, decimal saldo) : base(numero, saldo) { }

    public override void AcumularPuntos(decimal monto) {
        if (monto > 1000)
            Puntos += (int)(monto * 0.05m);
        else
            Puntos += (int)(monto * 0.03m);
    }
}
class CuentaPlata : Cuenta {
    public CuentaPlata(string numero, decimal saldo) : base(numero, saldo) { }

    public override void AcumularPuntos(decimal monto) {
        Puntos += (int)(monto * 0.02m);
    }
}

class CuentaBronce : Cuenta {
    public CuentaBronce(string numero, decimal saldo) : base(numero, saldo) { }

    public override void AcumularPuntos(decimal monto) {
        Puntos += (int)(monto * 0.01m);
    }
}

abstract class Operacion {
    public Cuenta Cuenta { get; set; }
    public decimal Monto { get; set; }

    public Operacion(string numero, decimal monto) {
        Cuenta = Banco.Buscar(numero);
        Monto = monto;
    }

    public abstract bool Ejecutar();

    public override string ToString() {
        return $"{GetType().Name} - Cuenta: {Cuenta?.Numero} - Monto: {Monto}";
    }
}

class Deposito : Operacion {
    public Deposito(string numero, decimal monto) : base(numero, monto) { }

    public override bool Ejecutar() {
        return Cuenta != null && Cuenta.Depositar(Monto);
    }
}

class Retiro : Operacion {
    public Retiro(string numero, decimal monto) : base(numero, monto) { }

    public override bool Ejecutar() {
        return Cuenta != null && Cuenta.Extraer(Monto);
    }
}

class Pago : Operacion {
    public Pago(string numero, decimal monto) : base(numero, monto) { }

    public override bool Ejecutar() {
        if (Cuenta == null) return false;
        if (Cuenta.Extraer(Monto)) {
            Cuenta.AcumularPuntos(Monto);
            return true;
        }
        return false;
    }
}

class Transferencia : Operacion {
    public Cuenta Destino { get; set; }

    public Transferencia(string origen, string destino, decimal monto) : base(origen, monto) {
        Destino = Banco.Buscar(destino);
    }

    public override bool Ejecutar() {
        if (Cuenta == null || Destino == null) return false;
        if (Cuenta.Extraer(Monto)) {
            return Destino.Depositar(Monto);
        }
        return false;
    }

    public override string ToString() {
        return $"Transferencia - De: {Cuenta?.Numero} a {Destino?.Numero} - Monto: {Monto}";
    }
}

/// EJEMPLO DE USO ///

// Definiciones 

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