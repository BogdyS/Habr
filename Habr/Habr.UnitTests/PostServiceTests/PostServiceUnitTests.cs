using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Habr.BusinessLogic.Mapping;
using Habr.BusinessLogic.Servises;
using Habr.Common.DTO;
using Habr.Common.DTO.User;
using Habr.Common.Exceptions;
using Habr.DataAccess;
using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Habr.UnitTests.PostServiceTests;

public class PostServiceUnitTests : IDisposable
{
    private readonly DataContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IValidator<IPost> _postValidator;
    private readonly IValidator<RegistrationDTO> _userValidator;
    public PostServiceUnitTests()
    {
        _mapper = new Mapper(new MapperConfiguration(
            options =>
            {
                options.AddProfile(typeof(PostProfile));
                options.AddProfile(typeof(UserProfile));
            }));

        var postValidatorMock = new Mock<IValidator<IPost>>();
        postValidatorMock.Setup(validator =>
                validator.ValidateAsync(It.IsAny<IPost>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult()); //validationResult.IsValid always return true
        _postValidator = postValidatorMock.Object;

        var userValidatorMock = new Mock<IValidator<RegistrationDTO>>();
        userValidatorMock.Setup(validator =>
                validator.ValidateAsync(It.IsAny<RegistrationDTO>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult()); //validationResult.IsValid always return true
        _userValidator = userValidatorMock.Object;

        var options = new DbContextOptionsBuilder<DataContext>().UseInMemoryDatabase(databaseName: "test").Options;
        _dbContext = new DataContext(options);
        _mapper.Map<List<Post>>(TestingPosts()).ForEach(post => _dbContext.Posts.Add(post));

        _mapper.Map<List<User>>(TestingUsers()).ForEach(user => _dbContext.Users.Add(user));
        _dbContext.SaveChanges();
    }

    [Fact]
    public async void CreatePost_GoodData_ReturnTrue()
    {
        //Arrange
        var user = await _dbContext.Users.FirstAsync();

        var userService = new UserService(_dbContext, _mapper, _userValidator);

        var postService = new PostService(_dbContext, _mapper, userService, _postValidator);

        var newPostDto = new CreatingPostDTO() { Text = "New Text", Title = "New Title", UserId = user.Id, IsDraft = false };

        //Act

        var post = await postService.CreatePostAsync(newPostDto);

        //Assert

        Assert.True(await postService.GetPostWithCommentsAsync(post.Id) is not null);
    }

    [Fact]
    public async void CreatePost_UserNotExists_ThrowsNotFoundException()
    {
        //Arrange
        int id = 1;
        while (await _dbContext.Users.Where(user => user.Id == id).FirstOrDefaultAsync() is not null)
        {
            id++;
        }

        var userService = new UserService(_dbContext, _mapper, _userValidator);
        
        var postService = new PostService(_dbContext, _mapper, userService, _postValidator);

        var newPostDto = new CreatingPostDTO() { Text = "New Text", Title = "New Title", UserId = id, IsDraft = false };

        //Assert
        await Assert.ThrowsAsync<NotFoundException>(
            async () => await postService.CreatePostAsync(newPostDto));
    }

    [Fact]
    public async void PublicPost_GoodData_ReturnTrue()
    {
        //Arrange
        var user = await _dbContext.Users.FirstAsync();

        var userService = new UserService(_dbContext, _mapper, _userValidator);

        var postService = new PostService(_dbContext, _mapper, userService, _postValidator);

        var newPostDto = new CreatingPostDTO() { Text = "New Text", Title = "New Title", UserId = user.Id, IsDraft = true };
        var post = await postService.CreatePostAsync(newPostDto);

        //Act
        await postService.PostFromDraftAsync(post.Id, user.Id);

        //Assert
        Assert.True((await _dbContext.Posts.Where(p => p.Id == post.Id).SingleAsync()).IsDraft == false);
    }

    [Fact]
    public async void PublicPost_AnotherUser_ThrowsAccessException()
    {
        //Arrange
        var users = await _dbContext.Users.Take(2).ToListAsync();

        var userService = new UserService(_dbContext, _mapper, _userValidator);

        var postService = new PostService(_dbContext, _mapper, userService, _postValidator);

        var newPostDto = new CreatingPostDTO() { Text = "New Text", Title = "New Title", UserId = users[0].Id, IsDraft = true };
        var post = await postService.CreatePostAsync(newPostDto);

        //Assert
        await Assert.ThrowsAsync<AccessException>(async () => await postService.PostFromDraftAsync(post.Id, users[1].Id));
    }

    [Fact]
    public async void PublicPost_PostNotDraft_ThrowsAccessException()
    {
        //Arrange
        var user = await _dbContext.Users.FirstAsync();

        var userService = new UserService(_dbContext, _mapper, _userValidator);

        var postService = new PostService(_dbContext, _mapper, userService, _postValidator);

        var newPostDto = new CreatingPostDTO() { Text = "New Text", Title = "New Title", UserId = user.Id, IsDraft = false };
        var post = await postService.CreatePostAsync(newPostDto);

        //Assert
        await Assert.ThrowsAsync<AccessException>(async () => await postService.PostFromDraftAsync(post.Id, user.Id));
    }

    [Fact]
    public async void PostToDraft_GoodData_ReturnTrue()
    {
        //Arrange
        var user = await _dbContext.Users.FirstAsync();

        var userService = new UserService(_dbContext, _mapper, _userValidator);

        var postService = new PostService(_dbContext, _mapper, userService, _postValidator);

        var newPostDto = new CreatingPostDTO() { Text = "New Text", Title = "New Title", UserId = user.Id, IsDraft = false };
        var post = await postService.CreatePostAsync(newPostDto);

        //Act
        await postService.RemovePostToDraftsAsync(post.Id, user.Id);

        //Assert
        Assert.True((await _dbContext.Posts.Where(p => p.Id == post.Id).SingleAsync()).IsDraft);
    }

    [Fact]
    public async void PostToDraft_AnotherUser_ThrowsAccessException()
    {
        //Arrange
        var users = await _dbContext.Users.Take(2).ToListAsync();

        var userService = new UserService(_dbContext, _mapper, _userValidator);

        var postService = new PostService(_dbContext, _mapper, userService, _postValidator);

        var newPostDto = new CreatingPostDTO() { Text = "New Text", Title = "New Title", UserId = users[0].Id, IsDraft = false };
        var post = await postService.CreatePostAsync(newPostDto);

        //Assert
        await Assert.ThrowsAsync<AccessException>(async () => await postService.RemovePostToDraftsAsync(post.Id, users[1].Id));
    }

    [Fact]
    public async void PostToDraft_PostAlreadyDraft_ThrowsAccessException()
    {
        //Arrange
        var user = await _dbContext.Users.FirstAsync();

        var userService = new UserService(_dbContext, _mapper, _userValidator);

        var postService = new PostService(_dbContext, _mapper, userService, _postValidator);

        var newPostDto = new CreatingPostDTO() { Text = "New Text", Title = "New Title", UserId = user.Id, IsDraft = true };
        var post = await postService.CreatePostAsync(newPostDto);

        //Assert
        await Assert.ThrowsAsync<AccessException>(async () => await postService.RemovePostToDraftsAsync(post.Id, user.Id));
    }

    [Fact]
    public async void DeletePost_GoodData_ReturnTrue()
    {
        //Arrange
        var user = await _dbContext.Users.FirstAsync();

        var userService = new UserService(_dbContext, _mapper, _userValidator);

        var postService = new PostService(_dbContext, _mapper, userService, _postValidator);

        var post = await _dbContext.Posts.FirstAsync();

        //Act
        await postService.DeletePostAsync(post.Id, user.Id);

        //Assert
        await Assert.ThrowsAsync<NotFoundException>(async () => await postService.GetPostWithCommentsAsync(post.Id));
    }

    [Fact]
    public async void DeletePost_AnotherUser_ThrowsAccessException()
    {
        //Arrange

        var userService = new UserService(_dbContext, _mapper, _userValidator);

        var postService = new PostService(_dbContext, _mapper, userService, _postValidator);

        var post = await _dbContext.Posts.FirstAsync();
        var user = await _dbContext.Users.FirstAsync(u => u.Id != post.UserId);
        //Assert
        await Assert.ThrowsAsync<AccessException>(
            async () => await postService.DeletePostAsync(post.Id, user.Id));
    }

    public static IEnumerable<CreatingPostDTO> TestingPosts()
    {
        return new List<CreatingPostDTO>()
        {
            new CreatingPostDTO() {Text = "Some Text", Title = "Some Title", UserId = 1, IsDraft = false},
            new CreatingPostDTO() {Text = "Some Text", Title = "Some Title", UserId = 2, IsDraft = false},
            new CreatingPostDTO() {Text = "Some Text", Title = "Some Title", UserId = 3, IsDraft = true},
            new CreatingPostDTO() {Text = "Some Text", Title = "Some Title", UserId = 1, IsDraft = true},
            new CreatingPostDTO() {Text = "Some Text", Title = "Some Title", UserId = 2, IsDraft = true}
        };
    }

    public static IEnumerable<RegistrationDTO> TestingUsers()
    {
        return new List<RegistrationDTO>()
        {
            new RegistrationDTO() {Login = "someEmail1@gmail.com", Password = "SomePassword", Name = "UserName"},
            new RegistrationDTO() {Login = "someEmail2@gmail.com", Password = "SomePassword", Name = "UserName"},
            new RegistrationDTO() {Login = "someEmail3@gmail.com", Password = "SomePassword", Name = "UserName"},
            new RegistrationDTO() {Login = "someEmail4@gmail.com", Password = "SomePassword", Name = "UserName"}
        };
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }
}