using System.ComponentModel.DataAnnotations;
using BioAlga.Backend.Models.Enums;

namespace BioAlga.Backend.Dtos;

public class CajaMovimientoCreate
{
    [Required] public int IdCajaApertura { get; set; }

    [Required] public TipoCajaMovimiento Tipo { get; set; } // Ingreso/Egreso

    [Required, MaxLength(180)]
    public string Concepto { get; set; } = string.Empty;

    [Range(0.01, 999999)]
    public decimal MontoEfectivo { get; set; }

    // opcional: si el movimiento proviene de venta
    public int? IdVenta { get; set; }
}
