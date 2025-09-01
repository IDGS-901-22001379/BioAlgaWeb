// ===============================
// DTOs de PEDIDOS
// Namespace sugerido: BioAlga.Backend.Dtos
// ===============================
using System.ComponentModel.DataAnnotations;
using BioAlga.Backend.Models.Enums;

namespace BioAlga.Backend.Dtos
{
    // -------- Lectura: Línea de pedido --------
    public class PedidoLineaDto
    {
        public int IdDetalle { get; set; }
        public int IdProducto { get; set; }
        public string ProductoNombre { get; set; } = string.Empty;

        // Cantidad solicitada (no la entregada; las entregas van en Ventas)
        public int Cantidad { get; set; }

        // Precio unitario CONGELADO (cuando Estatus pasa a Confirmado)
        public decimal PrecioUnitario { get; set; }

        // Monto calculado para conveniencia de UI
        public decimal Importe => System.Math.Round(Cantidad * PrecioUnitario, 2);
    }

    // -------- Lectura: Cabecera (para listados) --------
    public class PedidoListItemDto
    {
        public int IdPedido { get; set; }
        public EstatusPedido Estatus { get; set; }
        public DateTime FechaPedido { get; set; }
        public DateTime? FechaRequerida { get; set; }

        public int IdCliente { get; set; }
        public string ClienteNombre { get; set; } = string.Empty;

        public decimal Subtotal { get; set; }
        public decimal Impuestos { get; set; }
        public decimal Total { get; set; }
        public decimal Anticipo { get; set; }

        // Para UI: cuánto quedaría pendiente (no considera pagos externos aún)
        public decimal SaldoPendiente => System.Math.Round(Total - Anticipo, 2);
    }

    // -------- Lectura: Detalle completo --------
    public class PedidoDto : PedidoListItemDto
    {
        public int IdUsuario { get; set; }
        public string? Notas { get; set; }

        public List<PedidoLineaDto> Lineas { get; set; } = new();
    }

    // -------- Creación: Línea --------
    public class PedidoLineaCreateRequest
    {
        [Required]
        public int IdProducto { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Cantidad debe ser ≥ 1")]
        public int Cantidad { get; set; }

        // Opcional: si se manda, se respeta (según permisos);
        // si no se manda, el backend tomará el precio por tipo de cliente y lo CONGELARÁ al confirmar.
        public decimal? PrecioUnitarioOverride { get; set; }
    }

    // -------- Creación: Cabecera --------
    public class PedidoCreateRequest
    {
        [Required]
        public int IdCliente { get; set; }

        public DateTime? FechaRequerida { get; set; }

        [Range(0, 999999999999.99)]
        public decimal Anticipo { get; set; } = 0m;

        public string? Notas { get; set; }

        [MinLength(1, ErrorMessage = "Debe incluir al menos una línea")]
        public List<PedidoLineaCreateRequest> Lineas { get; set; } = new();
    }

    // -------- Actualización: Cabecera (en Borrador) --------
    // Usar para cambiar datos generales sin tocar líneas.
    public class PedidoUpdateHeaderRequest
    {
        [Required]
        public int IdPedido { get; set; }

        public int? IdCliente { get; set; }
        public DateTime? FechaRequerida { get; set; }

        [Range(0, 999999999999.99)]
        public decimal? Anticipo { get; set; }

        public string? Notas { get; set; }
    }

    // -------- Reemplazo/Edición de líneas (en Borrador) --------
    // Política simple: reemplaza TODAS las líneas por las nuevas.
    public class PedidoReplaceLinesRequest
    {
        [Required]
        public int IdPedido { get; set; }

        [MinLength(1)]
        public List<PedidoLineaCreateRequest> Lineas { get; set; } = new();
    }

    // -------- Edición granular de líneas (opcional) --------
    public class PedidoLineaEditRequest
    {
        [Required]
        public int IdPedido { get; set; }

        // Si IdDetalle es null => es una línea NUEVA
        public int? IdDetalle { get; set; }

        [Required]
        public int IdProducto { get; set; }

        [Range(1, int.MaxValue)]
        public int Cantidad { get; set; }

        public decimal? PrecioUnitarioOverride { get; set; }
    }

    // -------- Confirmar Pedido --------
    // Al confirmar: se CONGELAN precios por tipo de cliente y (opcionalmente) se RESERVA stock.
    public class PedidoConfirmarRequest
    {
        [Required]
        public int IdPedido { get; set; }

        // Si true, activa política de reservas en inventario (implementación posterior)
        public bool ReservarStock { get; set; } = false;
    }

    // -------- Cambio de Estatus (transiciones válidas) --------
    public class PedidoCambioEstatusRequest
    {
        [Required]
        public int IdPedido { get; set; }

        [Required]
        public EstatusPedido NuevoEstatus { get; set; }
    }
}
