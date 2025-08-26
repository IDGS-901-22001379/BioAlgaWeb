using System;
using BioAlga.Backend.Models.Enums;

namespace BioAlga.Backend.Models;

public class CajaMovimiento
{
    public int IdCajaMovimiento { get; set; }
    public int IdCajaApertura { get; set; }      // a qué apertura pertenece
    public DateTime Fecha { get; set; } = DateTime.UtcNow;

    public TipoCajaMovimiento Tipo { get; set; } // Ingreso/Egreso
    public string Concepto { get; set; } = string.Empty;

    public decimal MontoEfectivo { get; set; }   // movimiento directo de caja
    public int? IdVenta { get; set; }            // si está ligado a una venta (opcional)

    public int IdUsuario { get; set; }           // quien registró el movimiento

    // Navs
    public CajaApertura CajaApertura { get; set; } = default!;
    public Venta? Venta { get; set; }
    public Usuario Usuario { get; set; } = default!;
}
