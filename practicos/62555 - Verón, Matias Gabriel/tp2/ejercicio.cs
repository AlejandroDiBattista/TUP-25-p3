// TP2: Sistema de Cuentas Bancarias
//

// Implementar un sistema de cuentas bancarias que permita realizar operaciones como depósitos, retiros, transferencias y pagos.

class Banco
{

    public string Nombre { get; private set; }
    private List<Cliente> clientes = new List<Cliente>();
    private List<Operacion> operaciones = new List<Operacion>();

    public Banco(string nombre)
    {
        Nombre = nombre;
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
            foreach (var cuenta in cliente.Cuentas)
            {
                if (cuenta.Numero == numero)
                    return cuenta;
            }
        }
        return null;
    }

    public Cliente BuscarClientePorCuenta(string numero)
    {
        foreach (var cliente in clientes)
        {
            foreach (var cuenta in cliente.Cuentas)
            {
                if (cuenta.Numero == numero)
                    return cliente;
            }
        }
        return null;
    }

    public void Informe()
    {
        Console.WriteLine("\nBanco: " + Nombre + " | Clientes: " + clientes.Count + "\n");

        foreach (var cliente in clientes)
        {
            double saldoTotal = 0;
            double puntosTotal = 0;

            foreach (var cuenta in cliente.Cuentas)
            {
                saldoTotal += cuenta.Saldo;
                puntosTotal += cuenta.Puntos;
            }

            Console.WriteLine("  Cliente: " + cliente.Nombre + " | Saldo Total: $ " + saldoTotal.ToString("N2") + " | Puntos Total: $ " + puntosTotal.ToString("N2") + "\n");

            foreach (var cuenta in cliente.Cuentas)
            {
                Console.WriteLine("    Cuenta: " + cuenta.Numero + " | Saldo: $ " + cuenta.Saldo.ToString("N2") + " | Puntos: $ " + cuenta.Puntos.ToString("N2"));

                foreach (var op in cliente.Historial)
                {
                    if (op.CuentaOrigen == cuenta.Numero || op.CuentaDestino == cuenta.Numero)
                    {
                        Console.WriteLine("     -  " + op.Descripcion(this));
                    }
                }

                Console.WriteLine();
            }
        }
    }
}
class Cliente
{
    public string Nombre { get; private set; }
    public List<Cuenta> Cuentas { get; private set; } = new List<Cuenta>();
    public List<Operacion> Historial { get; private set; } = new List<Operacion>();

    public Cliente(string nombre)
    {
        Nombre = nombre;
    }

    public void Agregar(Cuenta cuenta)
    {
        Cuentas.Add(cuenta);
    }

    public void RegistrarOperacion(Operacion op)
    {
        Historial.Add(op);
    }
}

abstract class Cuenta
{
    public string Numero { get; protected set; }
    public double Saldo { get; set; }
    public double Puntos { get; set; }

    public Cuenta(string numero, double saldo)
    {
        Numero = numero;
        Saldo = saldo;
    }
}
class CuentaOro : Cuenta
{
    public CuentaOro(string numero, double saldo) : base(numero, saldo) { }
}
class CuentaPlata : Cuenta
{
    public CuentaPlata(string numero, double saldo) : base(numero, saldo) { }
}
class CuentaBronce : Cuenta
{
    public CuentaBronce(string numero, double saldo) : base(numero, saldo) { }
}

abstract class Operacion
{
    private string tipo;
    private double monto;
    private string cuentaOrigen;
    private string cuentaDestino;

    public string Tipo { get { return tipo; } }
    public double Monto { get { return monto; } }
    public string CuentaOrigen { get { return cuentaOrigen; } }
    public string CuentaDestino { get { return cuentaDestino; } }

    public Operacion(string tipo, string cuentaOrigen, double monto, string cuentaDestino = null)
    {
        this.tipo = tipo;
        this.cuentaOrigen = cuentaOrigen;
        this.monto = monto;
        this.cuentaDestino = cuentaDestino;
    }

    public abstract void Ejecutar(Banco banco);
    public abstract string Descripcion(Banco banco);
}
class Deposito : Operacion
{
    public Deposito(string cuenta, double monto) : base("Deposito", cuenta, monto) { }

    public override void Ejecutar(Banco banco)
    {
        var c = banco.BuscarCuenta(CuentaOrigen);
        c.Saldo += Monto;
        if (c is CuentaOro) c.Puntos += Monto * 0.01;
        var cliente = banco.BuscarClientePorCuenta(CuentaOrigen);
        if (cliente != null) cliente.RegistrarOperacion(this);
    }

    public override string Descripcion(Banco banco)
    {
        var cliente = banco.BuscarClientePorCuenta(CuentaOrigen);
        return "Deposito $ " + Monto.ToString("N2") + " a [" + CuentaOrigen + "/" + (cliente != null ? cliente.Nombre : "Desconocido") + "]";
    }
}
class Retiro : Operacion
{
    public Retiro(string cuenta, double monto) : base("Retiro", cuenta, monto) { }

    public override void Ejecutar(Banco banco)
    {
        var c = banco.BuscarCuenta(CuentaOrigen);
        c.Saldo -= Monto;
        if (c is CuentaPlata) c.Puntos += Monto * 0.02;
        var cliente = banco.BuscarClientePorCuenta(CuentaOrigen);
        if (cliente != null) cliente.RegistrarOperacion(this);
    }

    public override string Descripcion(Banco banco)
    {
        var cliente = banco.BuscarClientePorCuenta(CuentaOrigen);
        return "Retiro $ " + Monto.ToString("N2") + " de [" + CuentaOrigen + "/" + (cliente != null ? cliente.Nombre : "Desconocido") + "]";
    }
}
class Transferencia : Operacion
{
    public Transferencia(string desde, string hacia, double monto) : base("Transferencia", desde, monto, hacia) { }

    public override void Ejecutar(Banco banco)
    {
        var origen = banco.BuscarCuenta(CuentaOrigen);
        var destino = banco.BuscarCuenta(CuentaDestino);

        if (origen == null || destino == null)
        {
            Console.WriteLine("Error: Cuenta origen o destino no encontrada.");
            return;
        }

        origen.Saldo -= Monto;
        destino.Saldo += Monto;

        var clienteOrigen = banco.BuscarClientePorCuenta(CuentaOrigen);
        var clienteDestino = banco.BuscarClientePorCuenta(CuentaDestino);

        if (clienteOrigen != null) clienteOrigen.RegistrarOperacion(this);
        if (clienteDestino != null) clienteDestino.RegistrarOperacion(this);
    }

    public override string Descripcion(Banco banco)
    {
        var clienteOrigen = banco.BuscarClientePorCuenta(CuentaOrigen);
        var clienteDestino = banco.BuscarClientePorCuenta(CuentaDestino);
        return "Transferencia $ " + Monto.ToString("N2") + " de [" + CuentaOrigen + "/" + (clienteOrigen != null ? clienteOrigen.Nombre : "Desconocido") + "] a [" + CuentaDestino + "/" + (clienteDestino != null ? clienteDestino.Nombre : "Desconocido") + "]";
    }
}
class Pago : Operacion
{
    public Pago(string cuenta, double monto) : base("Pago", cuenta, monto) { }

    public override void Ejecutar(Banco banco)
    {
        var c = banco.BuscarCuenta(CuentaOrigen);
        c.Saldo -= Monto;
        if (c is CuentaBronce) c.Puntos += Monto * 0.01;
        var cliente = banco.BuscarClientePorCuenta(CuentaOrigen);
        if (cliente != null) cliente.RegistrarOperacion(this);
    }

    public override string Descripcion(Banco banco)
    {
        var cliente = banco.BuscarClientePorCuenta(CuentaOrigen);
        return "Pago $ " + Monto.ToString("N2") + " con [" + CuentaOrigen + "/" + (cliente != null ? cliente.Nombre : "Desconocido") + "]";
    }
}


/// EJEMPLO DE USO ///

// Definiciones 
class Prueba
{
    static void Main()
    {
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
        nac.Agregar(luis)

        var tup = new Banco("Banco TUP");
        tup.Agregar(luis);


        // Registrar Operaciones
        nac.Registrar(new Deposito("10001", 100));
        nac.Registrar(new Retiro("10002", 200));
        nac.Registrar(new Transferencia("10001", "10002", 300));
        nac.Registrar(new Transferencia("10003", "10004", 500));
        nac.Registrar(new Pago("10002", 400));
        nac.Registrar(new Transferencia("10005", "10002", 300));

        tup.Registrar(new Deposito("10005", 100));
        tup.Registrar(new Retiro("10005", 200));
        tup.Registrar(new Transferencia("10005", "10002", 300));
        tup.Registrar(new Pago("10005", 400));


        // Informe final
        nac.Informe();
        tup.Informe();

    }
}