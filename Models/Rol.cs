namespace RestApiBase.Models
{
    public class Rol: SoftDeleteEntityBase
    {
        public string Nombre {get; set;}
        public string Descripcion {get; set;}
    }
}