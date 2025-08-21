using System;

namespace BioAlga.Backend.Dtos
{
    public class ClienteDto
    {
        public int Id_Cliente { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Apellido_Paterno { get; set; }
        public string? Apellido_Materno { get; set; }
        public string? Correo { get; set; }
        public string? Telefono { get; set; }
        public string? Direccion { get; set; }
        public string Tipo_Cliente { get; set; } = "Normal";  // Normal | Mayoreo | Especial | Descuento
        public string Estado { get; set; } = "Activo";        // Activo | Inactivo
        public DateTime Fecha_Registro { get; set; }

        public string Nombre_Completo =>
            string.Join(' ',
                new[] { Nombre, Apellido_Paterno, Apellido_Materno }
                .Where(s => !string.IsNullOrWhiteSpace(s)));
    }
}
