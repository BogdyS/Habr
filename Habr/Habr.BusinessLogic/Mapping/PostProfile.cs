using AutoMapper;
using Habr.BusinessLogic.Helpers;
using Habr.Common.DTO;
using Habr.DataAccess.Entities;

namespace Habr.BusinessLogic.Mapping;

public class PostProfile : Profile
{
    public PostProfile()
    {
        CreateMap<Post, PostListDTO>()
            .ForMember(dto => dto.Posted,
                options => options.MapFrom(post => post.Posted))
            .ForMember(dto => dto.Title,
                options => options.MapFrom(post => post.Title))
            .ForMember(dto => dto.UserEmail,
                options => options.MapFrom(post => post.User.Email));

        CreateMap<Post, FullPostDTO>()
            .ForMember(dto => dto.PublishDate,
                options => options.MapFrom(post => post.Posted))
            .ForMember(dto => dto.Title,
                options => options.MapFrom(post => post.Title))
            .ForMember(dto => dto.Text,
                options => options.MapFrom(post => post.Text))
            .ForMember(dto => dto.AuthorEmail,
                options => options.MapFrom(post => post.User.Email))
            .ForMember(dto => dto.Comments,
                options => options.MapFrom(post => CommentTree.SortToTree(post.Comments, null)))
            .MaxDepth(100);

        CreateMap<Post, PostDraftDTO>()
            .ForMember(dto => dto.Created,
                options => options.MapFrom(post => post.Created))
            .ForMember(dto => dto.Title,
                options => options.MapFrom(post => post.Title))
            .ForMember(dto => dto.Updated,
                options => options.MapFrom(post => post.Updated));
    }
}
