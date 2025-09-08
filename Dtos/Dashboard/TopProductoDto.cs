namespace BioAlga.Backend.Dtos.Dashboard
{
    public class TopProductoDto
    {
        public int IdProducto { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int TotalUnidades { get; set; }
        public decimal IngresoTotal { get; set; }
    }
}
