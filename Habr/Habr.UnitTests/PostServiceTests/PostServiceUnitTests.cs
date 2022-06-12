using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Habr.BusinessLogic.Mapping;
using Habr.BusinessLogic.Servises;
using Habr.BusinessLogic.Validation;
using Habr.Common.DTO;
using Habr.Common.DTO.User;
using Habr.DataAccess;
using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Habr.UnitTests.PostServiceTests;

public class PostServiceUnitTests : IDisposable
{
    private DataContext _dbContext;
    private IMapper _mapper;
    public PostServiceUnitTests()
    {
        _mapper = new Mapper(new MapperConfiguration(
            options =>
            {
                options.AddProfile(typeof(PostProfile));
                options.AddProfile(typeof(UserProfile));
            }));
        var options = new DbContextOptionsBuilder<DataContext>().UseInMemoryDatabase(databaseName: "test").Options;
        _dbContext = new DataContext(options);
        _mapper.Map<List<Post>>(TestingPosts()).ForEach(post => _dbContext.Posts.Add(post));
        _dbContext.SaveChanges();

        _mapper.Map<List<User>>(TestingUsers()).ForEach(user => _dbContext.Users.Add(user));
        _dbContext.SaveChanges();
    }
    [Fact]
    public async void CreatePost_GoodData_ReturnTrue()
    {
        //Arrange
        var user = await _dbContext.Users.FirstAsync();

        var postValidator = new PostValidator();
        var userValidator = new UserValidator();

        var userService = new UserService(_dbContext, _mapper, userValidator);

        var postService = new PostService(_dbContext, _mapper, userService, postValidator);

        var newPostDto = new CreatingPostDTO() {Text = "New Text", Title = "New Title", UserId = user.Id, IsDraft = false};

        //Act

        var post = await postService.CreatePostAsync(newPostDto);

        //Assert

        Assert.True(await postService.GetPostWithCommentsAsync(post.Id) is not null);
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