namespace BioAlga.Backend.Dtos
{
    public class ProveedorDto
    {
        public int Id_Proveedor { get; set; }
        public string Nombre_Empresa { get; set; } = string.Empty;
        public string? Contacto { get; set; }
        public string? Correo { get; set; }
        public string? Telefono { get; set; }
        public string? Direccion { get; set; }
        public string? Rfc { get; set; }
        public string? Pais { get; set; }
        public string? Ciudad { get; set; }
        public string? Codigo_Postal { get; set; }
        public string Estatus { get; set; } = "Activo";
        public DateTime Created_At { get; set; }
    }
}
