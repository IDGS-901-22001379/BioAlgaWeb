// Dtos/VentaCreateRequest.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BioAlga.Backend.Models.Enums;

namespace BioAlga.Backend.Dtos
{
    /// <summary>
    /// Solicitud para registrar una venta.
    /// </summary>
    public class VentaCreateRequest : IValidatableObject
    {
        /// <summary>Id del cliente (nullable para venta a público en general).</summary>
        public int? ClienteId { get; set; }

        /// <summary>Método de pago seleccionado.</summary>
        [Required]
        public MetodoPago MetodoPago { get; set; } = MetodoPago.Efectivo;

        /// <summary>
        /// Efectivo recibido (solo aplica cuando el método de pago es Efectivo o Mixto).
        /// </summary>
        [Range(0, 999999)]
        public decimal? EfectivoRecibido { get; set; }

        /// <summary>Líneas de la venta (al menos 1).</summary>
        [Required]
        [MinLength(1, ErrorMessage = "La venta debe contener al menos un producto.")]
        public List<VentaLineaCreate> Lineas { get; set; } = new();

        /// <summary>
        /// Validaciones condicionales.
        /// </summary>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Validar EfectivoRecibido cuando el método lo requiere
            if ((MetodoPago == MetodoPago.Efectivo || MetodoPago == MetodoPago.Mixto) &&
                EfectivoRecibido is null)
            {
                yield return new ValidationResult(
                    "Debes especificar el efectivo recibido cuando el método de pago es Efectivo o Mixto.",
                    new[] { nameof(EfectivoRecibido) }
                );
            }

            // Asegurar que la colección no sea null ni vacía
            if (Lineas is null || Lineas.Count == 0)
            {
                yield return new ValidationResult(
                    "La venta debe contener al menos un producto.",
                    new[] { nameof(Lineas) }
                );
            }
        }
    }
}
