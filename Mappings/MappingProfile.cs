using System.Security.Cryptography;
using System.Text;
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
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username.ToLower()))
            .ForMember(dest => dest.PasswordSalt, opt => opt.MapFrom(src => GenerateSalt()))
            .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom((src, dest) => GenerateHash(src.Password, dest.PasswordSalt)));
        }

        #region custom mappings
        private byte[] GenerateSalt()
        {
            using (var hmac = new HMACSHA512())
            {
                return hmac.Key; // clave para generar el hash
            }
        }

        private byte[] GenerateHash(string password, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                return hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }
        #endregion
    }
}