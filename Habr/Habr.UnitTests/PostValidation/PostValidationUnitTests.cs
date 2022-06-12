using System.Collections.Generic;
using Habr.BusinessLogic.Validation;
using Habr.Common.DTO;
using Xunit;

namespace Habr.UnitTests
{
    public class PostValidationUnitTests
    {
        [Fact]
        public void IsValidPost_GoodData_ReturnsTrue()
        {
            //Arrange
            var postValidator = new PostValidator();
            var post = new CreatingPostDTO()
            {
                Text = "Some text",
                Title = "Some Title",
                UserId = 1,
                IsDraft = false
            };

            //Act
            var isValid = postValidator.Validate(post).IsValid;

            //Assert

            Assert.True(isValid);
        }

        [Theory]
        [MemberData(nameof(PostsForOneArgumentNullTest))]
        public void IsValidPost_OneArgumentNull_ReturnsFalse(IPost post)
        {
            //Arrange
            var postValidator = new PostValidator();

            //Act
            var isValid = postValidator.Validate(post).IsValid;

            //Assert

            Assert.False(isValid);
        }

        [Theory]
        [MemberData(nameof(PostsForOneArgumentTooBigTest))]
        public void IsValidPost_OneArgumentTooBig_ReturnsFalse(IPost post)
        {
            //Arrange
            var postValidator = new PostValidator();

            //Act
            var isValid = postValidator.Validate(post).IsValid;

            //Assert

            Assert.False(isValid);
        }

        public static IEnumerable<object[]> PostsForOneArgumentNullTest()
        {
            return new List<object[]>()
            {
               new object[] {new CreatingPostDTO()
                {
                    IsDraft = false,
                    Text = null,
                    Title = "Some Title",
                    UserId = 1
                }},
               new object[] {new CreatingPostDTO()
                {
                    IsDraft = false,
                    Text = "Some Text",
                    Title = null,
                    UserId = 1
                }}
            };
        }

        public static IEnumerable<object[]> PostsForOneArgumentTooBigTest()
        {
            return new List<object[]>()
            {
                new object[] {new CreatingPostDTO()
                {
                    IsDraft = false,
                    Text = new string('.', 5000),
                    Title = "Some Title",
                    UserId = 1
                }},
                new object[] {new CreatingPostDTO()
                {
                    IsDraft = false,
                    Text = "Some Text",
                    Title = new string('.', 5000),
                    UserId = 1
                }}
            };
        }
    }
}