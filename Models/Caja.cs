using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BioAlga.Backend.Models
{
    [Table("cajas")]
    public class Caja
    {
        [Key]
        [Column("id_caja")]
        public int IdCaja { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; } = null!;

        [Column("descripcion")]
        public string? Descripcion { get; set; }
    }
}
