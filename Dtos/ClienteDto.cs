// Dtos/ClienteDto.cs
namespace BioAlga.Backend.Dtos
{
    public class ClienteDto
    {
        public int Id_Cliente { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Apellido { get; set; }
        public string? Correo { get; set; }
        public string? Telefono { get; set; }
        public string? Direccion { get; set; }
        public string Tipo_Cliente { get; set; } = "Normal";
        public string Estado { get; set; } = "Activo";
        public DateTime Fecha_Registro { get; set; }
    }
}
