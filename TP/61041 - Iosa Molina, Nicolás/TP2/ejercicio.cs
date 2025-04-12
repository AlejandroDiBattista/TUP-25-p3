class Consola
{
    public static void Escribir(string color, string texto)
    {
        Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), color, true);
        Console.Write(texto);
        Console.ResetColor();
    }
}
class Banco
{
    public string Nombre { get; private set; }
    public List<Cliente> Clientes { get; private set; } = new List<Cliente>();
    public List<Operacion> Registro { get; private set; } = new List<Operacion>();
    public Banco(string nombre) { Nombre = nombre; }
    public void Agregar(Cliente cliente) { if (!(Clientes.Any(c => c.Nombre == cliente.Nombre))) Clientes.Add(cliente); }
    public void Registrar(Operacion operacion)
    {
        if (operacion.Ejecutar()) Registro.Add(operacion);
    }
    public void Informe()
    {
        Consola.Escribir("Green", $"\n\nBanco: ");
        Consola.Escribir("Yellow", $"{Nombre}");
        Consola.Escribir("Cyan", $" | ");
        Consola.Escribir("Green", $"Operaciones: ");
        Consola.Escribir("Magenta", $"{Registro.Count}");
        foreach (var operacion in Registro)
        {
            Consola.Escribir("Green", $"\n - ");
            Consola.Escribir("Yellow", $"{operacion.Descripcion()}");
        }
    }
    public void Resumen()
    {
        Consola.Escribir("Green", $"\n\nBanco: ");
        Consola.Escribir("Yellow", $"{Nombre}");
        Consola.Escribir("Cyan", $" | ");
        Consola.Escribir("Green", $"Clientes: ");
        Consola.Escribir("Magenta", $"{Clientes.Count}");
        foreach (var cliente in Clientes)
        {
            cliente.Resumen();
        }
    }
    public static Dictionary<string, Cuenta> Cuentas = new Dictionary<string, Cuenta>();
    public static bool Registrar(Cuenta cuenta)
    {
        if (Cuentas.ContainsKey(cuenta.Numero)) return false;
        Cuentas.Add(cuenta.Numero, cuenta); return true;
    }
    public static Cuenta Buscar(string numero)
    {
        if (Cuentas.TryGetValue(numero, out var cuenta)) return cuenta;
        return null;
    }
}
class Cliente
{
    public string Nombre { get; private set; }
    public List<Cuenta> Cuentas { get; private set; } = new List<Cuenta>();
    public Cliente(string nombre) { Nombre = nombre; }
    public void Agregar(Cuenta cuenta) { if (!Banco.Registrar(cuenta)) return; cuenta.Cliente = this; Cuentas.Add(cuenta); }
    public decimal CalcularSaldo()
    {
        decimal saldo = 0;
        foreach (var cuenta in Cuentas)
        {
            saldo += cuenta.Saldo;
        }
        return saldo;
    }
    public decimal CalcularPuntos()
    {
        decimal puntos = 0;
        foreach (var cuenta in Cuentas)
        {
            puntos += cuenta.Puntos;
        }
        return puntos;
    }
    public void Resumen()
    {
        Consola.Escribir("Green", $"\n\n  Cliente: ");
        Consola.Escribir("Yellow", $"{Nombre}");
        Consola.Escribir("Cyan", $" | ");
        Consola.Escribir("Green", $"Saldo Total: ");
        Consola.Escribir("Yellow", $"{CalcularSaldo():c}");
        Consola.Escribir("Cyan", $" | ");
        Consola.Escribir("Green", $"Puntos Totales: ");
        Consola.Escribir("Yellow", $"{Math.Floor(CalcularPuntos())}");
        foreach (var cuenta in Cuentas)
        {
            cuenta.Resumen();
        }
    }
}
abstract class Cuenta
{
    public string Numero { get; private set; }
    public decimal Saldo { get; private set; }
    public decimal Puntos { get; set; }
    public Cliente Cliente { get; set; }
    public List<Operacion> Historial { get; private set; } = new List<Operacion>();
    public Cuenta(string numero, decimal saldo) { Numero = numero; Saldo = saldo; }
    public bool Depositar(decimal monto) { if (monto <= 0) return false; Saldo += monto; return true; }
    public bool Extraer(decimal monto) { if (monto <= 0 || monto > Saldo) return false; Saldo -= monto; return true; }
    public bool Transferir(decimal monto, Cuenta cuentaDestino)
    {
        if (monto <= 0 || monto > Saldo) return false;
        Saldo -= monto; cuentaDestino.Depositar(monto); return true;
    }
    public bool Pagar(decimal monto) { if (monto <= 0 || monto > Saldo) return false; Saldo -= monto; SumarPuntos(monto); return true; }
    public void Registrar(Operacion operacion) { Historial.Add(operacion); }
    public void Resumen()
    {
        Consola.Escribir("Green", $"\n\n    Cuenta: ");
        Consola.Escribir("Magenta", $"{Numero}");
        Consola.Escribir("Cyan", $" | ");
        Consola.Escribir("Green", $"Saldo: ");
        Consola.Escribir("Yellow", $"{Saldo:c}");
        Consola.Escribir("Cyan", $" | ");
        Consola.Escribir("Green", $"Puntos: ");
        Consola.Escribir("Yellow", $"{Math.Floor(Puntos)}");
        foreach (var operacion in Historial)
        {
            Consola.Escribir("Green", $"\n     - ");
            Consola.Escribir("Yellow", $"{operacion.Descripcion()}");
        }
    }
    public abstract bool SumarPuntos(decimal monto);
}
class CuentaOro : Cuenta
{
    public CuentaOro(string numero, decimal saldo) : base(numero, saldo) { }
    public override bool SumarPuntos(decimal monto)
    {
        if (monto <= 0) return false; if (monto > 1000) { Puntos += monto * 0.05m; return true; }
        Puntos += monto * 0.03m; return true;
    }
}
class CuentaPlata : Cuenta
{
    public CuentaPlata(string numero, decimal saldo) : base(numero, saldo) { }
    public override bool SumarPuntos(decimal monto)
    {
        if (monto <= 0) return false; Puntos += monto * 0.02m; return true;
    }
}
class CuentaBronce : Cuenta
{
    public CuentaBronce(string numero, decimal saldo) : base(numero, saldo) { }
    public override bool SumarPuntos(decimal monto)
    {
        if (monto <= 0) return false; Puntos += monto * 0.01m; return true;
    }
}
abstract class Operacion
{
    public DateTime Fecha { get; private set; } = DateTime.Now;
    public Cuenta Origen { get; private set; }
    public decimal Monto { get; private set; }
    public Operacion(string numero, decimal monto) { Monto = monto; Origen = Banco.Buscar(numero); }
    public abstract bool Ejecutar();
    public abstract string Descripcion();
}
class Deposito : Operacion
{
    public Deposito(string numero, decimal monto) : base(numero, monto) { }
    public override bool Ejecutar()
    {
        if (Origen == null || !Origen.Depositar(Monto)) return false;
        Origen.Registrar(this);
        return true;
    }
    public override string Descripcion() { return $"{Fecha}: Deposito de {Monto:c} en cuenta {Origen.Numero} ({Origen.Cliente.Nombre})"; }
}
class Retiro : Operacion
{
    public Retiro(string numero, decimal monto) : base(numero, monto) { }
    public override bool Ejecutar()
    {
        if (Origen == null || !Origen.Extraer(Monto)) return false;
        Origen.Registrar(this);
        return true;
    }
    public override string Descripcion() { return $"{Fecha}: Retiro de {Monto:c} en cuenta {Origen.Numero} ({Origen.Cliente.Nombre})"; }
}
class Transferencia : Operacion
{
    public Cuenta Destino { get; private set; }
    public Transferencia(string numeroOrigen, string numeroDestino, decimal monto) : base(numeroOrigen, monto)
    {
        Destino = Banco.Buscar(numeroDestino);
    }
    public override bool Ejecutar()
    {
        if (Origen == null || Destino == null || Destino.Numero == Origen.Numero || !Origen.Transferir(Monto, Destino)) return false;
        Origen.Registrar(this);
        Destino.Registrar(this);
        return true;
    }
    public override string Descripcion() { return $"{Fecha}: Transferencia de {Monto:c} de cuenta {Origen.Numero} ({Origen.Cliente.Nombre}) a cuenta {Destino.Numero} ({Destino.Cliente.Nombre})"; }
}
class Pago : Operacion
{
    public Pago(string numero, decimal monto) : base(numero, monto) { }
    public override bool Ejecutar()
    {
        if (Origen == null || !Origen.Pagar(Monto)) return false;
        Origen.Registrar(this);
        return true;
    }
    public override string Descripcion() { return $"{Fecha}: Pago de {Monto:c} en cuenta {Origen.Numero}"; }
}
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
nac.Resumen();
tup.Informe();
tup.Resumen();