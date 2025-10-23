using BioAlga.Backend.Dtos;
using BioAlga.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BioAlga.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CajaTurnosController : ControllerBase
    {
        private readonly ICajaTurnoService _service;
        private readonly ILogger<CajaTurnosController> _logger;

        public CajaTurnosController(ICajaTurnoService service, ILogger<CajaTurnosController> logger)
        {
            _service = service;
            _logger = logger;
        }

        // GET: api/cajaturnos/buscar?idCaja=&idUsuario=&desde=&hasta=&page=&pageSize=
        [HttpGet("buscar")]
        [ProducesResponseType(typeof(PagedResponse<CajaTurnoDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponse<CajaTurnoDto>>> Buscar(
            [FromQuery] int? idCaja,
            [FromQuery] int? idUsuario,
            [FromQuery] DateTime? desde,
            [FromQuery] DateTime? hasta,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _service.BuscarAsync(idCaja, idUsuario, desde, hasta, page, pageSize);
            return Ok(result);
        }

        // GET: api/cajaturnos/{id}
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(CajaTurnoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CajaTurnoDto>> ObtenerPorId(int id)
        {
            if (id <= 0) return BadRequest(new { message = "Id inválido." });

            var dto = await _service.ObtenerPorIdAsync(id);
            if (dto is null) return NotFound(new { message = "Turno no encontrado." });

            return Ok(dto);
        }

        // GET: api/cajaturnos/abierto-por-caja/{idCaja}
        [HttpGet("abierto-por-caja/{idCaja:int}")]
        [ProducesResponseType(typeof(CajaTurnoDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<CajaTurnoDto?>> TurnoAbiertoPorCaja(int idCaja)
        {
            var dto = await _service.ObtenerTurnoAbiertoPorCajaAsync(idCaja);
            return Ok(dto);
        }

        // GET: api/cajaturnos/abierto-por-usuario/{idUsuario}
        [HttpGet("abierto-por-usuario/{idUsuario:int}")]
        [ProducesResponseType(typeof(CajaTurnoDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<CajaTurnoDto?>> TurnoAbiertoPorUsuario(int idUsuario)
        {
            var dto = await _service.ObtenerTurnoAbiertoPorUsuarioAsync(idUsuario);
            return Ok(dto);
        }

        // POST: api/cajaturnos/abrir
        [HttpPost("abrir")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(CajaTurnoDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CajaTurnoDto>> Abrir([FromBody] AbrirTurnoDto body)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            try
            {
                var dto = await _service.AbrirTurnoAsync(body);
                return CreatedAtAction(nameof(ObtenerPorId), new { id = dto.IdTurno }, dto);
            }
            catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al abrir turno");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Error interno al abrir turno." });
            }
        }

        // PUT: api/cajaturnos/cerrar/{idTurno}
        [HttpPut("cerrar/{idTurno:int}")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(CajaTurnoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CajaTurnoDto>> Cerrar(int idTurno, [FromBody] CerrarTurnoDto body)
        {
            if (idTurno <= 0) return BadRequest(new { message = "IdTurno inválido." });
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            try
            {
                var dto = await _service.CerrarTurnoAsync(idTurno, body);
                if (dto is null) return NotFound(new { message = "Turno no encontrado." });
                return Ok(dto);
            }
            catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cerrar turno {IdTurno}", idTurno);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Error interno al cerrar turno." });
            }
        }
    }
}
