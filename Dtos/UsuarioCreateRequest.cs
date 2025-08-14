namespace BioAlga.Backend.Dtos
{
    public class UsuarioCreateRequest
    {
        public string Nombre_Usuario { get; set; } = string.Empty;
        public string Contrasena { get; set; } = string.Empty;
        public int Id_Rol { get; set; }
        public int? Id_Empleado { get; set; }
        public bool Activo { get; set; } = true;
    }
}
