namespace BioAlga.Backend.Dtos
{
    public class PrecioDto
    {
        public int Id_Precio { get; set; }
        public int Id_Producto { get; set; }
        public string Tipo_Precio { get; set; } = "Normal"; // Normal/Mayoreo/…
        public decimal Precio { get; set; }
        public DateTime Vigente_Desde { get; set; }
        public DateTime? Vigente_Hasta { get; set; }
        public bool Activo { get; set; }
    }
}
