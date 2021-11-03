using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AccountManager.BusinessLogic.Services.Interfaces;
using AccountManager.DataAccess.Context;
using AccountManager.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.BusinessLogic.Services.Implementation
{
    public class UserService : IUserService
    {
        private readonly SignInManager<User> _signInManager;
        private readonly AccountManagerContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserService(SignInManager<User> signInManager, AccountManagerContext context, IHttpContextAccessor httpContextAccessor)
        {
            this._signInManager = signInManager;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<User> AuthenticateUserAsync(string name, string password)
        {

            var result = await _signInManager.PasswordSignInAsync(name, password, true, false);
            if(!result.Succeeded)
            {
                throw new Exception();
            }
            return await _signInManager.UserManager.FindByNameAsync(name);
            
        }


        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await _context.Users.Include(u => u.Employees).ThenInclude(e => e.Project).AsNoTracking().ToListAsync<User>();
        }
        public async Task<User> CreateUserAsync(string name, string password)
        {
            var hasher = new PasswordHasher<User>();
            User user = new User
            {
                UserName = name,
                NormalizedUserName = name.ToUpper(),
                PasswordHash = hasher.HashPassword(null, password),
                SecurityStamp = string.Empty
            };

            var insertedItem = await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return insertedItem.Entity;
        }
        public async Task ChangeUserNameAsync(int userId, string newName)
        {
            User user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                throw new Exception("User is not found.");
            }
            user.UserName = newName;
            await _context.SaveChangesAsync();
        }

        public async Task ChangeUserPasswordAsync(string oldPassword, string newPassword)
        {
            HttpContext context = _httpContextAccessor.HttpContext;
            int userId = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            User user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                throw new Exception("User is not found.");
            }

            var hasher = new PasswordHasher<User>();

            string passwordHash = hasher.HashPassword(null, oldPassword);
            if (passwordHash != user.PasswordHash)
            {
                throw new Exception("Password is invalid.");
            }
            user.PasswordHash = hasher.HashPassword(null, newPassword);
            await _context.SaveChangesAsync();
        }

        public async Task SetUserPasswordAsync(int userId, string newPassword)
        {
            User user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                throw new Exception("User is not found.");
            }

            var hasher = new PasswordHasher<User>();
            user.PasswordHash = hasher.HashPassword(null, newPassword);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(int userId)
        {
            User user = _context.Users.FirstOrDefault(u => u.Id == userId);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }





    }
}
