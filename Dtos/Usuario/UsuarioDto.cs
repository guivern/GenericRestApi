
using RestApiBase.Annotations;

namespace RestApiBase.Dtos.Usuario
{
    public class UsuarioDto
    {
        [Requerido] 
        public string Username { get; set; }

        [Requerido] 
        [LongMin(6)] 
        public string Password { get; set; }
        
        [Requerido] 
        public string ConfirmPassword { get; set; }
        
        [Requerido] 
        public long? IdRol { get; set; }
        
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string NroDocumento { get; set; }
    }
}