// TP2: Sistema de Cuentas Bancarias
//

// Implementar un sistema de cuentas bancarias que permita realizar operaciones como depósitos, retiros, transferencias y pagos.

class Banco{class Banco
{
    private string nombre;
    private List<Cliente> clientes = new List<Cliente>();
    private List<Operacion> operaciones = new List<Operacion>();

    public Banco(string nombre)
    {
        this.nombre = nombre;
    }

    public void Agregar(Cliente cliente)
    {
        clientes.Add(cliente);
    }

    public void Registrar(Operacion operacion)
    {
        operacion.Ejecutar(this);
        operaciones.Add(operacion);
    }

    public Cuenta BuscarCuenta(string numero)
    {
        foreach (var cliente in clientes)
        {
            Cuenta cuenta = cliente.BuscarCuenta(numero);
            if (cuenta != null)
                return cuenta;
        }
        return null;
    }

    public void RegistrarEnCliente(string numeroCuenta, Operacion operacion)
    {
        foreach (var cliente in clientes)
        {
            if (cliente.TieneCuenta(numeroCuenta))
            {
                cliente.RegistrarOperacion(operacion);
                break;
            }
        }
    }

    public void Informe()
    {
        Console.WriteLine($"\nBanco: {nombre} | Clientes: {clientes.Count}");

        foreach (var cliente in clientes)
        {
            cliente.Informe();
        }
    }
}

    }
class Cliente{ private string nombre;
    private List<Cuenta> cuentas = new List<Cuenta>();
    private List<Operacion> historial = new List<Operacion>();

    public Cliente(string nombre) { }

    public void Agregar(Cuenta cuenta) {}

    public bool TieneCuenta(string numeroCuenta) { }
    public Cuenta ObtenerCuenta(string numeroCuenta) {}

    public void RegistrarOperacion(Operacion operacion) {}

    public void MostrarHistorial() {}

    public void MostrarEstadoCuentas() { }
    }

abstract class Cuenta{
     public string Numero { get; private set; }
    public double Saldo { get; private set; }
    public double Puntos { get; private set; }

    public Cuenta(string numero, double saldoInicial)
    {
        Numero = numero;
        Saldo = saldoInicial;
        Puntos = 0;
    }

    public void Depositar(double monto) {
        Saldo+= monto;
    }

    public bool Extraer(double monto) {
        if (monto <= Saldo) {
            Saldo -= monto;
            return true;
        }
        return false;
    }

    public void Transferir(Cuenta destino, double monto) {
        if (Extraer(monto)) {
            destino.Depositar(monto);
        }
    }

    public void MostrarEstado() {}

    public abstract void Pagar(double monto);}
    private abstract void acumularPuntos(double monto) { }
class CuentaOro: Cuenta{ public CuentaOro(string numero, double saldo) : base(numero, saldo) { }
 public override void acumularPuntos(double monto) {
        if (monto > 1000) {
            Puntos += monto * 0.05; 
            else Puntos+= monto * 0.03;
            }
    }
    public override void Pagar(double monto) {}}
class CuentaPlata: Cuenta{public CuentaPlata(string numero, double saldo) : base(numero, saldo) { }

    private override void AcumularPuntos(double monto)
    {
        Puntos += monto * 0.02;
    }}
class CuentaBronce: Cuentapublic CuentaBronce(string numero, double saldo) : base(numero, saldo) { }

    private override void AcumularPuntos(double monto)
    {
        Puntos += monto * 0.01;
    }
    

abstract class Operacion{
     private double monto;
    private string origen;
    private string destino;

    public Operacion(double monto, string origen, string destino = null)
    {
        this.monto = monto;
        this.origen = origen;
        this.destino = destino;
    }

    public abstract void Ejecutar(Banco banco);

    public abstract string Descripcion();

    public bool Involucra(string cuenta)
    {
        return origen == cuenta || destino == cuenta;
    }
}
class Deposito: Operacion{ public Deposito(string destino, double monto) : base(monto, null, destino) {}

    public override void Ejecutar(Banco banco)
    {
        Cuenta cuenta = banco.BuscarCuenta(destino);
        if (cuenta != null)
        {
            cuenta.Depositar(monto);
            banco.RegistrarEnCliente(destino, this);
        }
    }

    public override string Descripcion()
    {
        return $"Deposito $ {monto:N2} a [{destino}]";
    }
    }
class Retiro: Operacion{public Retiro(string origen, double monto) : base(monto, origen) {}

    public override void Ejecutar(Banco banco)
    {
        Cuenta cuenta = banco.BuscarCuenta(origen);
        if (cuenta != null && cuenta.Saldo >= monto)
        {
            cuenta.Extraer(monto);
            banco.RegistrarEnCliente(origen, this);
        }
    }

    public override string Descripcion()
    {
        return $"Retiro $ {monto:N2} de [{origen}]";
    }
    }
class Transferencia: Operacion{
    public Transferencia(string origen, string destino, double monto) : base(monto, origen, destino) {}

    public override void Ejecutar(Banco banco)
    {
        Cuenta cOrigen = banco.BuscarCuenta(origen);
        Cuenta cDestino = banco.BuscarCuenta(destino);

        if (cOrigen != null && cDestino != null && cOrigen.Saldo >= monto)
        {
            cOrigen.Extraer(monto);
            cDestino.Depositar(monto);
            banco.RegistrarEnCliente(origen, this);
            banco.RegistrarEnCliente(destino, this);
        }
        }
}
class Pago: Operacion{public Pago(string origen, double monto) : base(monto, origen) {}

    public override void Ejecutar(Banco banco)
    {
        Cuenta cuenta = banco.BuscarCuenta(origen);
        if (cuenta != null && cuenta.Saldo >= monto)
        {
            cuenta.Pagar(monto);
            banco.RegistrarEnCliente(origen, this);
        }
    }

    public override string Descripcion()
    {
        return $"Pago $ {monto:N2} con [{origen}]";
    }}


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

