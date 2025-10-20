namespace BioAlga.Backend.Dtos
{
    public class AbrirTurnoDto
    {
        public int Id_Caja { get; set; }
        public int Id_Usuario { get; set; }
        public decimal Saldo_Inicial { get; set; } = 0m;
        public string? Observaciones { get; set; }
    }
}
