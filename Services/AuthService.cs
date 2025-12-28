using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TaskTrackerWeb.Entities;
using TaskTrackerWeb.Models;

namespace TaskTrackerWeb.Services
{
    public class AuthService(IDbContextFactory<TaskTrackerContext> factory, IPasswordHasher<User> hasher, AuthStateService authStateService)
    {
        private readonly IDbContextFactory<TaskTrackerContext> _factory = factory;
        private readonly IPasswordHasher<User> _hasher = hasher;
        private readonly AuthStateService _authStateService = authStateService;
        public async Task<(bool, string)> ValidateLoginData(LoginModel data)
        {
            using var ctx = await _factory.CreateDbContextAsync();
            string failedLoginError = "Account does not exist or password is wrong!";
            User? user;
            try
            {
                user = await ctx.Users.FirstOrDefaultAsync(u => u.UserEmail == data.Email);
                if (user is null) return (false, failedLoginError);
                var verifyPassword = _hasher.VerifyHashedPassword(user, user.UserPassword, data.Password);
                if (verifyPassword == PasswordVerificationResult.Failed) return (false, failedLoginError);                
            } catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return (false, "Cannot connect with the server! Try again later!");
            }

            await CreateSession(user);
            return (true, string.Empty);
        }
        public async Task<(bool, string)> AccountExist(string email)
        {
            using var ctx = await _factory.CreateDbContextAsync();
            bool userExist;
            try
            {
                userExist = await ctx.Users.AnyAsync(u => u.UserEmail == email);
            } catch
            {
                return (false, "Cannot connect with the server! Try again later!");
            }
            return (userExist, string.Empty);
        }
        public async Task<bool> AddUser(RegisterModel data)
        {
            using var ctx = await _factory.CreateDbContextAsync();
            User newUser = new()
            {
                UserName = data.Name,
                UserSurname = data.Surname,
                UserEmail = data.Email,
                UserImage = null,
                UserAdmin = false
            };
            newUser.UserPassword = _hasher.HashPassword(newUser, data.Password);
            ctx.Add<User>(newUser);
            try
            {
                await ctx.SaveChangesAsync();
            } catch
            {
                return false;
            }
            return true;
        }
        private async Task CreateSession(User user)
        {
            UserSession userSession = new()
            {
                Id = user.UserId,
                Name = user.UserName,
                Surname = user.UserSurname,
                Image = user.UserImage,
                Admin = user.UserAdmin
            };
            await _authStateService.CreateAuthenticationState(userSession);
        }
    }
}
