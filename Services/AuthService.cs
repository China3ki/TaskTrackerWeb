using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TaskTrackerWeb.Entities;
using TaskTrackerWeb.Models;

namespace TaskTrackerWeb.Services
{
    public class AuthService(IDbContextFactory<TaskTrackerContext> factory)
    {
        private readonly IDbContextFactory<TaskTrackerContext> _factory = factory;
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
            PasswordHasher<User> hasher = new PasswordHasher<User>();
            User newUser = new()
            {
                UserName = data.Name,
                UserSurname = data.Surname,
                UserEmail = data.Email,
            };
            newUser.UserPassword = hasher.HashPassword(newUser, data.Password);
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
    }
}
