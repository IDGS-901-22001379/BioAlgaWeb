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
    public class DevolucionesController : ControllerBase
    {
        private readonly IDevolucionService _service;

        public DevolucionesController(IDevolucionService service)
        {
            _service = service;
        }

        // POST: api/devoluciones
        [HttpPost]
        public async Task<ActionResult<int>> Crear([FromBody] DevolucionCreateRequest req)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            try
            {
                var idUsuario = GetUserId();
                var id = await _service.RegistrarDevolucionAsync(idUsuario, req);
                return Created($"/api/devoluciones/{id}", id);
            }
            catch (Exception ex)
            {
                return Problem(title: "No se pudo registrar la devolución",
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
