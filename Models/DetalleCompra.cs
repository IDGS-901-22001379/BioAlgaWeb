using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BioAlga.Backend.Models;

[Table("detalle_compra")]
public class DetalleCompra
{
    [Key] [Column("id_detalle")] public int IdDetalle { get; set; }

    [Column("id_compra")] public int IdCompra { get; set; }
    public Compra? Compra { get; set; }

    [Column("id_producto")] public int IdProducto { get; set; }
    public Producto? Producto { get; set; }

    [Column("cantidad")] public int Cantidad { get; set; }

    [Column("costo_unitario")] public decimal CostoUnitario { get; set; }

    [Column("iva_unitario")] public decimal IvaUnitario { get; set; } = 0m;
}
