class Punto {
    public int X {get; set;}
    public int Y {get; set;}

    public Punto(int x, int y) {
        X = x;
        Y = y;
    }
    public string ToString() {
        return $"({X}, {Y})";
    }

    public int DistanciaAlOrigen() {
        return (int)Math.Sqrt(X*X + Y*Y);
    }
}

record Punto(int X, int Y){
    public int DistanciaAlOrigen() {
        return (int)Math.Sqrt(X*X + Y*Y);
    }
};
var a = new Punto(1, 2);
WriteLine(a);