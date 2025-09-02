using AutoMapper;
using Application.DTOs;
using Domain.Entities;

namespace Application.Profiles
{
    public class TokenProfile : Profile
    {
        public TokenProfile()
        {
            CreateMap<RefreshTokenModel, RefreshToken>(); 
            CreateMap<RefreshToken, RefreshTokenModel>();
        }
    }
}
