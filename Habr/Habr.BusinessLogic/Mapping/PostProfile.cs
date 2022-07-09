using AutoMapper;
using Habr.BusinessLogic.Helpers;
using Habr.Common.DTO;
using Habr.DataAccess.Entities;

namespace Habr.BusinessLogic.Mapping;

public class PostProfile : Profile
{
    public PostProfile()
    {
        CreateMap<Post, PostListDtoV1>()
            .ForMember(dto => dto.Id,
                options => options.MapFrom(post => post.Id))
            .ForMember(dto => dto.Posted,
                options => options.MapFrom(post => post.Posted))
            .ForMember(dto => dto.Title,
                options => options.MapFrom(post => post.Title))
            .ForMember(dto => dto.UserEmail,
                options => options.MapFrom(post => post.User.Email));

        CreateMap<Post, PostListDtoV2>()
            .ForMember(dto => dto.Id,
                options => options.MapFrom(post => post.Id))
            .ForMember(dto => dto.Posted,
                options => options.MapFrom(post => post.Posted))
            .ForMember(dto => dto.Title,
                options => options.MapFrom(post => post.Title))
            .ForMember(dto => dto.Author,
                options => options.MapFrom(post => new AuthorDto()
                {
                    Email = post.User.Email,
                    Name = post.User.Name
                }));

        CreateMap<Post, FullPostDTO>()
            .ForMember(dto => dto.Id,
                options => options.MapFrom(post => post.Id))
            .ForMember(dto => dto.PublishDate,
                options => options.MapFrom(post => post.Posted))
            .ForMember(dto => dto.Title,
                options => options.MapFrom(post => post.Title))
            .ForMember(dto => dto.Text,
                options => options.MapFrom(post => post.Text))
            .ForMember(dto => dto.AuthorEmail,
                options => options.MapFrom(post => post.User.Email))
            .ForMember(dto => dto.Comments,
                options => options.MapFrom(post => post.Comments))
            .BeforeMap((post, dto) => post.Comments = CommentTree.SortToTree(post.Comments));

        CreateMap<Post, PostDraftDTO>()
            .ForMember(dto => dto.Id,
                options => options.MapFrom(post => post.Id))
            .ForMember(dto => dto.Created,
                options => options.MapFrom(post => post.Created))
            .ForMember(dto => dto.Title,
                options => options.MapFrom(post => post.Title))
            .ForMember(dto => dto.Updated,
                options => options.MapFrom(post => post.Updated));

        CreateMap<CreatingPostDTO, Post>()
            .ForMember(post => post.Title,
                options => options.MapFrom(dto => dto.Title))
            .ForMember(post => post.Text,
                options => options.MapFrom(dto => dto.Text))
            .ForMember(post => post.IsDraft,
                options => options.MapFrom(dto => dto.IsDraft))
            .ForMember(post => post.UserId,
                options => options.MapFrom(dto => dto.UserId));

        CreateMap<UpdatePostDTO, Post>()
            .ForMember(post => post.Title,
                options => options.MapFrom(dto => dto.Title))
            .ForMember(post => post.Text,
                options => options.MapFrom(dto => dto.Text));
    }
}
