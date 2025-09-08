using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace BioAlga.Backend.Models.Dashboard
{
    [Keyless]
    public class VentasResumen
    {
        [Column("dia")]
        public DateTime Dia { get; set; }

        [Column("anio")]
        public int Anio { get; set; }

        [Column("mes")]
        public int Mes { get; set; }

        [Column("semana")]
        public int Semana { get; set; }

        [Column("total_ventas")]
        public decimal TotalVentas { get; set; }

        [Column("subtotal")]
        public decimal Subtotal { get; set; }

        [Column("impuestos")]
        public decimal Impuestos { get; set; }

        [Column("num_tickets")]
        public int NumTickets { get; set; }
    }
}
