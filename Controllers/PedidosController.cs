using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

using BioAlga.Backend.Dtos;
using BioAlga.Backend.Models.Enums;
using BioAlga.Backend.Services.Interfaces;

namespace BioAlga.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PedidosController : ControllerBase
    {
        private readonly IPedidoService _service;

        public PedidosController(IPedidoService service)
        {
            _service = service;
        }

        // Utilidad: obtiene id de usuario desde header (o 1 por defecto en dev)
        private int ResolveUserId(int? headerUserId) => headerUserId ?? 1;

        // =========================
        // GET: api/pedidos
        // =========================
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<PedidoListItemDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponse<PedidoListItemDto>>> Buscar(
            [FromQuery] string? q,
            [FromQuery] EstatusPedido? estatus,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = "FechaPedido",
            [FromQuery] string? sortDir = "DESC")
        {
            var resp = await _service.BuscarAsync(q, estatus, page, pageSize, sortBy, sortDir);
            return Ok(resp);
        }

        // =========================
        // GET: api/pedidos/{id}
        // =========================
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(PedidoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PedidoDto>> GetById([FromRoute] int id)
        {
            var dto = await _service.GetAsync(id);
            if (dto is null) return NotFound();
            return Ok(dto);
        }

        // =========================
        // POST: api/pedidos  (BORRADOR)
        // =========================
        [HttpPost]
        [ProducesResponseType(typeof(PedidoDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PedidoDto>> Crear(
            [FromHeader(Name = "X-User-Id")] int? userId,
            [FromBody] PedidoCreateRequest req)
        {
            try
            {
                var dto = await _service.CrearAsync(ResolveUserId(userId), req);
                return CreatedAtAction(nameof(GetById), new { id = dto.IdPedido }, dto);
            }
            catch (ValidationException ex) { return BadRequest(ex.Message); }
            catch (KeyNotFoundException) { return NotFound(); }
        }

        // =========================
        // PUT: api/pedidos/header   (BORRADOR)
        // =========================
        [HttpPut("header")]
        [ProducesResponseType(typeof(PedidoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PedidoDto>> UpdateHeader(
            [FromHeader(Name = "X-User-Id")] int? userId,
            [FromBody] PedidoUpdateHeaderRequest req)
        {
            try
            {
                var dto = await _service.UpdateHeaderAsync(ResolveUserId(userId), req);
                return Ok(dto);
            }
            catch (ValidationException ex) { return BadRequest(ex.Message); }
            catch (KeyNotFoundException) { return NotFound(); }
        }

        // =========================
        // PUT: api/pedidos/lines/replace  (BORRADOR)
        // =========================
        [HttpPut("lines/replace")]
        [ProducesResponseType(typeof(PedidoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PedidoDto>> ReplaceLines(
            [FromHeader(Name = "X-User-Id")] int? userId,
            [FromBody] PedidoReplaceLinesRequest req)
        {
            try
            {
                var dto = await _service.ReplaceLinesAsync(ResolveUserId(userId), req);
                return Ok(dto);
            }
            catch (ValidationException ex) { return BadRequest(ex.Message); }
            catch (KeyNotFoundException) { return NotFound(); }
        }

        // =========================
        // PUT: api/pedidos/lines/edit   (BORRADOR)
        // =========================
        [HttpPut("lines/edit")]
        [ProducesResponseType(typeof(PedidoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PedidoDto>> EditLine(
            [FromHeader(Name = "X-User-Id")] int? userId,
            [FromBody] PedidoLineaEditRequest req)
        {
            try
            {
                var dto = await _service.EditLineAsync(ResolveUserId(userId), req);
                return Ok(dto);
            }
            catch (ValidationException ex) { return BadRequest(ex.Message); }
            catch (KeyNotFoundException) { return NotFound(); }
        }

        // =========================
        // PUT: api/pedidos/confirm
        // =========================
        [HttpPut("confirm")]
        [ProducesResponseType(typeof(PedidoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PedidoDto>> Confirmar(
            [FromHeader(Name = "X-User-Id")] int? userId,
            [FromBody] PedidoConfirmarRequest req)
        {
            try
            {
                var dto = await _service.ConfirmarAsync(ResolveUserId(userId), req);
                return Ok(dto);
            }
            catch (ValidationException ex) { return BadRequest(ex.Message); }
            catch (KeyNotFoundException) { return NotFound(); }
        }

        // =========================
        // PUT: api/pedidos/status
        // =========================
        [HttpPut("status")]
        [ProducesResponseType(typeof(PedidoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PedidoDto>> CambiarEstatus(
            [FromHeader(Name = "X-User-Id")] int? userId,
            [FromBody] PedidoCambioEstatusRequest req)
        {
            try
            {
                var dto = await _service.CambiarEstatusAsync(ResolveUserId(userId), req);
                return Ok(dto);
            }
            catch (ValidationException ex) { return BadRequest(ex.Message); }
            catch (KeyNotFoundException) { return NotFound(); }
        }

        // =========================
        // DELETE: api/pedidos/{id}
        // Solo permite eliminar pedidos en BORRADOR
        // =========================
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                await _service.EliminarAsync(id);
                return NoContent();
            }
            catch (ValidationException ex) { return BadRequest(ex.Message); }
            catch (KeyNotFoundException) { return NotFound(); }
        }
    }
}
