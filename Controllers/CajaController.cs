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
    // [Authorize]
    public class CajaController : ControllerBase
    {
        private readonly ICajaService _service;

        public CajaController(ICajaService service)
        {
            _service = service;
        }

        // POST: api/caja/apertura
        [HttpPost("apertura")]
        public async Task<ActionResult<int>> Abrir([FromBody] CajaAperturaCreate req)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            try
            {
                var idUsuario = GetUserId();
                var id = await _service.AbrirCajaAsync(idUsuario, req);
                return Created($"/api/caja/apertura/{id}", id);
            }
            catch (Exception ex)
            {
                return Problem(title: "No se pudo abrir la caja",
                               detail: ex.Message,
                               statusCode: 400);
            }
        }

        // POST: api/caja/movimiento
        [HttpPost("movimiento")]
        public async Task<ActionResult<int>> Movimiento([FromBody] CajaMovimientoCreate req)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            try
            {
                var idUsuario = GetUserId();
                var id = await _service.RegistrarMovimientoAsync(idUsuario, req);
                return Created($"/api/caja/movimiento/{id}", id);
            }
            catch (Exception ex)
            {
                return Problem(title: "No se pudo registrar el movimiento de caja",
                               detail: ex.Message,
                               statusCode: 400);
            }
        }

        // POST: api/caja/corte
        [HttpPost("corte")]
        public async Task<ActionResult<int>> Corte([FromBody] CajaCorteCreate req)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            try
            {
                var idUsuario = GetUserId();
                var id = await _service.RealizarCorteAsync(idUsuario, req);
                return Created($"/api/caja/corte/{id}", id);
            }
            catch (Exception ex)
            {
                return Problem(title: "No se pudo realizar el corte de caja",
                               detail: ex.Message,
                               statusCode: 400);
            }
        }

        private int GetUserId()
        {
            var claim = User?.FindFirst("id_usuario")?.Value
                        ?? User?.FindFirst("sub")?.Value
                        ?? "1";
            return int.TryParse(claim, out var id) ? id : 1;
        }
    }
}
