using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore; // DbUpdateException
using Microsoft.Extensions.Hosting;   // IHostEnvironment
using Microsoft.Extensions.Logging;   // ILogger
using BioAlga.Backend.Services.Interfaces;
using BioAlga.Backend.Dtos;

namespace BioAlga.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // [Authorize]  // Descomenta cuando tengas JWT listo
    public class VentasController : ControllerBase
    {
        private readonly IVentaService _service;
        private readonly ILogger<VentasController> _logger;
        private readonly IHostEnvironment _env;

        public VentasController(
            IVentaService service,
            ILogger<VentasController> logger,
            IHostEnvironment env)
        {
            _service = service;
            _logger = logger;
            _env = env;
        }

        // =========================================
        // POST: api/ventas
        // Crea una venta
        // =========================================
        [HttpPost]
        [ProducesResponseType(typeof(VentaDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<VentaDto>> Crear([FromBody] VentaCreateRequest req)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            try
            {
                var idUsuario = GetUserId();
                var venta = await _service.RegistrarVentaAsync(idUsuario, req);
                return Created($"/api/ventas/{venta.IdVenta}", venta);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "DbUpdateException al registrar venta");
                return ProblemBadRequest("No se pudo registrar la venta",
                    ex.InnerException?.Message ?? ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error no controlado al registrar venta");
                return ProblemBadRequest("No se pudo registrar la venta",
                    ex.InnerException?.Message ?? ex.Message);
            }
        }

        // =========================================
        // POST: api/ventas/{id}/cancelar
        // Cancela una venta
        // =========================================
        [HttpPost("{id:int}/cancelar")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Cancelar([FromRoute] int id)
        {
            try
            {
                var idUsuario = GetUserId();
                var ok = await _service.CancelarVentaAsync(idUsuario, id);
                return ok ? NoContent() : NotFound();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "DbUpdateException al cancelar venta {IdVenta}", id);
                return ProblemBadRequest("No se pudo cancelar la venta",
                    ex.InnerException?.Message ?? ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error no controlado al cancelar venta {IdVenta}", id);
                return ProblemBadRequest("No se pudo cancelar la venta",
                    ex.InnerException?.Message ?? ex.Message);
            }
        }

        // =========================================
        // Helpers
        // =========================================
        private int GetUserId()
        {
            // Ajusta el claim según tu Auth (ej. "id_usuario" o ClaimTypes.NameIdentifier)
            var claim = User?.FindFirst("id_usuario")?.Value
                        ?? User?.FindFirst("sub")?.Value
                        ?? "1"; // fallback para pruebas locales

            return int.TryParse(claim, out var id) ? id : 1;
        }

        private ActionResult ProblemBadRequest(string title, string detail)
        {
            // En desarrollo puedes incluir más detalle si quisieras; aquí ya pasamos el mensaje claro.
            var pd = new ProblemDetails
            {
                Title = title,
                Detail = detail,
                Status = StatusCodes.Status400BadRequest,
                Type = "https://httpstatuses.com/400"
            };

            // Opcional: agregar un flag para que el front decida cómo mostrarlo
            if (_env.IsDevelopment())
            {
                pd.Extensions["environment"] = "Development";
            }

            return BadRequest(pd);
        }
    }
}
