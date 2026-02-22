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
    public class PeliculasController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;

        public PeliculasController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        // Todas las películas - solo admin
        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAll()
            => Ok(await _db.Peliculas.OrderByDescending(p => p.FechaRegistro).ToListAsync());

        // Solo películas activas - clientes móvil
        [HttpGet("activas")]
        [Authorize]
        public async Task<IActionResult> GetActivas()
            => Ok(await _db.Peliculas
                .Where(p => p.Activa == 1)
                .OrderByDescending(p => p.FechaRegistro)
                .ToListAsync());

        // Registrar película
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([FromForm] PeliculaCreateDto dto)
        {
            string? imagenUrl = null;

            if (dto.Imagen != null && dto.Imagen.Length > 0)
            {
                var uploadsPath = Path.Combine(_env.WebRootPath ?? "wwwroot", "images");
                Directory.CreateDirectory(uploadsPath);
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.Imagen.FileName)}";
                var filePath = Path.Combine(uploadsPath, fileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await dto.Imagen.CopyToAsync(stream);
                imagenUrl = $"/images/{fileName}";
            }

            var pelicula = new Pelicula
            {
                Nombre = dto.Nombre,
                Genero = dto.Genero,
                ImagenUrl = imagenUrl,
                Descripcion = dto.Descripcion,
                TrailerUrl = dto.TrailerUrl,
                Activa = 1
            };

            _db.Peliculas.Add(pelicula);
            await _db.SaveChangesAsync();
            return Ok(pelicula);
        }

        // Modificar película
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Update(int id, [FromForm] PeliculaUpdateDto dto)
        {
            var p = await _db.Peliculas.FindAsync(id);
            if (p == null) return NotFound();

            p.Nombre = dto.Nombre;
            p.Genero = dto.Genero;
            p.Descripcion = dto.Descripcion;
            p.TrailerUrl = dto.TrailerUrl;
            await _db.SaveChangesAsync();
            return Ok(p);
        }

        // Activar
        [HttpPut("{id}/activar")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Activar(int id)
        {
            var p = await _db.Peliculas.FindAsync(id);
            if (p == null) return NotFound();
            p.Activa = 1;
            await _db.SaveChangesAsync();
            return Ok(new { message = "Película activada" });
        }

        // Inactivar
        [HttpPut("{id}/inactivar")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Inactivar(int id)
        {
            var p = await _db.Peliculas.FindAsync(id);
            if (p == null) return NotFound();
            p.Activa = 0;
            await _db.SaveChangesAsync();
            return Ok(new { message = "Película inactivada" });
        }

        // Eliminar
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var p = await _db.Peliculas.FindAsync(id);
            if (p == null) return NotFound();
            _db.Peliculas.Remove(p);
            await _db.SaveChangesAsync();
            return Ok(new { message = "Película eliminada" });
        }
    }
}
