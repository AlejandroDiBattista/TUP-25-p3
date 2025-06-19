using Microsoft.EntityFrameworkCore;

public class ContactsDbContext : DbContext
{
    public DbSet<Contact> Contacts { get; set; }

    public ContactsDbContext(DbContextOptions<ContactsDbContext> options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<Contact>().HasData(
            new Contact { Id = 1, Nombre = "Ana", Apellido = "García", Telefono = "111111111", Email = "ana.garcia@email.com" },
            new Contact { Id = 2, Nombre = "Luis", Apellido = "Pérez", Telefono = "222222222", Email = "luis.perez@email.com" },
            new Contact { Id = 3, Nombre = "María", Apellido = "López", Telefono = "333333333", Email = "maria.lopez@email.com" },
            new Contact { Id = 4, Nombre = "Carlos", Apellido = "Sánchez", Telefono = "444444444", Email = "carlos.sanchez@email.com" },
            new Contact { Id = 5, Nombre = "Lucía", Apellido = "Martínez", Telefono = "555555555", Email = "lucia.martinez@email.com" },
            new Contact { Id = 6, Nombre = "Javier", Apellido = "Fernández", Telefono = "666666666", Email = "javier.fernandez@email.com" },
            new Contact { Id = 7, Nombre = "Sofía", Apellido = "Ruiz", Telefono = "777777777", Email = "sofia.ruiz@email.com" },
            new Contact { Id = 8, Nombre = "Miguel", Apellido = "Torres", Telefono = "888888888", Email = "miguel.torres@email.com" },
            new Contact { Id = 9, Nombre = "Elena", Apellido = "Ramírez", Telefono = "999999999", Email = "elena.ramirez@email.com" },
            new Contact { Id = 10, Nombre = "Diego", Apellido = "Moreno", Telefono = "101010101", Email = "diego.moreno@email.com" }
        );
    }
}
