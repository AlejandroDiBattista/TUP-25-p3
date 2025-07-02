
// Definimos el servicio ContadoresServicio que maneja una lista de contadores y un reloj.
public class ContadoresServicio
{
    public List<Contador> Contadores { get; set; } = new List<Contador>();
    public DateTime HoraInicio { get; set; } = DateTime.Now;
    public DateTime HoraFin { get; set; } = DateTime.Now;

    public void Agregar(Contador contador) => Contadores.Add(contador);
    public void Eliminar(Contador contador) => Contadores.Remove(contador);
    public void IniciarReloj() => HoraInicio = DateTime.Now;
    public void ActualizarReloj() => HoraFin = DateTime.Now;

    public int SegundosTranscurridos => (int)(HoraFin - HoraInicio).TotalSeconds;
    public int TiempoFaltante => 600 - SegundosTranscurridos;
}