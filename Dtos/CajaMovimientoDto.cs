using System;
using BioAlga.Backend.Models.Enums;

namespace BioAlga.Backend.Dtos;

public class CajaMovimientoDto
{
    public int IdCajaMovimiento { get; set; }
    public int IdCajaApertura { get; set; }
    public DateTime Fecha { get; set; }
    public TipoCajaMovimiento Tipo { get; set; }
    public string Concepto { get; set; } = string.Empty;
    public decimal MontoEfectivo { get; set; }
    public int? IdVenta { get; set; }
    public int IdUsuario { get; set; }
}
