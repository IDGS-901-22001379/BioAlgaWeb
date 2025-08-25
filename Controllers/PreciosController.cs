// Controllers/PreciosController.cs
using BioAlga.Backend.Dtos;
using BioAlga.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BioAlga.Backend.Controllers
{
    [ApiController]
    [Route("api")]
    public class PreciosController : ControllerBase
    {
        private readonly IPrecioService _service;

        public PreciosController(IPrecioService service)
        {
            _service = service;
        }

        // GET: api/productos/5/precios     (historial completo)
        [HttpGet("productos/{idProducto:int}/precios")]
        [ProducesResponseType(typeof(IReadOnlyList<PrecioDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<PrecioDto>>> Historial(int idProducto, CancellationToken ct)
        {
            var lst = await _service.GetHistorialAsync(idProducto, ct);
            return Ok(lst);
        }

        // GET: api/productos/5/precios/vigentes   (0..1 por tipo: Normal/Mayoreo/Descuento/Especial)
        [HttpGet("productos/{idProducto:int}/precios/vigentes")]
        [ProducesResponseType(typeof(IReadOnlyList<PrecioDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<PrecioDto>>> Vigentes(int idProducto, CancellationToken ct)
        {
            var lst = await _service.GetVigentesAsync(idProducto, ct);
            return Ok(lst);
        }

        // POST: api/productos/5/precios
        // Regla: desactiva/cierrra el vigente anterior del mismo tipo antes de crear el nuevo
        [HttpPost("productos/{idProducto:int}/precios")]
        [ProducesResponseType(typeof(PrecioDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PrecioDto>> Crear(int idProducto, [FromBody] CrearPrecioDto dto, CancellationToken ct)
        {
            try
            {
                var creado = await _service.CrearAsync(idProducto, dto, ct);
                if (creado is null) return BadRequest(new { message = "No se pudo crear el precio." });
                // No hay endpoint GET por idPrecio; devolvemos 201 con el recurso en el body
                return Created($"api/productos/{idProducto}/precios", creado);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/precios/123   (actualiza precio/vigenteHasta/activo del registro histórico)
        [HttpPut("precios/{idPrecio:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Actualizar(int idPrecio, [FromBody] ActualizarPrecioDto dto, CancellationToken ct)
        {
            var ok = await _service.ActualizarAsync(idPrecio, dto, ct);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}
