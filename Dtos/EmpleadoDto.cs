using System;

namespace BioAlga.Backend.Dtos
{
    /// <summary>
    /// DTO para listar / obtener un empleado.
    /// </summary>
    public class EmpleadoDto
    {
        public int Id_Empleado { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Apellido_Paterno { get; set; }
        public string? Apellido_Materno { get; set; }
        public string? Curp { get; set; }
        public string? Rfc { get; set; }
        public string? Correo { get; set; }
        public string? Telefono { get; set; }
        public string? Puesto { get; set; }
        public decimal Salario { get; set; }
        public DateTime? Fecha_Ingreso { get; set; }
        public DateTime? Fecha_Baja { get; set; }
        public string Estatus { get; set; } = "Activo";
        public DateTime Created_At { get; set; }
        public DateTime Updated_At { get; set; }

        // Conveniencia para la UI
        public string Nombre_Completo =>
            string.Join(" ", new [] { Nombre, Apellido_Paterno, Apellido_Materno }
                .Where(s => !string.IsNullOrWhiteSpace(s)));
    }
}
