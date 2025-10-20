using BioAlga.Backend.Dtos;
using BioAlga.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BioAlga.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class VentaPagosController : ControllerBase
    {
        private readonly IVentaPagoService _service;
        private readonly ILogger<VentaPagosController> _logger;

        public VentaPagosController(IVentaPagoService service, ILogger<VentaPagosController> logger)
        {
            _service = service;
            _logger = logger;
        }

        // GET: api/ventapagos/por-venta/{idVenta}
        [HttpGet("por-venta/{idVenta:int}")]
        [ProducesResponseType(typeof(IReadOnlyList<VentaPagoDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<VentaPagoDto>>> ListarPorVenta(int idVenta)
        {
            var list = await _service.ListarPorVentaAsync(idVenta);
            return Ok(list);
        }

        // POST: api/ventapagos
        [HttpPost]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(VentaPagoDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<VentaPagoDto>> Crear([FromBody] CrearVentaPagoDto body)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            try
            {
                var dto = await _service.CrearAsync(body);
                return CreatedAtAction(nameof(ListarPorVenta), new { idVenta = dto.Id_Venta }, dto);
            }
            catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear pago de venta");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Error interno al crear el pago." });
            }
        }

        // DELETE: api/ventapagos/{idPago}
        [HttpDelete("{idPago:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Eliminar(int idPago)
        {
            try
            {
                var ok = await _service.EliminarAsync(idPago);
                if (!ok) return NotFound(new { message = "Pago no encontrado." });
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar pago {IdPago}", idPago);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Error interno al eliminar el pago." });
            }
        }
    }
}
