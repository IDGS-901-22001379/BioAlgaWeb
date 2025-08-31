namespace BioAlga.Backend.Dtos
{
    public class DevolucionDetalleDto
    {
        public int IdDetalle { get; set; }
        public int IdProducto { get; set; }
        public string ProductoNombre { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal ImporteLineaTotal { get; set; }
        public int? IdDetalleVenta { get; set; }
    }
}
