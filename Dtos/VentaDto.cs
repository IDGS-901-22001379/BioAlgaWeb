using System;
using System.Collections.Generic;
using BioAlga.Backend.Models.Enums;

namespace BioAlga.Backend.Dtos;

public class VentaDto
{
    public int IdVenta { get; set; }
    public int? ClienteId { get; set; }
    public DateTime FechaVenta { get; set; }

    public decimal Subtotal { get; set; }
    public decimal Impuestos { get; set; }
    public decimal Total { get; set; }

    public MetodoPago MetodoPago { get; set; }
    public decimal? EfectivoRecibido { get; set; }
    public decimal? Cambio { get; set; }

    public string Estatus { get; set; } = "Pagada";

    public List<VentaLineaCreate> Lineas { get; set; } = new();
}
