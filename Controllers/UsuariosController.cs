using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestApiBase.Data;
using RestApiBase.Dtos;
using RestApiBase.Models;

namespace RestApiBase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ApiControllerBase<Usuario,UsuarioDto>
    {
        public UsuariosController(DataContext context, IMapper mapper)
        : base(context, mapper){}

        protected override void CustomMapping(ref Usuario entity, UsuarioDto dto)
        {
            byte[] passwordHash, passwordSalt;

            CreatePasswordHash(dto.Password, out passwordHash, out passwordSalt);
            entity.PasswordHash = passwordHash;
            entity.PasswordSalt = passwordSalt;    
        }

        protected override async Task<bool> IsValidDto(UsuarioDto dto, long id = 0)
        {
            if (await IsValidUsername(dto.Username, id))
            {
                ModelState.AddModelError(nameof(dto.Username), "No disponible");
            }

            if (!dto.Password.Equals(dto.ConfirmPassword))
            {
                ModelState.AddModelError(nameof(dto.ConfirmPassword), "No coincide");
            }

            return ModelState.IsValid;
        }

        protected override IQueryable<Usuario> IncludeNestedEntitiesInList(IQueryable<Usuario> query)
        {
            return query.Include(u => u.Rol).Include(u => u.UsuarioCreador);
        }

        protected override IQueryable<Usuario> IncludeNestedEntitiesInDetail(IQueryable<Usuario> query)
        {
            return query.Include(u => u.Rol);
        }

        // Genera un salt y hash a partir del password
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key; // clave para generar el hash
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        private async Task<bool> IsValidUsername(string username, long id)
        {
            username = username.ToLower();
            return await _context.Usuarios
            .AnyAsync(u => u.Username.Equals(username) && u.Id != id);
        }
    }
}