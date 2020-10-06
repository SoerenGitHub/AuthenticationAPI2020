using AutoMapper;
using AuthAPI.Dtos;
using AuthAPI.Models;

namespace AuthAPI.Profiles {
    public class UsersProfile : Profile {
        public UsersProfile()
        {
            CreateMap<User, UserReadDto>();
            CreateMap<UserCreateDto, User>();
            CreateMap<UserUpdateDto, User>();
            CreateMap<User, UserUpdateDto>();
        }
    }
}