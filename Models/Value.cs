using RestApiBase.Annotations;

namespace RestApiBase.Models
{
    public class Value: AuditEntityBase
    {
        [SearchFilter]
        public string Descripcion {get; set;}
    }
}