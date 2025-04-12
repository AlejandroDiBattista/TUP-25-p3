// TP2: Sistema de Cuentas Bancarias

 abstract class Cuenta
{
    public string Numero { get; }
    public decimal Saldo { get; protected set; }
    public decimal Puntos { get; protected set; }
    public Cliente Dueño { get; }

    protected Cuenta(string numero, decimal saldoInicial, Cliente Dueño)
    {
        Numero = numero;
        Saldo = saldoInicial;
        Dueño = Dueño;
    }

    public abstract void AplicarPago(decimal monto);

    public override string ToString()
    {
        return $"Cuenta: {Numero} | Saldo: $ {Saldo:N2} | Puntos: $ {Puntos:N2}";
    }
}
class CuentaOro : Cuenta
{
    public CuentaOro(string numero, decimal saldoInicial, Cliente propietario)
        : base(numero, saldoInicial, propietario) { }

    public override void AplicarPago(decimal monto)
    {
        if (Saldo >= monto)
        {
            Saldo -= monto;
            Puntos += monto > 1000 ? monto * 0.07m : monto * 0.02m;
        }
        else throw new InvalidOperationException("Fondos insuficientes.");
    }
}

class CuentaPlata : Cuenta
{
    public CuentaPlata(string numero, decimal saldoInicial, Cliente propietario)
        : base(numero, saldoInicial, propietario) { }

    public override void AplicarPago(decimal monto)
    {
        if (Saldo >= monto)
        {
            Saldo -= monto;
            Puntos += monto * 0.…
class Cliente
{
    public string Nombre { get; }
    private List<Cuenta> cuentas = new();
    private List<Operacion> historial = new();

    public Cliente(string nombre) => Nombre = nombre;

    public void Agregar(Cuenta cuenta) => cuentas.Add(cuenta);

    public IEnumerable<Cuenta> Cuentas => cuentas;

    public decimal SaldoTotal => cuentas.Sum(c => c.Saldo);
    public decimal PuntosTotal => cuentas.Sum(c => c.Puntos);

    public void AgregarOperacion(Operacion op) => historial.Add(op);

    public IEnumerable<Operacion> Historial => historial;

    public override string ToString()
    {
        return $"Cliente: {Nombre} | Saldo Total: $ {SaldoTotal:N2} | Puntos Total: $ {PuntosTotal:N2}";
    }
}
 class Banco
{
    public string Nombre { get; }
    private List<Cliente> clientes = new();
    private List<Operacion> operaciones = new();

    public Banco(string nombre) => Nombre = nombre;

    public void Agregar(Cliente cliente) => clientes.Add(cliente);

    public void Registrar(Operacion op)
    {
        if (!op.Validar(this)) return;

        op.Ejecutar(this);
        operaciones.Add(op);
    }

    public Cuenta BuscarCuenta(string numero)
    {
        foreach (var c in clientes.SelectMany(cl => cl.Cuentas))
            if (c.Numero == numero) return c;
        return null;
    }

    public Cliente BuscarPropietario(string numero)
    {
        return clientes.FirstOrDefault(c => c.Cuentas.Any(a => a.Numero == numero));
    }

    public void abstract class Operacion
{
    public decimal Monto { get; protected set; }

    public abstract void Ejecutar(Banco banco);
    public abstract bool Validar(Banco banco);
    public abstract string Descripcion();
    public abstract bool Involucra(string cuenta);
}
 class Deposito : Operacion
{
    private string destino;

    public Deposito(string destino, decimal monto)
    {
        this.destino = destino;
        Monto = monto;
    }

    public override bool Validar(Banco banco) => banco.BuscarCuenta(destino) != null;

    public override void Ejecutar(Banco banco)
    {
        var cuenta = banco.BuscarCuenta(destino);
        cuenta.Saldo += Monto;
        cuenta.Propietario.AgregarOperacion(this);
    }

    public override string Descripcion() =>
        $"Deposito $ {Monto:N2} a [{destino}/{BancoGlobal.ObtenerCliente(destino)?.Nombre}]";

    public override bool Involucra(string cuenta) => cuenta == destino;
}
 class cuenta: Operacion
{
    private string origen;

    public Retiro(string origen, decimal monto)
    {
        this.origen = origen;
        Monto = monto;
    }

    public override bool Validar(Banco banco)
    {
        var cuenta = banco.BuscarCuenta(origen);
        return cuenta != null && cuenta.Saldo >= Monto;
    }

    public override void Ejecutar(Banco banco)
    {
        var cuenta = banco.BuscarCuenta(origen);
        cuenta.Saldo -= Monto;
        cuenta.Propietario.AgregarOperacion(this);
    }

    public override string Descripcion() =>
        $"Retiro $ {Monto:N2} de [{origen}/{BancoGlobal.ObtenerCliente(origen)?.Nombre}]";

    public override bool Involucra(string cuenta) => cuenta == origen;
    
     class Pago : Operacion
{
    private string origen;

    public Pago(string origen, decimal monto)
    {
        this.origen = origen;
        Monto = monto;
    }

    public override bool Validar(Banco banco)
    {
        var cuenta = banco.BuscarCuenta(origen);
        return cuenta != null && cuenta.Saldo >= Monto;
    }

    public override void Ejecutar(Banco banco)
    {
        var cuenta = banco.BuscarCuenta(origen);
        cuenta.AplicarPago(Monto);
        cuenta.Propietario.AgregarOperacion(this);
    }

    public override string Descripcion() =>
        $"Pago $ {Monto:N2} con [{origen}/{BancoGlobal.ObtenerCliente(origen)?.Nombre}]";

    public override bool Involucra(string cuenta) => cuenta == origen;
}
class Transferencia : Operacion
{
    private string origen, destino;

    public Transferencia(string origen, string destino, decimal monto)
    {
        this.origen = origen;
        this.destino = destino;
        Monto = monto;
    }

    public override bool Validar(Banco banco)
    {
        var origenCuenta = banco.BuscarCuenta(origen);
        var destinoCuenta = BancoGlobal.BuscarCuentaGlobal(destino);
        return origenCuenta != null && destinoCuenta != null && origenCuenta.Saldo >= Monto;
    }

    public override void Ejecutar(Banco banco)
    {
        var origenCuenta = banco.BuscarCuenta(origen);
        var destinoCuenta = BancoGlobal.BuscarCuentaGlobal(destino);

        origenCuenta.Saldo -= Monto;
        destinoCuenta.Saldo += Monto;…
static class BancoGlobal
{
    private static List<Banco> bancos = new();

    public static void Registrar(Banco banco) => bancos.Add(banco);

    public static Cuenta BuscarCuentaGlobal(string numero)
    {
        return bancos.SelectMany(b => b.BuscarCuenta(numero)).FirstOrDefault(c => c != null);
    }

    public static Cliente ObtenerCliente(string numero)
    {
        return bancos.Select(b => b.BuscarPropietario(numero)).FirstOrDefault(c => c != null);
    }
}