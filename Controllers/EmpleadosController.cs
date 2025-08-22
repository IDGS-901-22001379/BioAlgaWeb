// Controllers/EmpleadosController.cs
using BioAlga.Backend.Dtos;
using BioAlga.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BioAlga.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmpleadosController : ControllerBase
    {
        private readonly IEmpleadoService _service;

        public EmpleadosController(IEmpleadoService service)
        {
            _service = service;
        }

        // =========================================
        // GET: api/empleados  (Buscar + filtros + paginación)
        // Params esperados:
        // q, puesto, estatus, page, pageSize, sortBy, sortDir
        // =========================================
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<EmpleadoDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponse<EmpleadoDto>>> Buscar([FromQuery] EmpleadoQueryParams query)
        {
            var res = await _service.BuscarAsync(query);
            return Ok(res);
        }

        // =========================================
        // GET: api/empleados/{id}
        // =========================================
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(EmpleadoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EmpleadoDto>> ObtenerPorId(int id)
        {
            var emp = await _service.ObtenerPorIdAsync(id);
            if (emp is null) return NotFound();
            return Ok(emp);
        }

        // =========================================
        // POST: api/empleados
        // =========================================
        [HttpPost]
        [ProducesResponseType(typeof(EmpleadoDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<EmpleadoDto>> Crear([FromBody] CrearEmpleadoDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var creado = await _service.CrearAsync(dto);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = creado.Id_Empleado }, creado);
        }

        // =========================================
        // PUT: api/empleados/{id}
        // =========================================
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarEmpleadoDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            if (id != dto.Id_Empleado) return BadRequest("El id de la ruta no coincide con el del cuerpo.");

            var ok = await _service.ActualizarAsync(dto);
            if (!ok) return NotFound();

            return NoContent();
        }

        // =========================================
        // DELETE: api/empleados/{id}
        // =========================================
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Eliminar(int id)
        {
            var ok = await _service.EliminarAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}
