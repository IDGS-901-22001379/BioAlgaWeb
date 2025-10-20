using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BioAlga.Backend.Models
{
    [Table("caja_movimientos")]
    public class CajaMovimiento
    {
        [Key]
        [Column("id_mov")]
        public int IdMov { get; set; }

        [Column("id_turno")]
        public int IdTurno { get; set; }

        [Column("fecha")]
        public DateTime Fecha { get; set; } = DateTime.UtcNow;

        // 'Ingreso' | 'Egreso'
        [Column("tipo")]
        public string Tipo { get; set; } = null!;

        [Column("concepto")]
        public string Concepto { get; set; } = null!;

        [Column("monto", TypeName = "decimal(12,2)")]
        public decimal Monto { get; set; }

        [Column("referencia")]
        public string? Referencia { get; set; }

        // Nav opcional
        public CajaTurno? Turno { get; set; }
    }
}
