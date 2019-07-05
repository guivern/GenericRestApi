using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestApiBase.Data;
using RestApiBase.Dtos;
using RestApiBase.Models;
using RestApiBase.Services;

namespace RestApiBase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ApiControllerBase<Usuario,UsuarioDto>
    {
        private readonly IAuthService _authService;

        public UsuariosController(DataContext context, IAuthService authService, IMapper mapper)
        : base(context, mapper)
        {
            _authService = authService;
        }

        protected override void CustomMapping(ref Usuario entity, UsuarioDto dto)
        {
            byte[] passwordHash, passwordSalt;

            _authService.CreatePasswordHash(dto.Password, out passwordHash, out passwordSalt);
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

        private async Task<bool> IsValidUsername(string username, long id)
        {
            username = username.ToLower();
            return await _context.Usuarios
            .AnyAsync(u => u.Username.Equals(username) && u.Id != id);
        }
    }
}