using System.ComponentModel.DataAnnotations;

namespace RestApiBase.Models
{
    public class Usuario: AuditEntityBase
    {
        public const int UsernameMaxLength = 12;

        [MaxLength(UsernameMaxLength)]
        public string Username {get; set;}
        public byte[] PasswordHash {get; set;}
        public byte[] PasswordSalt {get; set;}
    }
}