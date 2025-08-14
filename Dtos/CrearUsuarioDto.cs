namespace BioAlga.Backend.Dtos
{
    public class CrearUsuarioDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string? ApellidoPaterno { get; set; }
        public string? ApellidoMaterno { get; set; }
        public string? Correo { get; set; }
        public string? Telefono { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Contrasena { get; set; } = string.Empty;
        public string Estatus { get; set; } = "Activo";
    }
}
