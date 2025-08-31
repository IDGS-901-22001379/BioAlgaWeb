using BioAlga.Backend.Dtos;
using BioAlga.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BioAlga.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DevolucionesController : ControllerBase
    {
        private readonly IDevolucionService _service;

        public DevolucionesController(IDevolucionService service)
        {
            _service = service;
        }

        // ============================
        // POST: api/devoluciones
        // ============================
        [HttpPost]
        [ProducesResponseType(typeof(DevolucionDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DevolucionDto>> Registrar([FromBody] DevolucionCreateRequest req)
        {
            // ⚠️ Aquí asumo que tomas el idUsuario del JWT o sesión.
            // Por ahora lo dejo hardcodeado (ajusta con tu auth real).
            int idUsuario = 1;

            var dto = await _service.RegistrarDevolucionAsync(idUsuario, req);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = dto.IdDevolucion }, dto);
        }

        // ============================
        // GET: api/devoluciones/{id}
        // ============================
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(DevolucionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DevolucionDto>> ObtenerPorId(int id)
        {
            var dto = await _service.ObtenerPorIdAsync(id);
            if (dto == null)
                return NotFound();

            return Ok(dto);
        }

        // ============================
        // GET: api/devoluciones
        // ============================
        [HttpGet]
        [ProducesResponseType(typeof(List<DevolucionDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<DevolucionDto>>> Listar([FromQuery] DevolucionQueryParams filtro)
        {
            var lista = await _service.ListarAsync(filtro);
            return Ok(lista);
        }
    }
}
