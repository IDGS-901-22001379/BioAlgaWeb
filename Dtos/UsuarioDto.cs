// Dtos/UsuarioDto.cs
namespace BioAlga.Backend.Dtos
{
    /// <summary>
    /// DTO para exponer información de usuario sin datos sensibles.
    /// </summary>
    public class UsuarioDto
    {
        public int Id_Usuario { get; set; }

        public string Nombre_Usuario { get; set; } = string.Empty;

        // Nombre del rol asociado (tabla roles)
        public string Rol { get; set; } = string.Empty;

        // Id del rol (por si se necesita en el frontend)
        public int Id_Rol { get; set; }

        // Datos opcionales del empleado asignado
        public int? Id_Empleado { get; set; }
        public string? Nombre_Empleado { get; set; }
        public string? Apellido_Paterno { get; set; }
        public string? Apellido_Materno { get; set; }

        // Estado de la cuenta
        public bool Activo { get; set; }

        // Último acceso
        public DateTime? Ultimo_Login { get; set; }

        // Fecha de registro del usuario
        public DateTime Fecha_Registro { get; set; }
    }
}
