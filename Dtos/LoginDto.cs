using RestApiBase.Annotations;

namespace RestApiBase.Dtos
{
    public class LoginDto
    {
        [Requerido]
        public string Username {get; set;}
        [Requerido]
        public string Password {get; set;}
    }
}