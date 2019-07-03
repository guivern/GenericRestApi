using System.ComponentModel.DataAnnotations;

namespace RestApiBase.Models
{
    public class TipoEntityBase: SoftDeleteEntityBase
    {
        public const int DescripcionMaxLength = 32;

        [MaxLength(DescripcionMaxLength)]
        public long Descripcion {get; set;}
    }
}