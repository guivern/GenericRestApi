using System.Threading.Tasks;
using RestApiBase.Models;

namespace RestApiBase.Services
{
    public interface IAuthService
    {
        void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
        bool IsValidPassword(string password, byte[] passwordHash, byte[] passwordSalt);
    }
}