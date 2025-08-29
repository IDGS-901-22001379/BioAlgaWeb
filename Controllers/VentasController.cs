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
        // GET: api/ventas
        // Historial paginado/filtrado de ventas
        // =========================================
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<VentaResumenDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResponse<VentaResumenDto>>> Buscar([FromQuery] VentaQueryParams query)
        {
            try
            {
                var resp = await _service.BuscarVentasAsync(query);
                return Ok(resp);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar/buscar ventas");
                return ProblemBadRequest("No se pudo obtener el historial de ventas",
                    ex.InnerException?.Message ?? ex.Message);
            }
        }

        // =========================================
        // GET: api/ventas/{id}
        // Detalle de una venta (encabezado + líneas)
        // =========================================
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(VentaDetalleDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<VentaDetalleDto>> ObtenerPorId([FromRoute] int id)
        {
            try
            {
                var dto = await _service.ObtenerVentaPorIdAsync(id);
                return dto is null ? NotFound() : Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener detalle de venta {IdVenta}", id);
                return ProblemBadRequest("No se pudo obtener el detalle de la venta",
                    ex.InnerException?.Message ?? ex.Message);
            }
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
                return CreatedAtAction(nameof(ObtenerPorId), new { id = venta.IdVenta }, venta);
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
            var pd = new ProblemDetails
            {
                Title = title,
                Detail = detail,
                Status = StatusCodes.Status400BadRequest,
                Type = "https://httpstatuses.com/400"
            };

            if (_env.IsDevelopment())
            {
                pd.Extensions["environment"] = "Development";
            }

            return BadRequest(pd);
        }
    }
}
