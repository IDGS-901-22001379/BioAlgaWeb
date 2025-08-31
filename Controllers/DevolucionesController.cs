using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BioAlga.Backend.Services.Interfaces;
using BioAlga.Backend.Dtos;

namespace BioAlga.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    // [Authorize]
    public class DevolucionesController : ControllerBase
    {
        private readonly IDevolucionService _service;
        private readonly IWebHostEnvironment _env;

        public DevolucionesController(IDevolucionService service, IWebHostEnvironment env)
        {
            _service = service;
            _env = env;
        }

        // ===============================
        // POST: api/devoluciones
        // Crea una devolución (sin obligar número de venta)
        // ===============================
        [HttpPost]
        // [AllowAnonymous] // <- descomenta en desarrollo si aún no tienes auth/token
        public async Task<ActionResult<DevolucionDto>> Crear(
            [FromBody] DevolucionCreateRequest req,
            CancellationToken ct)
        {
            if (!ModelState.IsValid) 
                return ValidationProblem(ModelState);

            try
            {
                var idUsuario = GetUserId();
                var dto = await _service.CrearAsync(idUsuario, req, ct);

                // Devolver siempre 200 con JSON para Angular
                return Ok(dto);
            }
            catch (Exception ex)
            {
                // Mostrar detalle completo SOLO en desarrollo
                var detalle = _env.IsDevelopment() ? ex.ToString() : ex.Message;

                return Problem(
                    title: "No se pudo registrar la devolución",
                    detail: detalle,
                    statusCode: StatusCodes.Status400BadRequest
                );
            }
        }

        // ===============================
        // GET: api/devoluciones/{id}
        // Obtiene una devolución con detalles
        // ===============================
        [HttpGet("{id:int}")]
        public async Task<ActionResult<DevolucionDto>> Obtener(int id, CancellationToken ct)
        {
            var dto = await _service.ObtenerAsync(id, ct);
            if (dto is null) 
                return NotFound(new { message = "Devolución no encontrada." });

            return Ok(dto);
        }

        // ===============================
        // GET: api/devoluciones
        // Listado con filtros/paginación
        // ===============================
        [HttpGet]
        public async Task<ActionResult<PagedResponse<DevolucionDto>>> Buscar(
            [FromQuery] DevolucionQueryParams q,
            CancellationToken ct)
        {
            var result = await _service.BuscarAsync(q, ct);
            return Ok(result);
        }

        // ===============================
        // Helpers
        // ===============================
        private int GetUserId()
        {
            // Ajusta si tu esquema de claims es distinto
            var claim =
                User?.FindFirst("id_usuario")?.Value ??
                User?.FindFirst("sub")?.Value ??
                "1"; // fallback para pruebas locales

            return int.TryParse(claim, out var id) ? id : 1;
        }
    }
}
