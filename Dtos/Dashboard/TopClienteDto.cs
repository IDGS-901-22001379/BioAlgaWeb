namespace BioAlga.Backend.Dtos.Dashboard
{
    public class TopClienteDto
    {
        public int IdCliente { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public decimal TotalGastado { get; set; }
    }
}
