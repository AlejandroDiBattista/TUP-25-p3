using Cliente.Models;

namespace Cliente.Services;

public class ValidationService
{
    public static class Producto
    {
        public static bool ValidarStock(int cantidad, int stockDisponible)
        {
            return cantidad > 0 && cantidad <= stockDisponible;
        }
        
        public static bool ValidarCantidad(int cantidad)
        {
            return cantidad >= 1;
        }
        
        public static string? ValidarPrecio(decimal precio)
        {
            if (precio <= 0)
                return "El precio debe ser mayor a cero";
            
            if (precio > 999999)
                return "El precio es demasiado alto";
                
            return null;
        }
    }
    
    public static class Cliente
    {
        public static string? ValidarNombre(string? nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                return "El nombre es requerido";
                
            if (nombre.Length < 2)
                return "El nombre debe tener al menos 2 caracteres";
                
            if (nombre.Length > 50)
                return "El nombre no puede tener más de 50 caracteres";
                
            return null;
        }
        
        public static string? ValidarEmail(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return "El email es requerido";
                
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                if (addr.Address != email)
                    return "Formato de email inválido";
            }
            catch
            {
                return "Formato de email inválido";
            }
            
            return null;
        }
    }
    
    public static class Busqueda
    {
        public static string? ValidarTerminoBusqueda(string? termino)
        {
            if (!string.IsNullOrWhiteSpace(termino) && termino.Length < 2)
                return "El término de búsqueda debe tener al menos 2 caracteres";
                
            if (!string.IsNullOrWhiteSpace(termino) && termino.Length > 100)
                return "El término de búsqueda es demasiado largo";
                
            return null;
        }
    }
}
