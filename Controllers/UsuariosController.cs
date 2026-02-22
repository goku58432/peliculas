using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StreamingAPI.Data;
using StreamingAPI.DTOs;
using StreamingAPI.Models;

namespace StreamingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _db;
        public UsuariosController(AppDbContext db) => _db = db;

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAll()
        {
            var usuarios = await _db.Usuarios
                .OrderByDescending(u => u.FechaRegistro)
                .Select(u => new
                {
                    u.Id, u.Nombre, u.ApellidoPaterno, u.ApellidoMaterno,
                    u.Correo, u.UserName, u.Activo, u.FechaRegistro
                }).ToListAsync();
            return Ok(usuarios);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([FromBody] UsuarioCreateDto dto)
        {
            if (await _db.Usuarios.AnyAsync(u => u.UserName == dto.UserName))
                return BadRequest(new { message = "El usuario ya existe" });

            var usuario = new Usuario
            {
                Nombre = dto.Nombre,
                ApellidoPaterno = dto.ApellidoPaterno,
                ApellidoMaterno = dto.ApellidoMaterno,
                Correo = dto.Correo,
                UserName = dto.UserName,
                Contrasena = BCrypt.Net.BCrypt.HashPassword(dto.Contrasena),
                Activo = 1
            };

            _db.Usuarios.Add(usuario);
            await _db.SaveChangesAsync();
            return Ok(new { message = "Usuario registrado" });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UsuarioCreateDto dto)
        {
            var u = await _db.Usuarios.FindAsync(id);
            if (u == null) return NotFound();
            u.Nombre = dto.Nombre;
            u.ApellidoPaterno = dto.ApellidoPaterno;
            u.ApellidoMaterno = dto.ApellidoMaterno;
            u.Correo = dto.Correo;
            if (!string.IsNullOrWhiteSpace(dto.Contrasena))
                u.Contrasena = BCrypt.Net.BCrypt.HashPassword(dto.Contrasena);
            await _db.SaveChangesAsync();
            return Ok(new { message = "Usuario actualizado" });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var u = await _db.Usuarios.FindAsync(id);
            if (u == null) return NotFound();
            _db.Usuarios.Remove(u);
            await _db.SaveChangesAsync();
            return Ok(new { message = "Usuario eliminado" });
        }

        // Crear primer admin (solo si no hay usuarios) - sin autenticación
        [HttpPost("setup")]
        [AllowAnonymous]
        public async Task<IActionResult> Setup([FromBody] UsuarioCreateDto dto)
        {
            if (await _db.Usuarios.AnyAsync())
                return BadRequest(new { message = "Ya existen administradores" });

            var usuario = new Usuario
            {
                Nombre = dto.Nombre,
                ApellidoPaterno = dto.ApellidoPaterno,
                ApellidoMaterno = dto.ApellidoMaterno,
                Correo = dto.Correo,
                UserName = dto.UserName,
                Contrasena = BCrypt.Net.BCrypt.HashPassword(dto.Contrasena),
                Activo = 1
            };

            _db.Usuarios.Add(usuario);
            await _db.SaveChangesAsync();
            return Ok(new { message = "Administrador inicial creado correctamente" });
        }
    }
}
