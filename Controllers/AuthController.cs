using System.Security.Cryptography;
using System.Text;

namespace RestApiBase.Controllers
{
    public class AuthController
    {
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