using System;

public class SistemaBancario {
	/* Clientes */
		class Cliente {
			public string Nombre { get => nombre; }
				protected string nombre;
			Cuenta[] cuentas;

			public Cliente( string nombre = null ){
				this.nombre = nombre;
				this.cuentas = new Cuenta[0];
			}

			public Cuenta Agregar( Cuenta nueva ){
				Array.Resize( ref cuentas, cuentas.Length + 1 );
				nueva.AsignarTitular( this );
				cuentas[ cuentas.Length - 1 ] = nueva;
			return nueva; }
			
			public Cuenta Buscar( long numero ){
				Cuenta encontrada = null;
				for( int c = 0; c < cuentas.Length && encontrada == null; ++c )
					if( cuentas[ c ].Numero == numero ) encontrada = cuentas[ c ];
			return encontrada; }

			public Cuenta Buscar( Cuenta buscada ){
				Cuenta encontrada = null;
				for( int c = 0; c < cuentas.Length && encontrada == null; ++c )
					if( cuentas[ c ] == buscada ) encontrada = cuentas[ c ];
			return encontrada; }

			public void Informe(){
				double saldo_total = 0.0;
				double puntos_total = 0.0;
				
				for( int c = 0; c < cuentas.Length; ++c ) saldo_total  += cuentas[ c ].Saldo;
				for( int c = 0; c < cuentas.Length; ++c ) puntos_total += cuentas[ c ].Puntos;

				Console.WriteLine( "\tCliente: {0} | Saldo Total: {1,9:$#####0.00} | Puntos Total: {2,9:$#####0.00}", nombre, saldo_total, puntos_total );
				Console.WriteLine();
				for( int c = 0; c < cuentas.Length; ++c )
					cuentas[ c ].Informe();
			}
		}

	/* Cuentas */
		abstract class Cuenta {
			static long numero_siguiente = 10001;

			public long Numero { get => numero; }
				private long numero;
			public Cliente Titular { get => titular; }
				private Cliente titular;
			public double Saldo { get => saldo; }
				private double saldo;
			public double Puntos { get => puntos; }
				private double puntos;
			Operacion[] operaciones;

			public Cuenta( double saldo_inicial = 0.0 ) {
				titular = null;
				numero = numero_siguiente++;
				saldo = saldo_inicial;
				puntos = 0;
				operaciones = new Operacion[0];
			}
			
			public void AsignarTitular( Cliente titular ){  this.titular = titular;  }

			abstract public double ObtenerPuntos( double monto );

			public double Depositar( double monto ) {
				saldo += monto;
			return monto; }

			public double Retirar( double monto ) { /* Devuelve el monto retirado. */
				monto = Math.Min( monto, saldo );
				saldo -= monto;
			return monto; }

			public double Pagar( double monto ) { /* Devuelve el monto pagado. */
				monto = Retirar( monto );
				puntos += ObtenerPuntos( monto );
			return monto; }

			public Operacion Registrar( Operacion operacion ){
				Array.Resize( ref operaciones, operaciones.Length + 1 );
				operaciones[ operaciones.Length - 1 ] = operacion;
			return operacion; }

			public void Informe(){
				Console.WriteLine( "\t\tCuenta: {0} | Saldo: {1,9:$#####0.00} | Puntos: {2,9:$#####0.00}", numero, saldo, puntos );
				for( int o = 0; o < operaciones.Length; ++o )
					operaciones[ o ].Informe( this );
				Console.WriteLine();
			}
		}

		class CuentaOro : Cuenta {
			public CuentaOro( double saldo_inicial = 0.0 ) : base( saldo_inicial ) {}
			override public double ObtenerPuntos( double monto ) {  return monto * ( monto >= 1000 ? 0.05 : 0.03 );  }
		}

		class CuentaPlata : Cuenta {
			public CuentaPlata( double saldo_inicial = 0.0 ) : base( saldo_inicial ) {}
			override public double ObtenerPuntos( double monto ) {  return monto * 0.02;  }
		}

		class CuentaBronce : Cuenta {
			public CuentaBronce( double saldo_inicial = 0.0 ) : base( saldo_inicial ) {}
			override public double ObtenerPuntos( double monto ) {  return monto * 0.01;  }
		}

	/* Operaciones */
		abstract class Operacion {
			static long numero_siguiente = 1;
			static Operacion[] operaciones = new Operacion[0]; /* Registra TODAS las operaciones */

			protected long numero;
			public Cuenta Cuenta { get => cuenta; }
				protected Cuenta cuenta;
			protected double monto;

			protected Operacion(){
				numero = 0;
				cuenta = null;
				monto  = Double.NaN;
			}

			public Operacion( Cuenta cuenta, double monto ){
				this.numero = numero_siguiente++;
				this.cuenta = cuenta;
				this.monto  = monto;

				Array.Resize( ref operaciones, operaciones.Length + 1 );
				operaciones[ operaciones.Length - 1 ] = this;
			}

			public virtual bool Involucra( Cuenta cual ){  return cuenta == cual;  }

			public virtual void Registrar(){
				cuenta.Registrar( this );
			}

			public abstract void Operar();

			public abstract void Informe( Cuenta cta );
			
			protected abstract void Informe();
			public static void InformeGlobal() {
				Console.WriteLine( "Informe Global de Operaciones" );
				Console.WriteLine( "{0,10}\t{1,10}\t{2,-15}\t{3,11}\t{4}", "Nº Operac.", "Cuenta", "Operación", "Monto", "Otros" );
				for( int o = 0; o < operaciones.Length; ++o )
					operaciones[ o ].Informe();
				Console.WriteLine();
			}
		}

		class Deposito : Operacion {
			public Deposito( Cuenta cuenta, double monto ) : base( cuenta, monto ) {}
			public override void Operar() {  monto = cuenta.Depositar( monto );  }
			public override void Informe( Cuenta cta ){
				Console.WriteLine( "\t\t\t{0,10:D}\t{1,-15}\t{2,11:+$#####0.00}", numero, "Depósito", monto );
			}
			protected override void Informe(){
				Console.WriteLine( "{0,10:D}\t{1,10:D}\t{2,-15}\t{3,11:+$#####0.00}", numero, cuenta.Numero, "Depósito", monto );
			}
		}

		class Retiro : Operacion {
			public Retiro( Cuenta cuenta, double monto ) : base( cuenta, monto ) {}
			override public void Operar() {  monto = cuenta.Retirar( monto );  }
			public override void Informe( Cuenta cta ){
				Console.WriteLine( "\t\t\t{0,10:D}\t{1,-15}\t{2,11:-$#####0.00}", numero, "Retiro", monto );
			}
			protected override void Informe(){
				Console.WriteLine( "{0,10:D}\t{1,10:D}\t{2,-15}\t{3,11:-$#####0.00}", numero, cuenta.Numero, "Retiro", monto );
			}
		}

		class Pago : Operacion {
			public Pago( Cuenta cuenta, double monto ) : base( cuenta, monto ) {}
			override public void Operar() {  monto = cuenta.Pagar( monto );  }
			public override void Informe( Cuenta cta ){
				Console.WriteLine( "\t\t\t{0,10:D}\t{1,-15}\t{2,11:-$#####0.00}", numero, "Pago", monto );
			}
			protected override void Informe(){
				Console.WriteLine( "{0,10:D}\t{1,10:D}\t{2,-15}\t{3,11:-$#####0.00}", numero, cuenta.Numero, "Pago", monto );
			}
		}

		class Transferencia : Operacion {
			Cuenta destino;

			public Transferencia() : base() { destino = null; }
			public Transferencia( Cuenta cuenta, double monto ) : this() {}
			public Transferencia( Cuenta cuenta, Cuenta destino, double monto ) : base( cuenta, monto ) {
				this.destino = destino;
			}

			public override bool Involucra( Cuenta cual ){  return base.Involucra( cual ) || ( destino == cual );  }

			public override void Registrar(){
				base.Registrar();
				destino.Registrar( this );
			}

			public override void Operar() {
				if( cuenta != destino ){
					monto = destino.Depositar( cuenta.Retirar( monto ) );
				} else {
					monto = 0;
				};
			}
			
			public override void Informe( Cuenta informando ){
				if( informando == cuenta )
					Console.WriteLine( "\t\t\t{0,10:D}\t{1,-15}\t{2,11:-$#####0.00}\tHacia {3}/{4}", numero, "Transferencia", monto, destino.Numero, destino.Titular.Nombre );
				else
					Console.WriteLine( "\t\t\t{0,10:D}\t{1,-15}\t{2,11:+$#####0.00}\tDesde {3}/{4}", numero, "Transferencia", monto, cuenta.Numero,  cuenta.Titular.Nombre  );
			}

			protected override void Informe(){
				Console.WriteLine( "{0,10:D}\t{1,10:D}\t{2,-15}\t{3,11:-$#####0.00}\tHacia {4,10:D}", numero, cuenta.Numero, "Transferencia", monto, destino.Numero );
			}
		}

	/* Bancos */
		class Banco {
			string nombre;
			Cliente[] clientes;

			public Banco( string nombre = null ){
				this.nombre = nombre;
				this.clientes = new Cliente[0];
			}

			public Cliente Agregar( Cliente nuevo ){
				Array.Resize( ref clientes, clientes.Length + 1 );
				clientes[ clientes.Length - 1 ] = nuevo;
			return nuevo; }

			public Cuenta BuscarCuenta( long numero ){
				Cuenta encontrada = null;
				for( int c = 0; c < clientes.Length && encontrada == null; ++c )
					encontrada = clientes[ c ].Buscar( numero );
			return encontrada; }
			
			public Cuenta BuscarCuenta( Cuenta buscada ){
				Cuenta encontrada = null;
				for( int c = 0; c < clientes.Length && encontrada == null; ++c )
					encontrada = clientes[ c ].Buscar( buscada );
			return encontrada; }
			
			public Operacion Registrar( Operacion operacion ){
				if( BuscarCuenta( operacion.Cuenta ) != null ){
					operacion.Registrar();
					operacion.Operar();
				};
			return operacion; }

			public void Informe(){
				Console.WriteLine( "Banco: {0} | Clientes: {1}", nombre, clientes.Length );
				Console.WriteLine();
				for( int c = 0; c < clientes.Length; ++c )
					clientes[ c ].Informe();
			}
		}


	public static void Main() {
		var nac = new Banco( "Banco Nac" );
			var raul = nac.Agregar( new Cliente( "Raúl Perez" ) );
				raul.Agregar( new CuentaOro( 1000 ) );   /* Cuenta Nº10001 */
				raul.Agregar( new CuentaPlata( 2000 ) ); /* Cuenta Nº10002 */
			var sara = nac.Agregar( new Cliente( "Sara López" ) );
				sara.Agregar( new CuentaPlata( 3000 ) ); /* Cuenta Nº10003 */
				sara.Agregar( new CuentaPlata( 4000 ) ); /* Cuenta Nº10004 */
		var tup = new Banco( "Banco TUP" );
			var luis = tup.Agregar( new Cliente( "Luís Gomez" ) );
				luis.Agregar( new CuentaBronce( 5000 ) ); /* Cuenta Nº10005 */

		nac.Registrar( new Deposito( nac.BuscarCuenta( 10001 ), 100.00 ) );
		nac.Registrar( new Retiro( nac.BuscarCuenta( 10002 ), 200.00 ) );
		nac.Registrar( new Transferencia( nac.BuscarCuenta( 10001 ), nac.BuscarCuenta( 10002 ), 300.00 ) );
		nac.Registrar( new Transferencia( nac.BuscarCuenta( 10003 ), nac.BuscarCuenta( 10004 ), 500.00 ) );
		nac.Registrar( new Pago( nac.BuscarCuenta( 10002 ), 400.00 ) );

		tup.Registrar( new Deposito( tup.BuscarCuenta( 10005 ), 100.00 ) );
		tup.Registrar( new Retiro( tup.BuscarCuenta( 10005 ), 200.00 ) );
		tup.Registrar( new Transferencia( tup.BuscarCuenta( 10005 ), nac.BuscarCuenta( 10002 ), 300.00 ) );
		tup.Registrar( new Pago( tup.BuscarCuenta( 10005 ), 400.00 ) );
		
		nac.Informe();
		tup.Informe();
		
		Console.WriteLine();
		Operacion.InformeGlobal();
	}
}


