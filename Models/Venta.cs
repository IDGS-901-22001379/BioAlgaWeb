using System;
using System.Collections.Generic;
using BioAlga.Backend.Models.Enums;

namespace BioAlga.Backend.Models;

public class Venta
{
    public int IdVenta { get; set; }
    public int? ClienteId { get; set; }          // FK clientes.id_cliente (nullable)
    public DateTime FechaVenta { get; set; } = DateTime.UtcNow;

    public decimal Subtotal { get; set; }
    public decimal Impuestos { get; set; }
    public decimal Total { get; set; }

    public decimal? EfectivoRecibido { get; set; }   // puede ser 0/NULL según método
    public decimal? Cambio { get; set; }

    public MetodoPago MetodoPago { get; set; } = MetodoPago.Efectivo;
    public int IdUsuario { get; set; }               // FK usuarios.id_usuario

    public EstatusVenta Estatus { get; set; } = EstatusVenta.Pagada;

    // Navs
    public Cliente? Cliente { get; set; }
    public Usuario? Usuario { get; set; }
    public ICollection<DetalleVenta> Detalle { get; set; } = new List<DetalleVenta>();
}
