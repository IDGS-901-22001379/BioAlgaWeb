namespace BioAlga.Backend.Dtos.Dashboard
{
    public class DevolucionesPorUsuarioDto
    {
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public int NumDevoluciones { get; set; }
        public decimal TotalDevuelto { get; set; }
    }
}
