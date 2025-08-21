using BioAlga.Backend.Dtos;
using BioAlga.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BioAlga.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ClientesController : ControllerBase
    {
        private readonly IClienteService _service;
        private readonly ILogger<ClientesController> _logger;

        public ClientesController(IClienteService service, ILogger<ClientesController> logger)
        {
            _service = service;
            _logger  = logger;
        }

        // =========================================
        // GET: api/clientes  (Buscar + filtros + paginación)
        // Params esperados desde Angular: q, tipo_Cliente, estado, page, pageSize, sortBy, sortDir
        // =========================================
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<ClienteDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponse<ClienteDto>>> Buscar(
            [FromQuery] string? q,
            [FromQuery(Name = "tipo_Cliente")] string? tipoCliente,
            [FromQuery] string? estado,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = "nombre",
            [FromQuery] string? sortDir = "asc")
        {
            // Normaliza límites de paginación
            page     = page     <= 0 ? 1  : page;
            pageSize = pageSize <= 0 ? 10 : pageSize;

            var query = new ClienteQueryParams
            {
                Q            = q,
                Tipo_Cliente = tipoCliente,
                Estado       = estado,
                Page         = page,
                PageSize     = pageSize,
                SortBy       = sortBy,
                SortDir      = sortDir
            };

            var result = await _service.BuscarAsync(query);
            return Ok(result);
        }

        // =========================================
        // GET: api/clientes/{id}
        // =========================================
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ClienteDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ClienteDto>> ObtenerPorId(int id)
        {
            if (id <= 0) return BadRequest(new { message = "Id inválido." });

            var dto = await _service.ObtenerPorIdAsync(id);
            if (dto is null) return NotFound(new { message = "Cliente no encontrado." });

            return Ok(dto);
        }

        // =========================================
        // POST: api/clientes
        // =========================================
        [HttpPost]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(ClienteDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ClienteDto>> Crear([FromBody] CrearClienteDto body)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            try
            {
                var creado = await _service.CrearAsync(body);

                // Defensa extra: si por alguna razón el servicio no asignó Id, lo registramos
                if (creado is null || creado.Id_Cliente <= 0)
                {
                    _logger.LogWarning("Se creó un cliente pero el DTO devuelto no contiene Id_Cliente válido.");
                    return BadRequest(new { message = "No fue posible obtener el Id del cliente creado." });
                }

                return CreatedAtAction(nameof(ObtenerPorId),
                    new { id = creado.Id_Cliente }, creado);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex) // p.ej. correo duplicado
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear cliente");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Error interno al crear el cliente." });
            }
        }

        // =========================================
        // PUT: api/clientes/{id}
        // =========================================
        [HttpPut("{id:int}")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(ClienteDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ClienteDto>> Actualizar(int id, [FromBody] ActualizarClienteDto body)
        {
            if (id <= 0) return BadRequest(new { message = "Id inválido." });
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            try
            {
                var actualizado = await _service.ActualizarAsync(id, body);
                if (actualizado is null) return NotFound(new { message = "Cliente no encontrado." });

                return Ok(actualizado);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex) // p.ej. correo duplicado
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar cliente {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Error interno al actualizar el cliente." });
            }
        }

        // =========================================
        // DELETE: api/clientes/{id}
        // =========================================
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
                if (!ok) return NotFound(new { message = "Cliente no encontrado." });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar cliente {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Error interno al eliminar el cliente." });
            }
        }
    }
}
