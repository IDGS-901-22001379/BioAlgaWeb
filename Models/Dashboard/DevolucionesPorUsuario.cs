namespace BioAlga.Backend.Models.Dashboard
{
    public class DevolucionesPorUsuario
    {
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public int NumDevoluciones { get; set; }
        public decimal TotalDevuelto { get; set; }
    }
}
