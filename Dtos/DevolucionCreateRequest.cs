using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BioAlga.Backend.Dtos;

public class DevolucionCreateRequest
{
    [Required] public int IdVenta { get; set; }

    [Required, MaxLength(120)]
    public string Motivo { get; set; } = "defecto"; // daño, garantía, error, etc.

    // true = reingresa a stock (Entrada); false = no reingresa (Merma/Ajuste)
    public bool ReingresaInventario { get; set; } = true;

    [MinLength(1)]
    public List<DevolucionLineaCreate> Lineas { get; set; } = new();
}
