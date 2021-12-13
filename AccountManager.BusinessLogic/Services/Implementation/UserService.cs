using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AccountManager.BusinessLogic.Services.Interfaces;
using AccountManager.DataAccess.Context;
using AccountManager.Domain.Exceptions;
using AccountManager.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.BusinessLogic.Services.Implementation
{
    public class UserService : IUserService
    {
        private readonly SignInManager<User> _signInManager;
        private readonly AccountManagerContext _context;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        public UserService(SignInManager<User> signInManager, AccountManagerContext context, RoleManager<IdentityRole<int>> roleManager)
        {
            this._signInManager = signInManager;
            _context = context;
            _roleManager = roleManager;
        }
        public async Task<User> AuthenticateUserAsync(string name, string password)
        {
            var result = await _signInManager.PasswordSignInAsync(name, password, true, false);
            if(!result.Succeeded)
            {
                throw new ExceptionAccountManager((int)HttpStatusCode.Unauthorized, $"Login or password is invalid.");
            }
            return await _signInManager.UserManager.FindByNameAsync(name);
        }
        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await _context.Users.Include(u => u.Employees).ThenInclude(e => e.Project).AsNoTracking().ToListAsync<User>();
        }
        public async Task<User> CreateUserAsync(string name, string password, int roleId)
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

            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            var result = await _signInManager.UserManager.AddToRoleAsync(insertedItem.Entity, role.Name);
            if (!result.Succeeded)
            {
                throw new ExceptionAccountManager((int)HttpStatusCode.InternalServerError, $"The role is not assigned to the user.");
            }

            return insertedItem.Entity;
        }
        public async Task UpdateUserAsync(int userId, string name, string password)
        {
            User user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                throw new ExceptionAccountManager((int)HttpStatusCode.NotFound, $"User with ID-{userId} not found.");
            }

            // update only name or password, or both 
            if (name != "" & user.UserName != name)
            {
                user.UserName = name;
            }
            
            if (password != "")
            {
                var hasher = new PasswordHasher<User>();
                string passwordHash = hasher.HashPassword(null, password);
                if (user.PasswordHash != passwordHash) 
                {
                    user.PasswordHash = passwordHash;
                }
            }
            await _context.SaveChangesAsync();
        }
        public async Task ChangeUserPasswordAsync(int userId, string oldPassword, string newPassword)
        {
            User user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                throw new ExceptionAccountManager((int)HttpStatusCode.NotFound, $"User with ID-{userId} not found.");
            }
            var hasher = new PasswordHasher<User>();
            string passwordHash = hasher.HashPassword(null, oldPassword);
            if (passwordHash != user.PasswordHash)
            {
                throw new ExceptionAccountManager((int)HttpStatusCode.BadRequest, $"Password is invalid.");
            }
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
