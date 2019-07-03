namespace RestApiBase.Models
{
    public class SoftDeleteEntityBase: EntityBase
    {
        public bool Activo {get; set;} = true;
    }
}