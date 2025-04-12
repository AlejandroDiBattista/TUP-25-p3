// TP2: Sistema de Cuentas Bancarias
//

// Implementar un sistema de cuentas bancarias que permita realizar operaciones como depósitos, retiros, transferencias y pagos.

class Banco{
    public string Nombre { get; }
    private List<Cliente> Clientes = new();
    private List<Operacion> Operaciones = new();

    private static Dictionary<string, string> CuentaTitularMap = new();

    public Banco(string nombre)
    {
        Nombre = nombre;
    }

    public void Agregar(Cliente cliente)
    {
        Clientes.Add(cliente);
        foreach (var cuenta in cliente.Cuentas)
        {
            CuentaTitularMap[cuenta.Numero] = cliente.Nombre;
        }
    }

    public void Registrar(Operacion operacion)
    {
        operacion.Ejecutar(this);
        Operaciones.Add(operacion);
    }

    public Cuenta BuscarCuenta(string numero)
    {
        foreach (var cliente in Clientes)
            foreach (var cuenta in cliente.Cuentas)
                if (cuenta.Numero == numero)
                    return cuenta;
        return null;
    }

    public void AgregarHistorial(Operacion op, Cuenta cuenta)
    {
        cuenta.Titular?.Historial.Add(op);
    }

    public static string ObtenerNombreTitular(string cuenta)
    {
        return CuentaTitularMap.ContainsKey(cuenta) ? CuentaTitularMap[cuenta] : "Desconocido";
    }

    public void Informe()
    {
        Console.WriteLine($"\nBanco: {Nombre} | Clientes: {Clientes.Count}");

        foreach (var cliente in Clientes)
        {
            Console.WriteLine($"\n  Cliente: {cliente.Nombre} | Saldo Total: $ {cliente.SaldoTotal:0.00} | Puntos Total: $ {cliente.PuntosTotal:0.00}");

            foreach (var cuenta in cliente.Cuentas)
            {
                Console.WriteLine($"\n    Cuenta: {cuenta.Numero} | Saldo: $ {cuenta.Saldo:0.00} | Puntos: $ {cuenta.Puntos:0.00}");

                foreach (var op in cliente.Historial.Where(o => op.Descripcion().Contains(cuenta.Numero)))
                {
                    Console.WriteLine("     -  " + op.Descripcion());
                }
            }
        }
    }
}
class Cliente{
     public string Nombre { get; }
    public List<Cuenta> Cuentas { get; }
    public List<Operacion> Historial { get; }

    public Cliente(string nombre)
    {
        Nombre = nombre;
        Cuentas = new List<Cuenta>();
        Historial = new List<Operacion>();
    }

    public void Agregar(Cuenta cuenta)
    {
        cuenta.Titular = this;
        Cuentas.Add(cuenta);
    }

    public decimal SaldoTotal => Cuentas.Sum(c => c.Saldo);
    public decimal PuntosTotal => Cuentas.Sum(c => c.Puntos);
}

abstract class Cuenta{
      public string Numero { get; }
    public decimal Saldo { get; set; }
    public decimal Puntos { get; set; }

    public Cliente Titular { get; set; }

    public Cuenta(string numero, decimal saldo)
    {
        Numero = numero;
        Saldo = saldo;
        Puntos = 0;
    }

    public void Depositar(decimal monto)
    {
        Saldo += monto;
    }

    public bool Extraer(decimal monto)
    {
        if (Saldo >= monto)
        {
            Saldo -= monto;
            return true;
        }
        return false;
    }

    public void Pagar(decimal monto)
    {
        if (Extraer(monto))
        {
            AgregarPuntos(monto);
        }
    }

    public abstract void AgregarPuntos(decimal monto);
}


class CuentaOro: Cuenta{
     public CuentaOro(string numero, decimal saldo) : base(numero, saldo) { }

    public override void AgregarPuntos(decimal monto)
    {
        if (monto > 1000)
            Puntos += monto * 0.05m;
        else
            Puntos += monto * 0.03m;
    }
}
class CuentaPlata: Cuenta{
    public CuentaPlata(string numero, decimal saldo) : base(numero, saldo) { }

    public override void AgregarPuntos(decimal monto)
    {
        Puntos += monto * 0.02m;
    }
}
class CuentaBronce: Cuenta{
    public CuentaBronce(string numero, decimal saldo) : base(numero, saldo) { }

    public override void AgregarPuntos(decimal monto)
    {
        Puntos += monto * 0.01m;
    }
}

abstract class Operacion{
    public decimal Monto { get; }
    public abstract void Ejecutar(Banco banco);

    public Operacion(decimal monto)
    {
        Monto = monto;
    }

    public abstract string Descripcion();
}
class Deposito: Operacion{
    string CuentaNumero;

    public Deposito(string cuenta, decimal monto) : base(monto)
    {
        CuentaNumero = cuenta;
    }

    public override void Ejecutar(Banco banco)
    {
        var cuenta = banco.BuscarCuenta(CuentaNumero);
        cuenta?.Depositar(Monto);
        banco.AgregarHistorial(this, cuenta);
    }

    public override string Descripcion()
    {
        return $"Deposito $ {Monto:0.00} a [{CuentaNumero}/{Banco.ObtenerNombreTitular(CuentaNumero)}]";
    }
}
class Retiro: Operacion{
    string CuentaNumero;

    public Retiro(string cuenta, decimal monto) : base(monto)
    {
        CuentaNumero = cuenta;
    }

    public override void Ejecutar(Banco banco)
    {
        var cuenta = banco.BuscarCuenta(CuentaNumero);
        if (cuenta != null && cuenta.Extraer(Monto))
        {
            banco.AgregarHistorial(this, cuenta);
        }
    }

    public override string Descripcion()
    {
        return $"Retiro $ {Monto:0.00} de [{CuentaNumero}/{Banco.ObtenerNombreTitular(CuentaNumero)}]";
    }
}
class Transferencia: Operacion{
    string Origen;
    string Destino;

    public Transferencia(string origen, string destino, decimal monto) : base(monto)
    {
        Origen = origen;
        Destino = destino;
    }

    public override void Ejecutar(Banco banco)
    {
        var cuentaOrigen = banco.BuscarCuenta(Origen);
        var cuentaDestino = banco.BuscarCuenta(Destino);

        if (cuentaOrigen != null && cuentaDestino != null && cuentaOrigen.Extraer(Monto))
        {
            cuentaDestino.Depositar(Monto);
            banco.AgregarHistorial(this, cuentaOrigen);
            banco.AgregarHistorial(this, cuentaDestino);
        }
    }

    public override string Descripcion()
    {
        return $"Transferencia $ {Monto:0.00} de [{Origen}/{Banco.ObtenerNombreTitular(Origen)}] a [{Destino}/{Banco.ObtenerNombreTitular(Destino)}]";
    }
}
class Pago: Operacion{
    string CuentaNumero;

    public Pago(string cuenta, decimal monto) : base(monto)
    {
        CuentaNumero = cuenta;
    }

    public override void Ejecutar(Banco banco)
    {
        var cuenta = banco.BuscarCuenta(CuentaNumero);
        if (cuenta != null && cuenta.Saldo >= Monto)
        {
            cuenta.Pagar(Monto);
            banco.AgregarHistorial(this, cuenta);
        }
    }

    public override string Descripcion()
    {
        return $"Pago $ {Monto:0.00} con [{CuentaNumero}/{Banco.ObtenerNombreTitular(CuentaNumero)}]";
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

