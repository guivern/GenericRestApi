using System;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace RestApiBase.Models
{
    public class AuditEntityBase: SoftDeleteEntityBase
    {
        [JsonIgnore]
        [ForeignKey("IdUsarioCreador")]
        public Usuario UsuarioCreador {get; set;}
        public long? IdUsarioCreador {get; set;}

        public DateTime FechaCreacion {get; set;} = DateTime.Now;

        [JsonIgnore]
        [ForeignKey("IdUsarioEditor")]
        public Usuario UsuarioEditor {get; set;}
        public long? IdUsarioEditor {get; set;}

        public DateTime? FechaEdicion {get; set;}
    }
}