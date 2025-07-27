using AutoMapper;
using BaseApi.Application.Dtos;
using BaseApi.Domain.Entities;

namespace BaseApi.Application.Mappings;

public class UserProfile : Profile
{
    public UserProfile()
    {
        // Entity to DTO
        CreateMap<ApplicationUser, UserDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.GetFullName()));

        // DTO to Entity
        CreateMap<RegisterDto, ApplicationUser>().ReverseMap();

        CreateMap<UpdateUserDto, ApplicationUser>().ReverseMap();
    }
} 