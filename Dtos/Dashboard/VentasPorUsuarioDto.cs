namespace BioAlga.Backend.Dtos.Dashboard
{
    public class VentasPorUsuarioDto
    {
        public int IdUsuario { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string ApellidoPaterno { get; set; } = string.Empty;
        public decimal TotalVendido { get; set; }
        public int NumVentas { get; set; }
    }
}
