public interface IContactoServicio {
    Task<List<Contacto>> TraerTodosAsync();
    Task<Contacto> TraerPorIdAsync(int id);
    Task<Contacto> CrearAsync(Contacto contacto);
    Task<Contacto> ActualizarAsync(Contacto contacto);
    Task<bool> EliminarAsync(int id);   
}

