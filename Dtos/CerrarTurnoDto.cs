namespace BioAlga.Backend.Dtos
{
    public class CerrarTurnoDto
    {
        public int Id_Turno { get; set; }
        public decimal Saldo_Cierre { get; set; }
        public string? Observaciones { get; set; }
    }
}
