using BioAlga.Backend.Dtos;
using BioAlga.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BioAlga.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProveedoresController : ControllerBase
    {
        private readonly IProveedorService _service;
        public ProveedoresController(IProveedorService service) => _service = service;

        // GET: api/proveedores?q=...&estatus=Activo&pais=...&ciudad=...&page=1&pageSize=10&sortBy=Nombre_Empresa&sortDir=asc
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<ProveedorDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponse<ProveedorDto>>> Buscar([FromQuery] ProveedorQueryParams p)
            => Ok(await _service.BuscarAsync(p));

        // GET: api/proveedores/5
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ProveedorDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProveedorDto>> Obtener(int id)
        {
            var dto = await _service.ObtenerPorIdAsync(id);
            return dto is null ? NotFound() : Ok(dto);
        }

        // POST: api/proveedores
        [HttpPost]
        [ProducesResponseType(typeof(ProveedorDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProveedorDto>> Crear([FromBody] CrearProveedorDto dto)
        {
            try
            {
                var creado = await _service.CrearAsync(dto);
                return CreatedAtAction(nameof(Obtener), new { id = creado.Id_Proveedor }, creado);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/proveedores/5
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ProveedorDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProveedorDto>> Actualizar(int id, [FromBody] ActualizarProveedorDto dto)
        {
            try
            {
                var updated = await _service.ActualizarAsync(id, dto);
                return updated is null ? NotFound() : Ok(updated);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PATCH: api/proveedores/5/estatus?valor=Inactivo
        [HttpPatch("{id:int}/estatus")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CambiarEstatus(int id, [FromQuery] string valor = "Activo")
        {
            var ok = await _service.CambiarEstatusAsync(id, valor);
            return ok ? NoContent() : NotFound();
        }

        // DELETE duro (solo si lo necesitas)
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Eliminar(int id)
        {
            var ok = await _service.EliminarDuroAsync(id);
            return ok ? NoContent() : NotFound();
        }
    }
}
