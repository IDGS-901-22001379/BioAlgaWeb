using System;

namespace BioAlga.Backend.Dtos
{
    public class CajaDto
    {
        public int Id_Caja { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
    }
}
