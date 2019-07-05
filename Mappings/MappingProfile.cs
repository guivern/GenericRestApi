using AutoMapper;
using RestApiBase.Controllers;
using RestApiBase.Dtos;
using RestApiBase.Models;

namespace RestApiBase.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            CreateMap<ValueDto, Value>();
            CreateMap<UsuarioDto, Usuario>()
            .ForMember(u => u.PasswordSalt, opt => opt.Ignore())
            .ForMember(u => u.PasswordHash, opt => opt.Ignore());
        }
    }
}
/* Ejemplos:
CreateMap<CategoriaDto, Categoria>();

Mapper.CreateMap<Employee, EmployeeDto>()
.ForMember(d => d.FullName, opt => opt.MapFrom(src => src.FirstName + " " + src.LastName));

CreateMap<CategoriaDto, Categoria>()
.ForMember(d => d.FechaCreacion, opt => opt.Ignore());
*/
