using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using RestApiBase.Annotations;
using static RestApiBase.Annotations.SearchFilter;

namespace RestApiBase.Models
{
    public class Usuario: AuditEntityBase
    {
        public const int UsernameMaxLength = 12;

        [MaxLength(UsernameMaxLength)]
        [SearchFilter]
        public string Username {get; set;}

        [JsonIgnore]
        public byte[] PasswordHash {get; set;}
        
        [JsonIgnore]
        public byte[] PasswordSalt {get; set;}

        [SearchFilter]
        public string Nombre {get; set;}
        
        [SearchFilter]
        public string Apellido{get; set;}
        
        [SearchFilter]
        public string NroDocumento {get; set;}
        
        [JsonIgnore]
        [ForeignKey("IdRol")]
        public Rol Rol {get; set;}
        public long IdRol {get; set;}
        
        [NotMapped]
        public virtual string NombreRol => Rol?.Nombre;
        
        [NotMapped]
        public virtual string NombreUsuarioCreador => UsuarioCreador?.Username;
    }
}