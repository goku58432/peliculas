using Microsoft.EntityFrameworkCore;
using StreamingAPI.Models;

namespace StreamingAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Pelicula> Peliculas { get; set; }
    }
}
