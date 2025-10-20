using BioAlga.Backend.Dtos;
using BioAlga.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BioAlga.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CajaMovimientosController : ControllerBase
    {
        private readonly ICajaMovimientoService _service;
        private readonly ILogger<CajaMovimientosController> _logger;

        public CajaMovimientosController(ICajaMovimientoService service, ILogger<CajaMovimientosController> logger)
        {
            _service = service;
            _logger = logger;
        }

        // GET: api/cajamovimientos/por-turno/{idTurno}?tipo=&q=&page=&pageSize=
        [HttpGet("por-turno/{idTurno:int}")]
        [ProducesResponseType(typeof(PagedResponse<CajaMovimientoDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponse<CajaMovimientoDto>>> BuscarPorTurno(
            int idTurno,
            [FromQuery] string? tipo,
            [FromQuery(Name = "q")] string? qConcepto,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _service.BuscarPorTurnoAsync(idTurno, tipo, qConcepto, page, pageSize);
            return Ok(result);
        }

        // GET: api/cajamovimientos/{id}
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(CajaMovimientoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CajaMovimientoDto>> Obtener(int id)
        {
            var dto = await _service.ObtenerPorIdAsync(id);
            if (dto is null) return NotFound(new { message = "Movimiento no encontrado." });
            return Ok(dto);
        }

        // POST: api/cajamovimientos
        [HttpPost]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(CajaMovimientoDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CajaMovimientoDto>> Crear([FromBody] CrearCajaMovimientoDto body)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            try
            {
                var dto = await _service.CrearAsync(body);
                return CreatedAtAction(nameof(Obtener), new { id = dto.Id_Mov }, dto);
            }
            catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear movimiento");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Error interno al crear el movimiento." });
            }
        }

        // PUT: api/cajamovimientos/{id}
        [HttpPut("{id:int}")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(CajaMovimientoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CajaMovimientoDto>> Actualizar(int id, [FromBody] ActualizarCajaMovimientoDto body)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            try
            {
                var dto = await _service.ActualizarAsync(id, body);
                if (dto is null) return NotFound(new { message = "Movimiento no encontrado." });
                return Ok(dto);
            }
            catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar movimiento {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Error interno al actualizar el movimiento." });
            }
        }

        // DELETE: api/cajamovimientos/{id}
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                var ok = await _service.EliminarAsync(id);
                if (!ok) return NotFound(new { message = "Movimiento no encontrado." });
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar movimiento {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Error interno al eliminar el movimiento." });
            }
        }
    }
}
