// TP2: Sistema de Cuentas Bancarias
//

// Implementar un sistema de cuentas bancarias que permita realizar operaciones como depósitos, retiros, transferencias y pagos.

public class Banco{
    public string Nombre { get; set; }
    private List<Cliente> clientes = new List<Cliente>();
    private List<Operacion> historialGlobal = new List<Operacion>();
    public static List<Banco> Bancos = new List<Banco>(); // Lista estática para buscar cuentas en otros bancos

    public Banco(string nombre)
    {
        Nombre = nombre;
        Bancos.Add(this); // Agregar el banco a la lista global
    }

    public void Agregar(Cliente cliente)
    {
        clientes.Add(cliente);
    }

    public void Registrar(Operacion operacion)
    {
        operacion.Ejecutar(this); // Ejecutar la operación pasándole el banco actual
        historialGlobal.Add(operacion); // Registrar en el historial global
    }
    public Cuenta ObtenerCuenta(string numero)
    {
        foreach (var cliente in clientes)
        {
            var cuenta = cliente.Cuentas.FirstOrDefault(c => c.Numero == numero);
            if (cuenta != null)
                return cuenta;
        }
        return null; // Retorna null si no se encuentra la cuenta
    }

    public void Informe()
    {
        Console.WriteLine($"Informe del banco {Nombre}:");
        foreach (var operacion in historialGlobal)
        {
            Console.WriteLine(operacion.ToString());
        }
    }
}
public class Cliente{
    public string Nombre { get; set; }
    public List<Cuenta> Cuentas { get; set; } = new List<Cuenta>();
    public List<Operacion> Historial { get; set; } = new List<Operacion>();

    public Cliente(string nombre)
    {
        Nombre = nombre;
    }

    public void Agregar(Cuenta cuenta)
    {
        Cuentas.Add(cuenta);
        cuenta.Cliente = this; // Establece la referencia al cliente en la cuenta
    }
}

public abstract class Cuenta{
    public string Numero { get; set; }
    public decimal Saldo { get; set; }
    public Cliente Cliente { get; set; } // Referencia al cliente propietario

    public Cuenta(string numero, decimal saldo)
    {
        Numero = numero;
        Saldo = saldo;
    }

    public void Depositar(decimal monto)
    {
        Saldo += monto;
    }

    public void Retirar(decimal monto)
    {
        if (Saldo >= monto)
            Saldo -= monto;
        else
            throw new InvalidOperationException("Saldo insuficiente");
    }
}
public class CuentaOro: Cuenta{
    public CuentaOro(string numero, decimal saldo) : base(numero, saldo) { }
}
public class CuentaPlata: Cuenta{
    public CuentaPlata(string numero, decimal saldo) : base(numero, saldo) { }
}
public class CuentaBronce: Cuenta{
    public CuentaBronce(string numero, decimal saldo) : base(numero, saldo) { }
}

public abstract class Operacion{
    public DateTime Fecha { get; set; }
    public abstract void Ejecutar(Banco banco);
    public abstract override string ToString();
}
public class Deposito: Operacion{
    public string NumeroCuenta { get; set; }
    public decimal Monto { get; set; }

    public Deposito(string numeroCuenta, decimal monto)
    {
        NumeroCuenta = numeroCuenta;
        Monto = monto;
        Fecha = DateTime.Now;
    }

    public override void Ejecutar(Banco banco)
    {
        var cuenta = banco.ObtenerCuenta(NumeroCuenta);
        if (cuenta == null)
            throw new InvalidOperationException("Cuenta no encontrada");
        cuenta.Depositar(Monto);
        cuenta.Cliente.Historial.Add(this); // Registrar en el historial del cliente
    }
    public override string ToString()
    {
        return $"Deposito de {Monto} en cuenta {NumeroCuenta} el {Fecha}";
    }
}
public class Retiro: Operacion{
    public string NumeroCuenta { get; set; }
    public decimal Monto { get; set; }

    public Retiro(string numeroCuenta, decimal monto)
    {
        NumeroCuenta = numeroCuenta;
        Monto = monto;
        Fecha = DateTime.Now;
    }

    public override void Ejecutar(Banco banco)
    {
        var cuenta = banco.ObtenerCuenta(NumeroCuenta);
        if (cuenta == null)
            throw new InvalidOperationException("Cuenta no encontrada");
        cuenta.Retirar(Monto);
        cuenta.Cliente.Historial.Add(this);
    }

    public override string ToString()
    {
        return $"Retiro de {Monto} de cuenta {NumeroCuenta} el {Fecha}";
    }
}
public class Transferencia: Operacion{
    public string NumeroOrigen { get; set; }
    public string NumeroDestino { get; set; }
    public decimal Monto { get; set; }

    public Transferencia(string numeroOrigen, string numeroDestino, decimal monto)
    {
        NumeroOrigen = numeroOrigen;
        NumeroDestino = numeroDestino;
        Monto = monto;
        Fecha = DateTime.Now;
    }

    public override void Ejecutar(Banco bancoOrigen)
    {
        var origen = bancoOrigen.ObtenerCuenta(NumeroOrigen);
        if (origen == null)
            throw new InvalidOperationException("Cuenta origen no encontrada");
    
    Cuenta destino = null;
        foreach (var banco in Banco.Bancos)
        {
            destino = banco.ObtenerCuenta(NumeroDestino);
            if (destino != null)
                break;
        }
        if (destino == null)
            throw new InvalidOperationException("Cuenta destino no encontrada");

        origen.Retirar(Monto);
        destino.Depositar(Monto);
        origen.Cliente.Historial.Add(this);
        if (destino.Cliente != origen.Cliente)
            destino.Cliente.Historial.Add(this); // Registrar en ambos clientes si son diferentes
    }
    public override string ToString()
    {
        return $"Transferencia de {Monto} de cuenta {NumeroOrigen} a cuenta {NumeroDestino} el {Fecha}";
    }
}
public class Pago: Operacion{
    public string NumeroCuenta { get; set; }
    public decimal Monto { get; set; }

    public Pago(string numeroCuenta, decimal monto)
    {
        NumeroCuenta = numeroCuenta;
        Monto = monto;
        Fecha = DateTime.Now;
    }

    public override void Ejecutar(Banco banco)
    {
        var cuenta = banco.ObtenerCuenta(NumeroCuenta);
        if (cuenta == null)
            throw new InvalidOperationException("Cuenta no encontrada");
        cuenta.Retirar(Monto);
        cuenta.Cliente.Historial.Add(this);
    }
    public override string ToString()
    {
        return $"Pago de {Monto} desde cuenta {NumeroCuenta} el {Fecha}";
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
