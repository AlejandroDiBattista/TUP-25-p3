class Agenda {
    static int ProximoId = 1; // El primer ID que se asignará a un nuevo contacto
    public List<Contacto> Contactos { get; init; } = new();
    
    public void Registrar(Contacto contacto) {
        if (contacto.EsNuevo){ // Si es nuevo lo agregamos
            // Asignar un nuevo ID al contacto
            contacto.Id = ProximoId++;
            Contactos.Add(contacto);
        } else { // Si no es nuevo, buscamos el contacto existente
            var anterior = Contactos.Find(c => c.Id == contacto.Id);
            if (anterior is not null) {
                // Actualizar los datos del contacto existente
                anterior.Nombre   = contacto.Nombre;
                anterior.Apellido = contacto.Apellido;
                anterior.Telefono = contacto.Telefono;
            }
        }
    }

    public void Borrar(Contacto contacto) {
        var anterior = Contactos.Find(c => c.Id == contacto.Id);
        if (anterior is not null) {
            Contactos.Remove(anterior);
        }
    }
}