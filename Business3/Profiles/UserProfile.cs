using AutoMapper;
using Application.DTOs;
using Domain.Entities;

namespace Business.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserModel, User>();
            CreateMap<User, UserModel>();
        }
    }
}
