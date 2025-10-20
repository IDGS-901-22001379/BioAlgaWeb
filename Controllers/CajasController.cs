using BioAlga.Backend.Dtos;
using BioAlga.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BioAlga.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CajasController : ControllerBase
    {
        private readonly ICajaService _service;
        private readonly ILogger<CajasController> _logger;

        public CajasController(ICajaService service, ILogger<CajasController> logger)
        {
            _service = service;
            _logger = logger;
        }

        // GET: api/cajas
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<CajaDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<CajaDto>>> Listar()
        {
            var result = await _service.ListarAsync();
            return Ok(result);
        }

        // GET: api/cajas/{id}
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(CajaDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CajaDto>> Obtener(int id)
        {
            if (id <= 0) return BadRequest(new { message = "Id inválido." });

            var dto = await _service.ObtenerPorIdAsync(id);
            if (dto is null) return NotFound(new { message = "Caja no encontrada." });

            return Ok(dto);
        }

        // POST: api/cajas
        [HttpPost]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(CajaDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CajaDto>> Crear([FromBody] CrearCajaDto body)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            try
            {
                var creado = await _service.CrearAsync(body);
                return CreatedAtAction(nameof(Obtener), new { id = creado.Id_Caja }, creado);
            }
            catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear caja");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Error interno al crear la caja." });
            }
        }

        // PUT: api/cajas/{id}
        [HttpPut("{id:int}")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(CajaDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CajaDto>> Actualizar(int id, [FromBody] ActualizarCajaDto body)
        {
            if (id <= 0) return BadRequest(new { message = "Id inválido." });
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            try
            {
                var actualizado = await _service.ActualizarAsync(id, body);
                if (actualizado is null) return NotFound(new { message = "Caja no encontrada." });
                return Ok(actualizado);
            }
            catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar caja {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Error interno al actualizar la caja." });
            }
        }

        // DELETE: api/cajas/{id}
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Eliminar(int id)
        {
            if (id <= 0) return BadRequest(new { message = "Id inválido." });

            try
            {
                var ok = await _service.EliminarAsync(id);
                if (!ok) return NotFound(new { message = "Caja no encontrada." });
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar caja {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Error interno al eliminar la caja." });
            }
        }
    }
}
