namespace BioAlga.Backend.Dtos
{
    public class CorteMetodoDto
    {
        public string Metodo { get; set; } = string.Empty; // Efectivo | Tarjeta | Transferencia | Otro
        public decimal Monto { get; set; }
    }
}
