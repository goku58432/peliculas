using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
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
        private readonly Cloudinary _cloudinary;

        public PeliculasController(AppDbContext db, IConfiguration config)
        {
            _db = db;
            var account = new Account(
                config["Cloudinary:CloudName"],
                config["Cloudinary:ApiKey"],
                config["Cloudinary:ApiSecret"]
            );
            _cloudinary = new Cloudinary(account);
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAll()
            => Ok(await _db.Peliculas.OrderByDescending(p => p.FechaRegistro).ToListAsync());

        [HttpGet("activas")]
        [Authorize]
        public async Task<IActionResult> GetActivas()
            => Ok(await _db.Peliculas
                .Where(p => p.Activa == 1)
                .OrderByDescending(p => p.FechaRegistro)
                .ToListAsync());

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([FromForm] PeliculaCreateDto dto)
        {
            string? imagenUrl = null;

            if (dto.Imagen != null && dto.Imagen.Length > 0)
            {
                using var stream = dto.Imagen.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(dto.Imagen.FileName, stream),
                    Folder = "streaming/peliculas"
                };
                var result = await _cloudinary.UploadAsync(uploadParams);
                imagenUrl = result.SecureUrl.ToString();
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

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Update(int id, [FromForm] PeliculaUpdateDto dto)
        {
            var p = await _db.Peliculas.FindAsync(id);
            if (p == null) return NotFound();

            if (dto.Imagen != null && dto.Imagen.Length > 0)
            {
                using var stream = dto.Imagen.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(dto.Imagen.FileName, stream),
                    Folder = "streaming/peliculas"
                };
                var result = await _cloudinary.UploadAsync(uploadParams);
                p.ImagenUrl = result.SecureUrl.ToString();
            }

            p.Nombre = dto.Nombre;
            p.Genero = dto.Genero;
            p.Descripcion = dto.Descripcion;
            p.TrailerUrl = dto.TrailerUrl;
            await _db.SaveChangesAsync();
            return Ok(p);
        }

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
