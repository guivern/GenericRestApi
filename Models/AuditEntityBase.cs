using System;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace RestApiBase.Models
{
    public class AuditEntityBase: SoftDeleteEntityBase
    {
        [JsonIgnore]
        [ForeignKey("IdUsuarioCreador")]
        public Usuario UsuarioCreador {get; set;}
        public long? IdUsuarioCreador {get; set;}
        
        public DateTime FechaCreacion {get; set;} = DateTime.Now;

        [JsonIgnore]
        [ForeignKey("IdUsuarioModificador")]
        public Usuario UsuarioModificador {get; set;}
        public long? IdUsuarioModificador {get; set;}

        public DateTime? UltimaModificacion {get; set;}
    }
}