using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BioAlga.Backend.Services.Interfaces;
using BioAlga.Backend.Dtos;

namespace BioAlga.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // [Authorize]  // Descomenta cuando tengas JWT listo
    public class VentasController : ControllerBase
    {
        private readonly IVentaService _service;

        public VentasController(IVentaService service)
        {
            _service = service;
        }

        // POST: api/ventas
        [HttpPost]
        public async Task<ActionResult<VentaDto>> Crear([FromBody] VentaCreateRequest req)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            try
            {
                var idUsuario = GetUserId();
                var venta = await _service.RegistrarVentaAsync(idUsuario, req);
                return Created($"/api/ventas/{venta.IdVenta}", venta);
            }
            catch (Exception ex)
            {
                return Problem(
                    title: "No se pudo registrar la venta",
                    detail: ex.Message,
                    statusCode: 400
                );
            }
        }

        // POST: api/ventas/{id}/cancelar
        [HttpPost("{id:int}/cancelar")]
        public async Task<IActionResult> Cancelar([FromRoute] int id)
        {
            try
            {
                var idUsuario = GetUserId();
                var ok = await _service.CancelarVentaAsync(idUsuario, id);
                return ok ? NoContent() : NotFound();
            }
            catch (Exception ex)
            {
                return Problem(
                    title: "No se pudo cancelar la venta",
                    detail: ex.Message,
                    statusCode: 400
                );
            }
        }

        private int GetUserId()
        {
            // Ajusta el claim según tu Auth (ej. "id_usuario" o ClaimTypes.NameIdentifier)
            var claim = User?.FindFirst("id_usuario")?.Value
                        ?? User?.FindFirst("sub")?.Value
                        ?? "1"; // fallback para pruebas locales

            return int.TryParse(claim, out var id) ? id : 1;
        }
    }
}
