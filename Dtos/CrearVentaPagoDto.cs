namespace BioAlga.Backend.Dtos
{
    public class CrearVentaPagoDto
    {
        public int Id_Venta { get; set; }
        public string Metodo { get; set; } = string.Empty;    // Efectivo | Tarjeta | Transferencia | Otro
        public decimal Monto { get; set; }
    }
}
