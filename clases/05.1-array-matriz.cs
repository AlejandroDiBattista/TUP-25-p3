using static System.Console;

// Implementación de un juego de "Tic Tac Toe" (Tateti) en C#
// para mostrar como usar una matriz de 3x3

// Fichas
var Vacio = "  ";
var X     = "❌";
var O     = "⭕️";

// Tablero (matriz 3x3 o un array bidimensional)
string[,] tablero = new [,] {
    { Vacio, Vacio, Vacio },
    { Vacio, Vacio, Vacio },
    { Vacio, Vacio, Vacio }
};

// Mostrar el tablero
void MostrarTablero(string mensaje = ""){
    Clear();
    WriteLine($"\n{mensaje}\n\n");
    for(int f = 0; f < 3; f++){
        for(int c = 0; c < 3; c++){
            Write(tablero[f, c]);
            if (c < 2) Write(" | ");
        }
        WriteLine();
        if (f < 2) WriteLine("---+----+---");
    }
    WriteLine();
}

// Leer un valor entero
int Leer(string mensaje){
    while (true){
        Write(mensaje);
        var entrada = ReadLine();
        if (int.TryParse(entrada, out int valor)){
            return valor;
        }
    }
}

// Muestra un mensaje y espera una tecla
void Mensaje(string mensaje){
    Write($"\n{mensaje}...");
    ReadKey();
}

// Verifica si el jugador gano
bool Gano(string jugador){
    // Verifica que el jugador ocupe una fila, columna o diagonal

    // Diagonal principal                        
    if (tablero[0, 0] == jugador &&      // X - -
        tablero[1, 1] == jugador &&      // - X - 
        tablero[2, 2] == jugador) {      // - - X
        return true; 
    }

    // Diagonal secundaria
    if (tablero[0, 2] == jugador &&      // - - X
        tablero[1, 1] == jugador &&      // - X -
        tablero[2, 0] == jugador) {      // X - -
        return true; 
    }
    
    // Recorrer por filas 
    for (int f = 0; f < 3; f++){  
        if (tablero[f, 0] == jugador &&  // X - -   - X -   - - X
            tablero[f, 1] == jugador &&  // X - -   - X -   - - X
            tablero[f, 2] == jugador)    // X - -   - X -   - - X
            return true;
    }

    // Recorrer por columnas
    for (int c = 0; c < 3; c++){
        if( tablero[0, c] == jugador &&  // X X X   - - -   - - -
            tablero[1, c] == jugador &&  // - - -   X X X   - - -
            tablero[2, c] == jugador)    // - - -   - - -   X X X
            return true;
    }

    return false;
}

// Verifica si hay empate
bool TableroCompleto(){
    for(int f = 0; f < 3; f++){
        for(int c = 0; c < 3; c++){
            if(tablero[f, c] == Vacio) return false;
        }
    }
    return true;
}

// Juego principal

string jugador = X;
while(true){
    MostrarTablero($"Turno de {jugador}");
    int f = Leer("Fila    (1-3): ");
    int c = Leer("Columna (1-3): ");

    if(f < 1 || f > 3 || c < 1 || c > 3){
        Mensaje("Fila o columna no válida");
        continue;
    }

    if(tablero[f-1, c-1] != Vacio){
        Mensaje("Posición ocupada");
        continue;
    }

    tablero[f-1, c-1] = jugador;

    if(Gano(jugador)){
        MostrarTablero($"🎉 Gano el jugador {jugador}");
        break;
    } 
    
    if(TableroCompleto()){
        MostrarTablero("🤷🏻‍♂️ Empate");
        break;
    }
    
    jugador = (jugador == X) ? O : X; // Cambia de jugador
}