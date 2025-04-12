namespace TUP;

public class EstadoPractico {
    public static readonly EstadoPractico NoPresentado = new(" ");
    public static readonly EstadoPractico Aprobado = new("+");
    public static readonly EstadoPractico Desaprobado = new("-");
    public static readonly EstadoPractico Error = new("*");
    public static readonly EstadoPractico EnProgreso = new("~");
        
    public static EstadoPractico FromString(string c) => c switch {
        "." or " " => NoPresentado,
        "+" => Aprobado,
        "-" => Desaprobado,
        "*" => Error,
        "~" => EnProgreso,
        _ => Error
    };
    
    public ConsoleColor Color => _value switch {
        "." or " " => ConsoleColor.White,
        "+" => ConsoleColor.Green,
        "-" => ConsoleColor.Yellow,
        "*" => ConsoleColor.Red,
        "~" => ConsoleColor.Cyan,
        _ => ConsoleColor.White
    };

    public string Emoji => _value switch {
        "." or " " => "❓",
        "+" => "✅",
        "-" => "❌",
        "*" => "⚠️",
        "~" => "⏳",
        _ => "❓"
    };

    private readonly string _value;
    
    private EstadoPractico(string value) {
        _value = value;
    }
    
    public override string ToString() => _value.ToString();
}