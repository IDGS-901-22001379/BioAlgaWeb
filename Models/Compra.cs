using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BioAlga.Backend.Models;

[Table("compras")]
public class Compra
{
    [Key] [Column("id_compra")] public int IdCompra { get; set; }

    [Column("proveedor_id")] public int? ProveedorId { get; set; }
    [Column("proveedor_texto")] public string? ProveedorTexto { get; set; }

    [Column("fecha_compra")] public DateTime FechaCompra { get; set; } = DateTime.UtcNow;

    [Column("subtotal")] public decimal Subtotal { get; set; }
    [Column("impuestos")] public decimal Impuestos { get; set; }
    [Column("total")] public decimal Total { get; set; }

    [Column("id_usuario")] public int IdUsuario { get; set; }
    [Column("notas")] public string? Notas { get; set; }

    // Estado simple: "Borrador" | "Confirmada"
    [NotMapped] public string Estado => Detalles?.Any() == true && Confirmada ? "Confirmada" : "Borrador";
    [NotMapped] public bool Confirmada { get; set; } = false; // se mantiene en memoria para reglas

    public ICollection<DetalleCompra> Detalles { get; set; } = new List<DetalleCompra>();
}
