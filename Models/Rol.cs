using System.ComponentModel.DataAnnotations;

namespace RestApiBase.Models
{
    public class Rol: SoftDeleteEntityBase
    {
        [Required]
        public string Nombre {get; set;}
        public string Descripcion {get; set;}
    }
}