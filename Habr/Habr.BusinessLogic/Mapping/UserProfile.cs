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
                options => options.MapFrom(user => user.Name));
    }
}