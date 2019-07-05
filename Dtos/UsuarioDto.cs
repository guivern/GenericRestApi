using RestApiBase.Annotations;

namespace RestApiBase.Dtos
{
    public class UsuarioDto
    {
        [Requerido]
        public string Username { get; set; }
        [Requerido]
        [LongMin(5)]
        public string Password { get; set; }
        [Requerido]
        [LongMin(5)]
        public string ConfirmPassword { get; set; }
        [Requerido]
        public long? IdRol { get; set; }
        public long? IdUsuarioCreador {get; set;}
    }
}