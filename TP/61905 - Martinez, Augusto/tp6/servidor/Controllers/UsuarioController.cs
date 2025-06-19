using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using servidor.Data;
using servidor.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

[Route("api/usuarios")]
[ApiController]
public class UsuariosController : ControllerBase
{
    private readonly AppDbContext _context;

    public UsuariosController(AppDbContext context)
    {
        _context = context;
    }

    // ✅ 1️⃣ Obtener todos los usuarios
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Usuario>>> ObtenerUsuarios()
    {   
        Console.WriteLine("🔹 Se está ejecutando ObtenerUsuarios()"); // 🚀 Confirma que la función se está llamando
    
        var usuarios = await _context.Usuarios.AsNoTracking().ToListAsync();
        Console.WriteLine($"🔹 Usuarios encontrados en la BD: {usuarios.Count}"); // ✅ Verifica si hay usuarios

        if (usuarios.Count == 0) 
            return NotFound("No hay usuarios en la base de datos.");

        return Ok(usuarios);
    }
   


    // ✅ 2️⃣ Obtener un usuario por ID
    [HttpGet("{id}")]
    public async Task<ActionResult<Usuario>> ObtenerUsuario(int id)
    {
        var usuario = await _context.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
        if (usuario == null) return NotFound($"Usuario con ID {id} no encontrado."); // 🔥 Mensaje más descriptivo
        return Ok(usuario);
    }

    // ✅ 3️⃣ Crear un nuevo usuario (Verifica email duplicado)
    [HttpPost]
    public async Task<ActionResult<Usuario>> CrearUsuario([FromBody] Usuario usuario)
    {
        if (usuario == null || string.IsNullOrEmpty(usuario.Email)) return BadRequest("Datos inválidos.");

        // Verificar si el email ya existe
        var existeUsuario = await _context.Usuarios.AnyAsync(u => u.Email == usuario.Email);
        if (existeUsuario) return Conflict("Ya existe un usuario con este email.");

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(ObtenerUsuario), new { id = usuario.Id }, usuario);
    }

    // ✅ 4️⃣ Actualizar usuario (Validación de datos)
    [HttpPut("{id}")]
    public async Task<IActionResult> ActualizarUsuario(int id, [FromBody] Usuario usuarioActualizado)
    {
        if (id != usuarioActualizado.Id) return BadRequest("IDs no coinciden.");
        if (string.IsNullOrEmpty(usuarioActualizado.Nombre) || string.IsNullOrEmpty(usuarioActualizado.Email))
            return BadRequest("Nombre y Email son obligatorios.");

        var usuarioExistente = await _context.Usuarios.FindAsync(id);
        if (usuarioExistente == null) return NotFound("Usuario no encontrado.");

        usuarioExistente.Nombre = usuarioActualizado.Nombre;
        usuarioExistente.Email = usuarioActualizado.Email;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // ✅ 5️⃣ Eliminar usuario
    [HttpDelete("{id}")]
    public async Task<IActionResult> EliminarUsuario(int id)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null) return NotFound("Usuario no encontrado.");

        _context.Usuarios.Remove(usuario);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
