using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BioAlga.Backend.Models
{
    public enum TipoMovimiento { Entrada, Salida, Ajuste }
    public enum OrigenMovimiento { Compra, Venta, Pedido, Ajuste, Devolucion }

    [Table("inventario_movimientos")]
    public class InventarioMovimiento
    {
        [Key]
        [Column("id_movimiento")]
        public int IdMovimiento { get; set; }

        [Column("id_producto")]
        public int IdProducto { get; set; }

        // Guardamos el enum como texto en la BD (Entrada/Salida/Ajuste)
        [Column("tipo_movimiento")]
        public string TipoMovimiento { get; set; } = nameof(Models.TipoMovimiento.Entrada);

        [Column("cantidad")]
        public int Cantidad { get; set; }

        [Column("fecha")]
        public DateTime Fecha { get; set; } = DateTime.UtcNow;

        // Origen del movimiento: Compra, Venta, Pedido, Ajuste, Devolucion
        [Column("origen_tipo")]
        public string OrigenTipo { get; set; } = nameof(Models.OrigenMovimiento.Compra);

        [Column("origen_id")]
        public int? OrigenId { get; set; }

        [Column("id_usuario")]
        public int IdUsuario { get; set; }

        [Column("referencia")]
        public string? Referencia { get; set; }
    }
}
