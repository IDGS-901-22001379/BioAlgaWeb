namespace BioAlga.Backend.Models.Dashboard
{
    public class ComprasPorProveedor
    {
        public int IdProveedor { get; set; }
        public string NombreEmpresa { get; set; } = string.Empty;
        public decimal TotalComprado { get; set; }
        public int NumCompras { get; set; }
    }
}
