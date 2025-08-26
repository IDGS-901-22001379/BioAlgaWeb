using System.ComponentModel.DataAnnotations;

namespace BioAlga.Backend.Dtos;

public class CajaAperturaCreate
{
    [Range(0, 999999)]
    public decimal FondoInicial { get; set; }
}
