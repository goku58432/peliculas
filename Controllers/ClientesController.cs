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
    public class ClientesController : ControllerBase
    {
        private readonly AppDbContext _db;
        public ClientesController(AppDbContext db) => _db = db;

        // Obtener todos (sin mostrar clave) - admin
        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAll()
        {
            var clientes = await _db.Clientes
                .OrderByDescending(c => c.FechaRegistro)
                .Select(c => new
                {
                    c.Id,
                    c.Nombre,
                    c.ApellidoPaterno,
                    c.ApellidoMaterno,
                    c.Correo,
                    c.Activo,
                    c.FechaRegistro
                    // NO se devuelve la Clave
                }).ToListAsync();
            return Ok(clientes);
        }

        // Registrar cliente - público (desde web o móvil)
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Create([FromBody] ClienteCreateDto dto)
        {
            if (await _db.Clientes.AnyAsync(c => c.Correo == dto.Correo))
                return BadRequest(new { message = "El correo ya está registrado" });

            var clave = GenerarClave();
            var cliente = new Cliente
            {
                Nombre = dto.Nombre,
                ApellidoPaterno = dto.ApellidoPaterno,
                ApellidoMaterno = dto.ApellidoMaterno,
                Correo = dto.Correo,
                Clave = clave,
                Activo = 1
            };

            _db.Clientes.Add(cliente);
            await _db.SaveChangesAsync();

            // Se retorna la clave SOLO en el momento del registro
            return Ok(new
            {
                cliente.Id,
                cliente.Nombre,
                cliente.ApellidoPaterno,
                cliente.ApellidoMaterno,
                cliente.Correo,
                clave, // Solo aquí se muestra
                mensaje = "Cliente registrado. Guarde su clave, no se volverá a mostrar."
            });
        }

        // Actualizar datos (no clave)
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Update(int id, [FromBody] ClienteUpdateDto dto)
        {
            var c = await _db.Clientes.FindAsync(id);
            if (c == null) return NotFound();

            c.Nombre = dto.Nombre;
            c.ApellidoPaterno = dto.ApellidoPaterno;
            c.ApellidoMaterno = dto.ApellidoMaterno;
            c.Correo = dto.Correo;
            await _db.SaveChangesAsync();
            return Ok(new { message = "Cliente actualizado" });
        }

        // Activar
        [HttpPut("{id}/activar")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Activar(int id)
        {
            var c = await _db.Clientes.FindAsync(id);
            if (c == null) return NotFound();
            c.Activo = 1;
            await _db.SaveChangesAsync();
            return Ok(new { message = "Cliente activado" });
        }

        // Inactivar
        [HttpPut("{id}/inactivar")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Inactivar(int id)
        {
            var c = await _db.Clientes.FindAsync(id);
            if (c == null) return NotFound();
            c.Activo = 0;
            await _db.SaveChangesAsync();
            return Ok(new { message = "Cliente inactivado" });
        }

        // Eliminar
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var c = await _db.Clientes.FindAsync(id);
            if (c == null) return NotFound();
            _db.Clientes.Remove(c);
            await _db.SaveChangesAsync();
            return Ok(new { message = "Cliente eliminado" });
        }

        private static string GenerarClave()
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghjkmnpqrstuvwxyz23456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
