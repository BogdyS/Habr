using AutoMapper;
using Habr.Common.DTO;
using Habr.DataAccess.Entities;

namespace Habr.BusinessLogic.Mapping;

public class CommentProfile : Profile
{
    public CommentProfile()
    {
        CreateMap<Comment, CommentDTO>()
            .ForMember(dto => dto.Id,
                options => options.MapFrom(comment => comment.Id))
            .ForMember(dto => dto.AuthorName,
                options => options.MapFrom(comment => comment.User.Name))
            .ForMember(dto => dto.Text,
                options => options.MapFrom(comment => comment.Text))
            .ForMember(dto => dto.Comments,
                options => options.MapFrom(comment => comment.Comments));

        CreateMap<CreateCommentDTO, Comment>()
            .ForMember(comment => comment.PostId,
                options => options.MapFrom(dto => dto.PostId))
            .ForMember(comment => comment.Text,
                options => options.MapFrom(dto => dto.Text))
            .ForMember(comment => comment.UserId,
                options => options.MapFrom(dto => dto.UserId));
    }
}