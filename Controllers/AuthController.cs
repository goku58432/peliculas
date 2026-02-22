using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using StreamingAPI.Data;
using StreamingAPI.DTOs;

namespace StreamingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;

        public AuthController(AppDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        // Login administradores (WEB)
        [HttpPost("login/admin")]
        public async Task<IActionResult> LoginAdmin([FromBody] LoginDto dto)
        {
            var user = await _db.Usuarios
                .FirstOrDefaultAsync(u => u.UserName == dto.Usuario && u.Activo == 1);

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Contrasena, user.Contrasena))
                return Unauthorized(new { message = "Usuario o contraseña incorrectos" });

            var token = GenerarToken(user.Id.ToString(), user.UserName, "admin");
            return Ok(new { token, nombre = user.Nombre, rol = "admin" });
        }

        // Login clientes (MÓVIL)
        [HttpPost("login/cliente")]
        public async Task<IActionResult> LoginCliente([FromBody] LoginDto dto)
        {
            var cliente = await _db.Clientes
                .FirstOrDefaultAsync(c => c.Correo == dto.Usuario && c.Activo == 1);

            if (cliente == null || cliente.Clave != dto.Contrasena)
                return Unauthorized(new { message = "Credenciales incorrectas" });

            var token = GenerarToken(cliente.Id.ToString(), cliente.Correo, "cliente");
            return Ok(new { token, nombre = cliente.Nombre, rol = "cliente" });
        }

        private string GenerarToken(string id, string usuario, string rol)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, id),
                new Claim(ClaimTypes.Name, usuario),
                new Claim(ClaimTypes.Role, rol)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(8),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
