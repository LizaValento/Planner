using AutoMapper;
using Business.Models;
using Data.Entities;

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
