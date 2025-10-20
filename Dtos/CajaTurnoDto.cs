using System;

namespace BioAlga.Backend.Dtos
{
    public class CajaTurnoDto
    {
        public int Id_Turno { get; set; }
        public int Id_Caja { get; set; }
        public int Id_Usuario { get; set; }
        public DateTime Apertura { get; set; }
        public DateTime? Cierre { get; set; }
        public decimal Saldo_Inicial { get; set; }
        public decimal? Saldo_Cierre { get; set; }
        public string? Observaciones { get; set; }
    }
}
