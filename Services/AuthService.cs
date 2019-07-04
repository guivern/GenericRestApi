using System.Security.Cryptography;
using System.Text;
using RestApiBase.Data;

namespace RestApiBase.Services
{
    public class AuthService : IAuthService
    {
        private readonly DataContext _context;

        public AuthService(DataContext context)
        {
            _context = context;
        }

        // Genera un salt y hash a partir del password
        public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key; // clave para generar el hash
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        // Chequea si el password proporcionado es correcto
        public bool IsValidPassword(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                // genera un hash a partir del password y el passwordSalt
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

                // compara el hash generado con el hash original 
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i]) return false;
                }
                return true;
            }
        }
    }
}