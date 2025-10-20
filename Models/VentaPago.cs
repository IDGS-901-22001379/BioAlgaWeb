using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BioAlga.Backend.Models
{
    [Table("venta_pagos")]
    public class VentaPago
    {
        [Key]
        [Column("id_pago")]
        public int IdPago { get; set; }

        [Column("id_venta")]
        public int IdVenta { get; set; }

        // 'Efectivo' | 'Tarjeta' | 'Transferencia' | 'Otro'
        [Column("metodo")]
        public string Metodo { get; set; } = null!;

        [Column("monto", TypeName = "decimal(12,2)")]
        public decimal Monto { get; set; }

        [Column("creado_en")]
        public DateTime CreadoEn { get; set; } = DateTime.UtcNow;
    }
}
