using System;
using System.Threading.Tasks;
using FullStackDevExercise.Models;
using FullStackDevExercise.Repos;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Tests.tests
{
    public class AuthTests
    {
        [Fact]
        public async Task RegisterPasswordCreatedTest()
        {
            
            var dataContext = DbContextMock.getDataContext(nameof(RegisterPasswordCreatedTest));
            var _repo = new AuthRepository(dataContext);
            var user = new User
                {
                    Email = "Nick.Chlam@rht.com",
                    FirstName = "Nick",
                    LastName = "Chlam",
                    UserName = "NickChlam"
                };
            
            var userCreated = await _repo.Register(user, "password");
            var userFromDb = await dataContext.Users.FirstOrDefaultAsync(x => x.UserName == user.UserName);
            var users = await dataContext.Users.ToListAsync();

            dataContext.Dispose();

            Assert.NotEmpty(userCreated.PasswordHash);
            Assert.NotEmpty(userFromDb.PasswordHash);
            Assert.InRange(userFromDb.UserId, 0, 5);
            Assert.Equal(user.UserName, userFromDb.UserName);
            Assert.Single(users );
            
        }

        [Fact]
        public async Task LoginTest()
        {
            var context = DbContextMock.getDataContext(nameof(LoginTest));
            var _repo = new AuthRepository(context);

            var user = new User
                {
                    Email = "Nick.Chlam@rht.com",
                    FirstName = "Nick",
                    LastName = "Chlam",
                    UserName = "NickChlam"
                };
            
            var UserCreated = await _repo.Register(user, "password");

            var loggedInUser = await _repo.Login("NickChlam", "password");

            context.Dispose();

            Assert.Equal(user.UserName, loggedInUser.UserName);
            Assert.NotNull(loggedInUser);

        }

        
        [Theory]
        [InlineData("Password")]
        [InlineData("PAssword")]
        [InlineData("PaSSword")]
        [InlineData("PassworD")]
        public async Task LoginInvalidTest(string password)
        {
            var context = DbContextMock.getDataContext(nameof(LoginInvalidTest));
            var _repo = new AuthRepository(context);

            var user = new User
                {
                    Email = "Nick.Chlam@rht.com",
                    FirstName = "Nick",
                    LastName = "Chlam",
                    UserName = "NickChlam"
                };
            
            var UserCreated = await _repo.Register(user, "password");

            var loggedInUser = await _repo.Login("NickChlam", password);

            context.Dispose();

            Assert.Null(loggedInUser);
        }

        [Fact]
        public async Task UserExistsTest()
        {
            var context = DbContextMock.getDataContext(nameof(UserExistsTest));
            var _repo = new AuthRepository(context);

            var user = new User
                {
                    Email = "Nick.Chlam@rht.com",
                    FirstName = "Nick",
                    LastName = "Chlam",
                    UserName = "NickChlam"
                };

            var UserCreated = await _repo.Register(user, "password");

            var userExists = await _repo.UserExists("NickChlam");

            context.Dispose();
            Assert.True(userExists);

        }
         [Fact]
        public async Task UserDoesNotExistsTest()
        {
            var context = DbContextMock.getDataContext(nameof(UserDoesNotExistsTest));
            var _repo = new AuthRepository(context);

            var user = new User
                {
                    Email = "Nick.Chlam@rht.com",
                    FirstName = "Nick",
                    LastName = "Chlam",
                    UserName = "NickChlam"
                };

            var UserCreated = await _repo.Register(user, "password");

            var userExists = await _repo.UserExists("SarahRose");

            context.Dispose();
            Assert.False(userExists);

        }
    }
}
