namespace RestApiBase.Dtos.Usuario
{
    public class UsuarioDetailDto
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string NroDocumento { get; set; }
        public long IdRol { get; set; }
    }
}