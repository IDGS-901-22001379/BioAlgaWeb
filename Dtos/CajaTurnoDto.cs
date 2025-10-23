namespace BioAlga.Backend.Dtos
{
    public class CajaTurnoDto
    {
        public int IdTurno { get; set; }
        public int IdCaja { get; set; }
        public int IdUsuario { get; set; }
        public DateTime Apertura { get; set; }
        public DateTime? Cierre { get; set; }
        public decimal SaldoInicial { get; set; }
        public decimal? SaldoCierre { get; set; }
        public string? Observaciones { get; set; }
    }
}
