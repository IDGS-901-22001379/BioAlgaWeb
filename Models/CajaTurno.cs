using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BioAlga.Backend.Models
{
    [Table("caja_turnos")]
    public class CajaTurno
    {
        [Key]
        [Column("id_turno")]
        public int IdTurno { get; set; }

        [Column("id_caja")]
        public int IdCaja { get; set; }

        [Column("id_usuario")]
        public int IdUsuario { get; set; }

        [Column("apertura")]
        public DateTime Apertura { get; set; } = DateTime.UtcNow;

        [Column("cierre")]
        public DateTime? Cierre { get; set; }

        [Column("saldo_inicial", TypeName = "decimal(12,2)")]
        public decimal SaldoInicial { get; set; } = 0m;

        [Column("saldo_cierre", TypeName = "decimal(12,2)")]
        public decimal? SaldoCierre { get; set; }

        [Column("observaciones")]
        public string? Observaciones { get; set; }

        // Navs opcionales
        public Caja? Caja { get; set; }
    }
}
