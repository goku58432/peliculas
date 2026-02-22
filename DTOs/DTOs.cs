namespace StreamingAPI.DTOs
{
    public class LoginDto
    {
        public string Usuario { get; set; } = "";
        public string Contrasena { get; set; } = "";
    }

    public class ClienteCreateDto
    {
        public string Nombre { get; set; } = "";
        public string ApellidoPaterno { get; set; } = "";
        public string ApellidoMaterno { get; set; } = "";
        public string Correo { get; set; } = "";
    }

    public class ClienteUpdateDto
    {
        public string Nombre { get; set; } = "";
        public string ApellidoPaterno { get; set; } = "";
        public string ApellidoMaterno { get; set; } = "";
        public string Correo { get; set; } = "";
    }

    public class UsuarioCreateDto
    {
        public string Nombre { get; set; } = "";
        public string ApellidoPaterno { get; set; } = "";
        public string ApellidoMaterno { get; set; } = "";
        public string Correo { get; set; } = "";
        public string UserName { get; set; } = "";
        public string Contrasena { get; set; } = "";
    }

    public class PeliculaCreateDto
    {
        public string Nombre { get; set; } = "";
        public string Genero { get; set; } = "";
        public IFormFile? Imagen { get; set; }
        public string Descripcion { get; set; } = "";
        public string TrailerUrl { get; set; } = "";
    }

    public class PeliculaUpdateDto
    {
        public string Nombre { get; set; } = "";
        public string Genero { get; set; } = "";
        public string Descripcion { get; set; } = "";
        public string TrailerUrl { get; set; } = "";
    }
}
