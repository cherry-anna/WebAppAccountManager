using System;
using System.Threading.Tasks;
using AccountManager.BusinessLogic.Services.Interfaces;
using AccountManager.Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace AccountManager.BusinessLogic.Services.Implementation
{
    public class UserService : IUserService
    {
        private readonly SignInManager<User> _signInManager;

        public UserService(SignInManager<User> signInManager)
        {
            this._signInManager = signInManager;
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

        public Task<User> RegisterUserAsync(string login, string password)
        {
            throw new NotImplementedException();
        }
    }
}
