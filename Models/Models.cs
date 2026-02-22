using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StreamingAPI.Models
{
    [Table("usuarios")]
    public class Usuario
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; } = "";

        [Column("apellido_paterno")]
        public string ApellidoPaterno { get; set; } = "";

        [Column("apellido_materno")]
        public string ApellidoMaterno { get; set; } = "";

        [Column("correo")]
        public string Correo { get; set; } = "";

        [Column("user_name")]
        public string UserName { get; set; } = "";

        [Column("contrasena")]
        public string Contrasena { get; set; } = "";

        [Column("activo")]
        public int Activo { get; set; } = 1;

        [Column("fecha_registro")]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }

    [Table("clientes")]
    public class Cliente
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; } = "";

        [Column("apellido_paterno")]
        public string ApellidoPaterno { get; set; } = "";

        [Column("apellido_materno")]
        public string ApellidoMaterno { get; set; } = "";

        [Column("correo")]
        public string Correo { get; set; } = "";

        [Column("clave")]
        public string Clave { get; set; } = "";

        [Column("activo")]
        public int Activo { get; set; } = 1;

        [Column("fecha_registro")]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }

    [Table("peliculas")]
    public class Pelicula
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; } = "";

        [Column("genero")]
        public string Genero { get; set; } = "";

        [Column("imagen_url")]
        public string? ImagenUrl { get; set; }

        [Column("descripcion")]
        public string Descripcion { get; set; } = "";

        [Column("trailer_url")]
        public string? TrailerUrl { get; set; }

        [Column("activa")]
        public int Activa { get; set; } = 1;

        [Column("fecha_registro")]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}
