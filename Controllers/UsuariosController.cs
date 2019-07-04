using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestApiBase.Annotations;
using RestApiBase.Data;
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

        [HttpPost]
        public override async Task<IActionResult> Create(UsuarioDto dto)
        {
            if (!await IsValidDto(dto)) return BadRequest(ModelState);

            var usuario = new Usuario()
            {
                Username = dto.Username.ToLower(),
                IdRol = (long)dto.IdRol,
            };

            byte[] passwordHash, passwordSalt;

            _authService.CreatePasswordHash(dto.Password, out passwordHash, out passwordSalt);
            usuario.PasswordHash = passwordHash;
            usuario.PasswordSalt = passwordSalt;

            await _context.Usuarios.AddAsync(usuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Detail", new { id = usuario.Id }, usuario); 
        }

        [HttpPut("{id}")]
        public override async Task<IActionResult> Update(long id, UsuarioDto dto)
        {
            if (!await UserExits(id)) return NotFound();
            if (!await IsValidDto(dto, id)) return BadRequest(ModelState);

            byte[] passwordHash, passwordSalt;

            var usuario = await _context.Usuarios.FindAsync(id);
            usuario.Username = dto.Username.ToLower();
            usuario.IdRol = (long)dto.IdRol;

            _authService.CreatePasswordHash(dto.Password, out passwordHash, out passwordSalt);
            usuario.PasswordHash = passwordHash;
            usuario.PasswordSalt = passwordSalt;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        protected override async Task<bool> IsValidDto(UsuarioDto dto, long id = 0)
        {
            if (await UsernameIsValid(dto.Username, id))
            {
                ModelState.AddModelError(nameof(dto.Username), "No disponible");
            }

            if (!dto.Password.Equals(dto.ConfirmPassword))
            {
                ModelState.AddModelError(nameof(dto.ConfirmPassword), "No coincide");
            }

            return ModelState.IsValid;
        }

        private async Task<bool> UsernameIsValid(string username, long id)
        {
            username = username.ToLower();
            return await _context.Usuarios
            .AnyAsync(u => u.Username.Equals(username) && u.Id != id);
        }

        private async Task<bool> UserExits(long id)
        {
            return await _context.Usuarios.AnyAsync(u => u.Id == id);
        }

    }

    public class UsuarioDto
    {
        [Requerido]
        public string Username { get; set; }
        [Requerido]
        [LongMin(5)]
        public string Password { get; set; }
        [Requerido]
        [LongMin(5)]
        public string ConfirmPassword { get; set; }
        [Requerido]
        public long? IdRol { get; set; }
    }
}