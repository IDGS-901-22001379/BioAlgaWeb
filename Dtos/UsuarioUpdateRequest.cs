namespace BioAlga.Backend.Dtos
{
    public class UsuarioUpdateRequest
    {
        public string? Nombre_Usuario { get; set; }
        public string? Contrasena { get; set; }
        public int? Id_Rol { get; set; }
        public int? Id_Empleado { get; set; }
        public bool? Activo { get; set; }
    }
}

