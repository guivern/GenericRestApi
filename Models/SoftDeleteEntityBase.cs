using Newtonsoft.Json;

namespace RestApiBase.Models
{
    public class SoftDeleteEntityBase: EntityBase
    {
        [JsonIgnore]
        public bool Activo {get; set;} = true;
    }
}