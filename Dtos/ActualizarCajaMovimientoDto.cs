namespace BioAlga.Backend.Dtos
{
    public class ActualizarCajaMovimientoDto
    {
        public string Tipo { get; set; } = string.Empty;      // Ingreso | Egreso
        public string Concepto { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public string? Referencia { get; set; }
    }
}
