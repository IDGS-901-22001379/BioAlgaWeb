using System;

namespace BioAlga.Backend.Models;

public class CajaCorte
{
    public int IdCajaCorte { get; set; }
    public int IdCajaApertura { get; set; }      // se cierra una apertura
    public DateTime FechaCorte { get; set; } = DateTime.UtcNow;

    public decimal TotalEfectivoEsperado { get; set; } // fondo + ventas - egresos + ingresos
    public decimal TotalEfectivoContado { get; set; }  // lo contado físicamente
    public decimal Diferencia { get; set; }            // contado - esperado

    public int IdUsuario { get; set; }                 // quien realiza el corte

    // Navs
    public CajaApertura CajaApertura { get; set; } = default!;
    public Usuario Usuario { get; set; } = default!;
}
