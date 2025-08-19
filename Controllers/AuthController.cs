// Controllers/AuthController.cs
using BioAlga.Backend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BioAlga.Backend.Controllers
{
    // DTO para la petición de login
    public class LoginRequest
    {
        [Required] 
        public string NombreUsuario { get; set; } = string.Empty;

        [Required] 
        public string Contrasena { get; set; } = string.Empty;
    }

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public AuthController(ApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Endpoint de autenticación de usuario
        /// POST: api/auth/login
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            // Validación de datos
            if (!ModelState.IsValid) 
                return BadRequest("Datos inválidos.");

            // Buscar usuario en la base de datos
            var user = await _db.Usuarios
                .FirstOrDefaultAsync(u => u.Nombre_Usuario == req.NombreUsuario);

            if (user is null) 
                return Unauthorized("Usuario o contraseña inválidos.");

            // Verificación de contraseña con soporte para hash BCrypt
            bool valid;
            if (user.Contrasena.StartsWith("$2")) // Prefijo típico de BCrypt
            {
                valid = BCrypt.Net.BCrypt.Verify(req.Contrasena, user.Contrasena);
            }
            else
            {
                // Comparación simple (temporal, para pruebas)
                valid = user.Contrasena == req.Contrasena;
            }

            if (!valid) 
                return Unauthorized("Usuario o contraseña inválidos.");

            // Retornar datos básicos (podrías añadir un JWT en producción)
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
