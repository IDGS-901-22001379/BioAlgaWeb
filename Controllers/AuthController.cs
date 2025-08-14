// Controllers/AuthController.cs
using BioAlga.Backend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BioAlga.Backend.Controllers
{
    public class LoginRequest
    {
        [Required] public string NombreUsuario { get; set; } = string.Empty;
        [Required] public string Contrasena { get; set; } = string.Empty;
    }

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public AuthController(ApplicationDbContext db) => _db = db;

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            if (!ModelState.IsValid) return BadRequest("Datos inválidos.");

            var user = await _db.Usuarios
                .FirstOrDefaultAsync(u => u.Nombre_Usuario == req.NombreUsuario);

            if (user is null) return Unauthorized("Usuario o contraseña inválidos.");

            // ✅ Soporta hash BCrypt; si la contraseña guardada NO es hash, hace comparación plana (temporal).
            bool valid;
            if (user.Contrasena.StartsWith("$2")) // típico prefijo de BCrypt
                valid = BCrypt.Net.BCrypt.Verify(req.Contrasena, user.Contrasena);
            else
                valid = user.Contrasena == req.Contrasena; // ❗ solo para pruebas. Luego migra a hash.

            if (!valid) return Unauthorized("Usuario o contraseña inválidos.");

            return Ok(new
            {
                user.Id_Usuario,
                user.Nombre_Usuario,
                user.Rol
                // token = "<jwt opcional>"
            });
        }
    }
}
