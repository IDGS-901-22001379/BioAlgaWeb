using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace BioAlga.Backend.Models.Dashboard
{
    [Keyless]
    public class ComprasPorProveedor
    {
        [Column("id_proveedor")]
        public int IdProveedor { get; set; }

        [Column("nombre_empresa")]
        public string NombreEmpresa { get; set; } = string.Empty;

        [Column("total_comprado")]
        public decimal TotalComprado { get; set; }

        [Column("num_compras")]
        public int NumCompras { get; set; }
    }
}
