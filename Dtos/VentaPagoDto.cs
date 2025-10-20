using System;

namespace BioAlga.Backend.Dtos
{
    public class VentaPagoDto
    {
        public int Id_Pago { get; set; }
        public int Id_Venta { get; set; }
        public string Metodo { get; set; } = string.Empty;    // Efectivo | Tarjeta | Transferencia | Otro
        public decimal Monto { get; set; }
        public DateTime Creado_En { get; set; }
    }
}
