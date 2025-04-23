// TP2: Sistema de Cuentas Bancarias
//

// Implementar un sistema de cuentas bancarias que permita realizar operaciones como depósitos, retiros, transferencias y pagos.

class Banco{}
class Cliente{
    public string Nombre {get; private set;}
    public List<Cuenta> Cuentas {get; private set;}
    public List<Operacion> HistorialOp{get; private set;}
    
    public Cliente(string nombre){
        Nombre = nombre;
        Cuentas = new List<Operacion>();
        HistorialOp = new List<Operacion>();
    }

    public void AgregarCuenta(Cuenta cuenta){
        Cuentas.Add(cuenta);
    }

    public Cuenta ObtenerCuenta(string cuenta){
        return Cuentas.FirstOrDefault(c => c.Numero == Numero);           //FirstOrDefault: me va a buscar el primer elemento que cumpla con la condición, en caso de no encontrar me va a devolver null
                                                                          //c => c.Numero == Numero: es una funcion flecha. Va a traer la cuenta c donde el numero de cuenta sea igual al que le pasaron como parametro
    }

    public void RegistrarOp(Operacion op){
        HistorialOp.Add(op);
    }

    public void Informe(){
        Console.WriteLine($".... Informe de {Nombre} ....");
        Console.WriteLine("Cuentas:");
        foreach (var cuenta in Cuentas){
            Console.WriteLine($"Cuenta {cuenta.Numero} .. Saldo: {cuenta.Saldo} .. Puntos: {cuenta.Puntos}");

        }
        Console.WriteLine("Historial de Operaciones ;):")
            foreach(var op in HistorialOp){
                Console.WriteLine(op.Descripcion());
            }

            Console.WriteLine();
    }
}

abstract class Cuenta{
    public string Numero {get; set;}
    public decimal Saldo {get; protected set;}   //protected sirve solo para que la propia clase o una que hereda de Cuenta pueda modificar el saldo
    public int Puntos {get; protected set;}

    public Cuenta(string numero){
        Numero = numero;
        Saldo = 0;
        Puntos = 0;
    }

    public virtual void Depositar(decimal monto){       //virtual quiere decir que el metodo puede ser modificado en una clase hija
        Saldo += monto;
    }

    public virtual bool Extraer(decimal monto){
        if(Saldo >= monto){
            Saldo -= monto;
            return true;
        }
        return false;
    }
    public abstract void Pagar(decimal monto);
}
class CuentaOro: Cuenta{
    public CuentaOro(string numero) : base(numero) {}

    public override void Pagar(decimal monto){
        if (Extraer(monto)){
            int puntosGanados = monto > 1000 ? (int)(monto*0.05m) : (int)(monto*0.03m);
            Puntos += puntosGanados;
        }
    }
}
class CuentaPlata: Cuenta{
    public CuentaPlata(string numero) : base(numero) {}

    public override void Pagar(decimal monto){
        if (Extraer(monto)){
            int puntosGanados= (int)(monto*0.02m);
            Puntos += puntosGanados;
        }
    }
}
class CuentaBronce: Cuenta{
    public CuentaBronce(string numero) : base(numero) {}

    public override void Pagar(decimal monto){
        if(Extraer(monto)){
            int puntosGanados = (int)(monto*0.01m);
            Puntos += puntosGanados;
        }
    }
}

abstract class Operacion{
    public DateTime Fecha {get; private set;}
    public decimal Monto {get; private set;}

    public Operacion(decimal monto){
        Fecha = DateTime.Now;
        Monto = monto;
    }

    public abstract void Ejecutar();
    public abstract string Descripcion();
}
class Deposito: Operacion{
    private Cuenta cuenta;

    public Deposito(Cuenta cuenta, decimal monto) : base(monto){
        this.cuenta = cuenta;
    }

    public override void Ejecutar(){
        cuenta.Depositar(Monto);
    }

    public override string Descripcion(){
        return $"[DEPOSITO] {Fecha}: +{Monto} en cuenta {cuenta.Numero}";
    }
}
class Retiro: Operacion{
    private Cuenta cuenta;

    public Retiro(Cuenta cuenta, decimal monto) : base(monto){
        this.cuenta = cuenta;
    }

    public override void Ejecutar(){
        if(!cuenta.Extraer(Monto)){
            throw new InvaliOperationException("No tienes dinero suficiente para retirar. :()");
        }
    }

    public override string Descripcion(){
        return $"[RETIRO] {Fecha}: ..{Monto} de cuenta {cuenta.Numero}";
    }
}
class Transferencia: Operacion{
    private Cuenta origen;
    private Cuenta destino

    public Transferencia(Cuenta origen, Cuenta destino, decimal monto) : base(monto){
        this.origen = origen;
        this.destino = destino;
    }

    public override void Ejecutar(){
        if(!origen.Extraer(Monto)){
            throw new InvaliOperationException("No tienes ese dinero para transferir.");
        }
        destino.Depositar(Monto);
    }

    public override string Descripcion(){
        return $"[TRANSFERENCIA] {fecha}: ..{Monto} de {origen.Numero} a {destino.Numero}";
    }
}
class Pago: Operacion{
    private Cuenta Cuenta;

    public Pago(Cuenta cuenta, decimal monto) : base(monto){
        this.Cuenta = cuenta;
    }

    public override void Ejecutar(){
        cuenta.Pagar(Monto);
    }

    public override string Descripcion(){
        return $"[PAGO] {Fecha}: ..{Monto} de cuenta {cuenta.Numero}";
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

