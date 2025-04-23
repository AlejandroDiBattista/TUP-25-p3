using System;
using System.IO;

public class Agenda{
	static bool running = true;

	struct Contacto {
		public int ID;
		public String Nombre;
		public long Telefono;
		public String Email;
	};

	const int MAX_CANT_CONTACTOS = 10;
	static Contacto[] Contactos = new Contacto[ MAX_CANT_CONTACTOS ];
	static int cant_contactos = 0;
	static int next_ID = 1;
	static string csv_path = "agenda.csv";


	public static void Pausa(){
		Console.WriteLine( "Presione cualquier tecla para continuar..." );
		Console.ReadKey( true );
		Console.WriteLine();
	}
		
	public static void Agregar_Contacto(){
		Contacto nuevo = new Contacto();
		bool aceptado = false;

		Console.WriteLine( "=== Agregar Contacto ===" );
		if( cant_contactos < MAX_CANT_CONTACTOS ){
			Console.Write( "Nombre   : " );
				nuevo.Nombre = Console.ReadLine();
			do {
				Console.Write( "Teléfono : " );
					aceptado = long.TryParse( Console.ReadLine(), out nuevo.Telefono );
					if( !aceptado )
						Console.WriteLine( "\t¡Valor incorrecto para teléfono! Ingrese solamente dígitos." );
			} while( !aceptado );
			Console.Write( "Email    : " );
				nuevo.Email = Console.ReadLine();
				
			nuevo.ID = next_ID++;
			Contactos[ cant_contactos++ ] = nuevo;
			Console.WriteLine( "Contacto agregado con ID = {0}", nuevo.ID );	
		} else {
			Console.WriteLine( "No hay lugar para más contactos." );	
			Console.WriteLine();
		}
		
		Pausa();
	}

	public static void Modificar_Contacto(){
		int ID_modificando = 0;
		int index;
		string linea;
		bool aceptado = false;

		Console.WriteLine( "=== Modificar Contacto ===" );
		if( cant_contactos > 0 ){
			do {
				Console.Write( "Ingrese el ID del contacto a modificar: " );
				aceptado = int.TryParse( Console.ReadLine(), out ID_modificando );
				if( !aceptado )
					Console.WriteLine( "\t¡Valor incorrecto para ID! Ingrese solamente dígitos." );
			} while( !aceptado );

			/* Busca el contacto por ID */
				for( index = 0; index < cant_contactos && Contactos[ index ].ID != ID_modificando; ++index );

			if( index < cant_contactos ){
				Console.WriteLine( "Datos actuales => Nombre: {0}, Teléfono : {1}, Email: {2}", Contactos[ index ].Nombre, Contactos[ index ].Telefono, Contactos[ index ].Email );
				Console.WriteLine( "(Deje el campo en blanco para no modificar)" );
				Console.WriteLine();
				Console.Write( "Nombre   : " );
					linea = Console.ReadLine();
					if( linea.Length > 0 ) Contactos[ index ].Nombre = linea;
				do {
					Console.Write( "Teléfono : " );
						linea = Console.ReadLine();
						aceptado = true;
						if( linea.Length > 0 ){
							aceptado = long.TryParse( linea, out Contactos[ index ].Telefono );
							if( !aceptado )
								Console.WriteLine( "\t¡Valor incorrecto para teléfono! Ingrese solamente dígitos." );
						};
				} while( !aceptado );
				Console.Write( "Email    : " );
					linea = Console.ReadLine();
					if( linea.Length > 0 ) Contactos[ index ].Email = linea;
				Console.WriteLine();
				Console.WriteLine( "Contacto modificado con éxito." );
			} else {
				Console.WriteLine( "No se encontró contacto con ID: {0}", ID_modificando );
				Console.WriteLine();
			};
		} else {
			Console.WriteLine( "No hay contactos para modificar." );	
			Console.WriteLine();
		}

		Pausa();
	}

	public static void Borrar_Contacto(){
		int ID_borrando = 0;
		int index;
		bool aceptado = false;

		Console.WriteLine( "=== Borrar Contacto ===" );
		if( cant_contactos > 0 ){
			do {
				Console.Write( "Ingrese el ID del contacto a borrar: " );
				aceptado = int.TryParse( Console.ReadLine(), out ID_borrando );
				if( !aceptado )
					Console.WriteLine( "\t¡Valor incorrecto para ID! Ingrese solamente dígitos." );
			} while( !aceptado );

			/* Busca el contacto por ID */
				for( index = 0; index < cant_contactos && Contactos[ index ].ID != ID_borrando; ++index );

			if( index < cant_contactos ){
				--cant_contactos;
				for( ; index < cant_contactos; ++index )
					Contactos[ index ] = Contactos[ index + 1 ];
					
				Console.WriteLine( "Contacto con ID={0} eliminado con éxito.", ID_borrando );
			} else {
				Console.WriteLine( "No se encontró contacto con ID: {0}", ID_borrando );
				Console.WriteLine();
			};
		} else {
			Console.WriteLine( "No hay contactos para borrar." );	
			Console.WriteLine();
		}

		Pausa();
	}

	public static void Listar_Contactos(){
		Console.WriteLine( "=== Lista de Contactos ===" );
		Console.WriteLine( "{0,-5}\t{1,-50}\t{2,-15}\t{3,-50}", "ID", "NOMBRE", "TELÉFONO", "EMAIL" );
		for( int c = 0; c < cant_contactos; ++c )
			Console.WriteLine( "{0,-5}\t{1,-50}\t{2,-15}\t{3,-50}"
			, Contactos[ c ].ID
			, Contactos[ c ].Nombre.Substring( 0, Math.Min( Contactos[ c ].Nombre.Length, 50 ) )
			, Contactos[ c ].Telefono
			, Contactos[ c ].Email.Substring(  0, Math.Min( Contactos[ c ].Email.Length,  50 ) )
			);

		Pausa();
	}

	public static void Buscar_Contacto(){
		string termino;

		Console.WriteLine( "=== Buscar Contacto ===" );
		if( cant_contactos > 0 ){
			Console.Write( "Ingrese un término de búsqueda (nombre, teléfono o email): " );
			termino = Console.ReadLine();
			if( termino.Length > 0 ){
				Console.WriteLine();
				Console.WriteLine( "Resultados de la búsqueda:" );
				Console.WriteLine( "{0,-5}\t{1,-50}\t{2,-15}\t{3,-50}", "ID", "NOMBRE", "TELÉFONO", "EMAIL" );
				for( int c = 0; c < cant_contactos; ++c )
					if( Contactos[ c ].Nombre.IndexOf( termino ) > -1
					 || Contactos[ c ].Telefono.ToString().IndexOf( termino ) > -1
					 || Contactos[ c ].Email.IndexOf( termino ) > -1
					)
						Console.WriteLine( "{0,-5}\t{1,-50}\t{2,-15}\t{3,-50}"
						, Contactos[ c ].ID
						, Contactos[ c ].Nombre.Substring( 0, Math.Min( Contactos[ c ].Nombre.Length, 50 ) )
						, Contactos[ c ].Telefono
						, Contactos[ c ].Email.Substring(  0, Math.Min( Contactos[ c ].Email.Length,  50 ) )
						);
			} else {
				Console.WriteLine();
				Console.WriteLine( "No hay término de búsqueda. Sin resultados." );
			};
			Console.WriteLine();
		} else {
			Console.WriteLine( "No hay contactos para buscar." );	
			Console.WriteLine();
		}

		Pausa();
	}

	public static void Salir(){
		Console.WriteLine( "Saliendo de la aplicación..." );
		running = false;
	}


	public static void Main(){
		char opcion;

		/* Inicialización */
			for( int c = 0; c < MAX_CANT_CONTACTOS; ++c )
				Contactos[ c ] = new Contacto(){ ID = 0, Nombre = "", Telefono = 0, Email = "" };

		/* Lee la agenda desde el archivo. */
			if( File.Exists( csv_path ) )
				using( StreamReader reader = new StreamReader( csv_path ) ){
					string line;
					
					reader.ReadLine(); /* Header */
					while( ( line = reader.ReadLine() ) != null ){
						int from = 0, pos = 0;

						if(  from < line.Length  &&  ( pos = line.IndexOf( ',', from ) ) > -1  ){
							if( ! int.TryParse( line.Substring( from, pos - from ), out Contactos[ cant_contactos ].ID ) )
								Contactos[ cant_contactos ].ID = next_ID++;
							from = pos + 1;
						};
						if(  from < line.Length  &&  ( pos = line.IndexOf( ',', from ) ) > -1  ){
							Contactos[ cant_contactos ].Nombre = line.Substring( from, pos - from );
							from = pos + 1;
						};
						if(  from < line.Length  &&  ( pos = line.IndexOf( ',', from ) ) > -1  ){
							if( ! long.TryParse( line.Substring( from, pos - from ), out Contactos[ cant_contactos ].Telefono ) )
								Contactos[ cant_contactos ].Telefono = 0;
							from = pos + 1;
						};
						if(  from < line.Length  ){
							Contactos[ cant_contactos ].Email = line.Substring( from );
						};
						
						next_ID = Math.Max( next_ID, Contactos[ cant_contactos ].ID + 1 );
						++cant_contactos;
					};
				};		

		/* Ciclo principal. */			
			do{
				Console.WriteLine( "===== AGENDA DE CONTACTOS =====" );
				Console.WriteLine( "1) Agregar contacto"   );
				Console.WriteLine( "2) Modificar contacto" );
				Console.WriteLine( "3) Borrar contacto"    );
				Console.WriteLine( "4) Listar contactos"   );
				Console.WriteLine( "5) Buscar contacto"    );
				Console.WriteLine( "0) Salir"              );
				Console.WriteLine();
				Console.Write( "Seleccione una opción: " );
				 opcion = Convert.ToChar( Console.Read() );
				 Console.WriteLine();
				
				Console.WriteLine();
				switch( opcion ){
					case '1' : Agregar_Contacto();   break;
					case '2' : Modificar_Contacto(); break;
					case '3' : Borrar_Contacto();    break;
					case '4' : Listar_Contactos();   break;
					case '5' : Buscar_Contacto();    break;
					case '0' : Salir();              break;
					default :
						Console.WriteLine( "¡Opción incorrecta!" );
						Console.WriteLine();
						break;
				};
			} while( running );
			
		/* Escribe -o sobrescribe- el archivo de agenda. */ 
			using( StreamWriter writer = new StreamWriter( csv_path, false ) ){
				writer.WriteLine( "ID,NOMBRE,TELÉFONO,EMAIL" );
				for( int c = 0; c < cant_contactos; ++c )
					writer.WriteLine( "{0},{1},{2},{3}", Contactos[ c ].ID, Contactos[ c ].Nombre, Contactos[ c ].Telefono, Contactos[ c ].Email );
			};		
	}
}
