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
        [JsonIgnore]
        public DateTime FechaCreacion {get; set;} = DateTime.Now;

        [JsonIgnore]
        [ForeignKey("IdUsuarioEditor")]
        public Usuario UsuarioEditor {get; set;}
        public long? IdUsuarioEditor {get; set;}
        [JsonIgnore]
        public DateTime? FechaEdicion {get; set;}
    }
}