using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestApiBase.Data;
using RestApiBase.Dtos.Usuario;
using RestApiBase.Models;

namespace RestApiBase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Administrador")]
    public class UsuariosController : CrudControllerBase<Usuario, UsuarioDto, UsuarioListDto, UsuarioDetailDto>
    {
        public UsuariosController(DataContext context, IMapper mapper) : base(context, mapper) { }

        protected override List<string> GetFilterableProperties()
        {
            return new List<string>
            {
                nameof(Usuario.Nombre),
                nameof(Usuario.Apellido),
                nameof(Usuario.Username),
                nameof(Usuario.NroDocumento),
                $"{nameof(Usuario.Rol)}.{nameof(Rol.Nombre)}"
            };
        }

        protected override async Task<bool> IsValidDtoModel(UsuarioDto dto, long id = 0)
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
                && u.Username.Equals(username)
                && u.Id != id);
        }

        protected override Task BeforeSaveChangesAsync(Usuario entity)
        {
            return Task.CompletedTask;
        }
    }
}