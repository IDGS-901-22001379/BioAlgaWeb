namespace BioAlga.Backend.Models.Dashboard
{
    public class TopCliente
    {
        public int IdCliente { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public decimal TotalGastado { get; set; }
    }
}
