// Controllers/ProductosController.cs
using BioAlga.Backend.Dtos;
using BioAlga.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BioAlga.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosController : ControllerBase
    {
        private readonly IProductoService _service;

        public ProductosController(IProductoService service)
        {
            _service = service;
        }

        // GET: api/productos?q=...&page=1&pageSize=10&sortBy=nombre&sortDir=asc
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<ProductoDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponse<ProductoDto>>> Buscar([FromQuery] ProductoQueryParams qp, CancellationToken ct)
        {
            var result = await _service.BuscarAsync(qp, ct);
            return Ok(result);
        }

        // GET: api/productos/5
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ProductoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductoDto>> ObtenerPorId(int id, CancellationToken ct)
        {
            var dto = await _service.ObtenerPorIdAsync(id, ct);
            if (dto is null) return NotFound();
            return Ok(dto);
        }

        // POST: api/productos
        [HttpPost]
        [ProducesResponseType(typeof(ProductoDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProductoDto>> Crear([FromBody] CrearProductoDto dto, CancellationToken ct)
        {
            try
            {
                var creado = await _service.CrearAsync(dto, ct);
                return CreatedAtAction(nameof(ObtenerPorId), new { id = creado.Id_Producto }, creado);
            }
            catch (InvalidOperationException ex)
            {
                // Ej: “El SKU ya existe”
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/productos/5
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarProductoDto dto, CancellationToken ct)
        {
            try
            {
                var ok = await _service.ActualizarAsync(id, dto, ct);
                if (!ok) return NotFound();
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                // Ej: “El SKU ya existe”
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/productos/5
        // (Si prefieres “inactivar” en lugar de borrar, cambia la implementación en el Service.)
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Eliminar(int id, CancellationToken ct)
        {
            var ok = await _service.EliminarAsync(id, ct);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}
