using System.ComponentModel.DataAnnotations;

namespace BioAlga.Backend.Dtos
{
    /// <summary>
    /// Parámetros de búsqueda/filtrado/paginación para empleados.
    /// </summary>
    public class EmpleadoQueryParams
    {
        /// <summary>
        /// Texto libre para buscar por nombre y apellidos (LIKE).
        /// </summary>
        public string? q { get; set; }

        /// <summary>
        /// Filtrar por puesto exacto (opcional).
        /// </summary>
        public string? puesto { get; set; }

        /// <summary>
        /// 'Activo' | 'Inactivo' | 'Baja' (opcional).
        /// </summary>
        public string? estatus { get; set; }

        // Paginación
        [Range(1, int.MaxValue)]
        public int page { get; set; } = 1;

        [Range(1, 100)]
        public int pageSize { get; set; } = 10;

        // Ordenamiento
        // Campos permitidos: nombre, fecha_ingreso, salario, puesto, estatus
        public string? sortBy { get; set; } = "nombre";
        public string? sortDir { get; set; } = "ASC"; // ASC | DESC
    }
}

