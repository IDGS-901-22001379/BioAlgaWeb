using System;
using System.Collections.Generic;

namespace BioAlga.Backend.Dtos;

public class DevolucionDto
{
    public int IdDevolucion { get; set; }
    public int IdVenta { get; set; }
    public DateTime Fecha { get; set; }

    public string Motivo { get; set; } = string.Empty;
    public bool ReingresaInventario { get; set; }

    public decimal Subtotal { get; set; }
    public decimal Impuestos { get; set; }
    public decimal Total { get; set; }

    public List<DevolucionLineaCreate> Lineas { get; set; } = new();
}
