namespace BioAlga.Backend.Models.Dashboard
{
    public class TopProducto
    {
        public int IdProducto { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int TotalUnidades { get; set; }
        public decimal IngresoTotal { get; set; }   // solo se usa en vw_top_productos_ingreso
    }
}
