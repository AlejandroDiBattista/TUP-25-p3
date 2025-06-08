public class Contador
{
    public string Nombre { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    
    public void Incrementar()
    {
        Cantidad++;
    }
}