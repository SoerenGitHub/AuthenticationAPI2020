using AutoMapper;
using AuthAPI.Dtos;
using AuthAPI.Models;

namespace AuthAPI.Profiles {
    public class TokensProfile : Profile {
        public TokensProfile()
        {
            CreateMap<Token, TokenReadDto>();
        }
    }
}