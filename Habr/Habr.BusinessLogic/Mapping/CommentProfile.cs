using AutoMapper;
using Habr.Common.DTO;
using Habr.DataAccess.Entities;

namespace Habr.BusinessLogic.Mapping;

public class CommentProfile : Profile
{
    public CommentProfile()
    {
        CreateMap<Comment, CommentDTO>()
             .ForMember(dto => dto.AuthorName,
                 options => options.MapFrom(comment => comment.User.Name))
             .ForMember(dto => dto.Text,
                 options => options.MapFrom(comment => comment.Text))
             .ForMember(dto => dto.Comments,
                 options => options.MapFrom(comment => comment.Comments));
    }
}