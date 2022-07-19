using AutoMapper;
using Habr.Common.DTO.User;
using Habr.DataAccess.Entities;

namespace Habr.BusinessLogic.Mapping;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDTO>()
            .ForMember(dto => dto.Id,
                options => options.MapFrom(user => user.Id))
            .ForMember(dto => dto.Email,
                options => options.MapFrom(user => user.Email))
            .ForMember(dto => dto.Name,
                options => options.MapFrom(user => user.Name))
            .ForMember(dto => dto.Role,
                options => options.MapFrom(user => user.Role));

        CreateMap<RegistrationDTO, User>()
            .ForMember(user => user.Email,
                options => options.MapFrom(dto => dto.Login))
            .ForMember(user => user.Name,
                options => options.MapFrom(dto => dto.Name))
            .ForMember(user => user.Password,
                options => options.MapFrom(dto => dto.Password))
            .ForMember(user => user.DateOfBirth,
                options => options.MapFrom(dto => dto.DateOfBirth));

        CreateMap<User, LoginDTO>()
            .ForMember(dto => dto.Login,
                options => options.MapFrom(user => user.Email))
            .ForMember(dto => dto.Password,
                options => options.MapFrom(user => user.Password));

        CreateMap<User, BirthdayDTO>()
            .ForMember(dto => dto.Name,
                options => options.MapFrom(user => user.Name))
            .ForMember(dto => dto.Email,
                options => options.MapFrom(user => user.Email));
    }
}