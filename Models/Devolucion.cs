using System;
using System.Collections.Generic;

namespace BioAlga.Backend.Models;

public class Devolucion
{
    public int IdDevolucion { get; set; }
    public int IdVenta { get; set; }            // referencia a la venta original
    public DateTime Fecha { get; set; } = DateTime.UtcNow;

    public string Motivo { get; set; } = "defecto";   // daño, garantía, error, etc.
    public bool ReingresaInventario { get; set; } = true; // si false -> Merma

    public int IdUsuario { get; set; }          // quien registra la devolución

    // Totales informativos
    public decimal Subtotal { get; set; }
    public decimal Impuestos { get; set; }
    public decimal Total { get; set; }

    // Navs
    public Venta Venta { get; set; } = default!;
    public Usuario Usuario { get; set; } = default!;
    public ICollection<DetalleDevolucion> Detalle { get; set; } = new List<DetalleDevolucion>();
}
