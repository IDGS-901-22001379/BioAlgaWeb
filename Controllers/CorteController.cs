using BioAlga.Backend.Dtos;
using BioAlga.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BioAlga.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CorteController : ControllerBase
    {
        private readonly ICorteService _service;
        private readonly ILogger<CorteController> _logger;

        public CorteController(ICorteService service, ILogger<CorteController> logger)
        {
            _service = service;
            _logger = logger;
        }

        // GET: api/corte/turno/{idTurno}/resumen
        [HttpGet("turno/{idTurno:int}/resumen")]
        [ProducesResponseType(typeof(CorteResumenDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CorteResumenDto>> ResumenPorTurno(int idTurno)
        {
            var dto = await _service.ObtenerResumenTurnoAsync(idTurno);
            if (dto is null) return NotFound(new { message = "Turno no encontrado." });
            return Ok(dto);
        }
    }
}
