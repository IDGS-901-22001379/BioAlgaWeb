namespace BioAlga.Backend.Dtos
{
    public class ActualizarUsuarioDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string? ApellidoPaterno { get; set; }
        public string? ApellidoMaterno { get; set; }
        public string? Correo { get; set; }
        public string? Telefono { get; set; }
        public string? Estatus { get; set; }
        // Si quieres permitir cambio de contraseña:
        public string? NuevaContrasena { get; set; }
    }
}
