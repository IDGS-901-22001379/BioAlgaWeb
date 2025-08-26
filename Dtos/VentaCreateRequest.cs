using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BioAlga.Backend.Models.Enums;

namespace BioAlga.Backend.Dtos;

public class VentaCreateRequest
{
    public int? ClienteId { get; set; }

    [Required]
    public MetodoPago MetodoPago { get; set; } = MetodoPago.Efectivo;

    // Solo aplica cuando MetodoPago = Efectivo o Mixto
    [Range(0, 999999)]
    public decimal? EfectivoRecibido { get; set; }

    [MinLength(1)]
    public List<VentaLineaCreate> Lineas { get; set; } = new();
}
