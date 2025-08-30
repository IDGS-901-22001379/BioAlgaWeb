using System.ComponentModel;

namespace BioAlga.Backend.Models.Enums
{
    public enum OrigenMovimiento
    {
        [Description("Compra")]
        Compra = 1,

        [Description("Venta")]
        Venta = 2,

        [Description("Pedido")]
        Pedido = 3,

        [Description("Ajuste")]
        Ajuste = 4,

        [Description("Devolucion")]
        Devolucion = 5
    }
}
