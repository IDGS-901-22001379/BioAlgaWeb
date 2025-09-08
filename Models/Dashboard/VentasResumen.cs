namespace BioAlga.Backend.Models.Dashboard
{
    public class VentasResumen
    {
        public DateTime Dia { get; set; }
        public int Anio { get; set; }
        public int Mes { get; set; }
        public int Semana { get; set; }
        public decimal TotalVentas { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Impuestos { get; set; }
        public int NumTickets { get; set; }
    }
}
