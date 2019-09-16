using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestApiBase.Data;
using RestApiBase.Dtos;
using RestApiBase.Models;

namespace RestApiBase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Administrador")]
    public class UsuariosController : ApiControllerBase<Usuario, UsuarioDto>
    {
        public UsuariosController(DataContext context, IMapper mapper) : base(context, mapper) { }

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

        protected override IQueryable<Usuario> IncludeInList(IQueryable<Usuario> query)
        {
            return query.Include(u => u.Rol).Include(u => u.Rol);
        }

        protected override IQueryable<Usuario> IncludeInDetail(IQueryable<Usuario> query)
        {
            return query.Include(u => u.Rol);
        }

        private async Task<bool> IsValidUsername(string username, long id)
        {
            username = username.ToLower();
            return await _context.Usuarios.AnyAsync(u => u.Activo 
                && u.Username.Equals(username) && u.Id != id);
        }
    }
}