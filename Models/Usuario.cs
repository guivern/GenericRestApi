using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestApiBase.Models
{
    public class Usuario: AuditEntityBase
    {
        public const int UsernameMaxLength = 12;

        [MaxLength(UsernameMaxLength)]
        public string Username {get; set;}
        public byte[] PasswordHash {get; set;}
        public byte[] PasswordSalt {get; set;}
        public string Nombre {get; set;}
        public string Apellido{get; set;}
        public string NroDocumento {get; set;}
        
        [ForeignKey("IdRol")]
        public Rol Rol {get; set;}
        public long IdRol {get; set;}
    }
}