using AutoMapper;
using AutoMapper.QueryableExtensions;
using Habr.BusinessLogic.Helpers;
using Habr.BusinessLogic.Interfaces;
using Habr.Common.DTO;
using Habr.Common.DTO.Pagination;
using Habr.Common.Exceptions;
using Habr.Common.Resourses;
using Habr.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Habr.BusinessLogic.Servises;

public class PagedPostService : IPagedPostService
{
    private readonly DataContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IUserService _userService;
    public PagedPostService(DataContext dbContext, IMapper mapper, IUserService userService)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _userService = userService;
    }
    public async Task<PaginatedData<PostListDtoV2>> GetAllPostsPageAsync(int pageNumber, int pageSize)
    {
        var context = new PaginationContext()
        {
            PageIndex = --pageNumber,
            PageSize = pageSize
        };

        var response = await _dbContext.Posts
            .Where(p => !p.IsDraft)
            .Include(p => p.User)
            .ProjectTo<PostListDtoV2>(_mapper.ConfigurationProvider)
            .GetPagedDataAsync(context);

        return response;
    }

    public async Task<PaginatedData<PostListDtoV1>?> GetUserPostsPageAsync(int userId, int pageNumber, int pageSize)
    {
        if (await _userService.IsUserExistsAsync(userId) is null)
        {
            throw new NotFoundException(ExceptionMessages.UserNotFound);
        }
        var context = new PaginationContext()
        {
            PageIndex = --pageNumber,
            PageSize = pageSize
        };

        var response = await _dbContext.Posts
            .Where(p => p.UserId == userId)
            .Where(p => !p.IsDraft)
            .ProjectTo<PostListDtoV1>(_mapper.ConfigurationProvider)
            .GetPagedDataAsync(context);

        return response;
    }

    public async Task<PaginatedData<PostDraftDTO>?> GetUserDraftsPageAsync(int userId, int pageNumber, int pageSize)
    {
        if (await _userService.IsUserExistsAsync(userId) is null)
        {
            throw new NotFoundException(ExceptionMessages.UserNotFound);
        }

        var context = new PaginationContext()
        {
            PageIndex = --pageNumber,
            PageSize = pageSize
        };

        var response = await _dbContext.Posts
            .Where(p => p.UserId == userId)
            .Where(p => p.IsDraft)
            .ProjectTo<PostDraftDTO>(_mapper.ConfigurationProvider)
            .GetPagedDataAsync(context);

        return response;
    }
}