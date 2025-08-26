using System.ComponentModel.DataAnnotations;

namespace BioAlga.Backend.Dtos;

public class CajaCorteCreate
{
    [Required] public int IdCajaApertura { get; set; }

    // Lo contado físicamente al cierre
    [Range(0, 999999)]
    public decimal TotalEfectivoContado { get; set; }
}
