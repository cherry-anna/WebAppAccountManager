using AccountManager.Domain.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManager.DataAccess.Context
{
    public static class ApplicationDbInitializer
    {
        public static void SeedUsers(UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager)
        {
            IdentityRole<int> identityRole;
                identityRole = new IdentityRole<int>("Admin");
           roleManager.CreateAsync(identityRole).Wait();
            identityRole = new IdentityRole<int>("Manager");
            roleManager.CreateAsync(identityRole).Wait();
            identityRole = new IdentityRole<int>("Employee");
            roleManager.CreateAsync(identityRole).Wait();

            //if (userManager.FindByEmailAsync("admin@gmail.com").Result == null)
            //{
            User user = new User
                {
                     UserName = "Admin",
                     Email = "admin@gmail.com",

                };
              
                IdentityResult result = userManager.CreateAsync(user, "Admin-1").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Admin").Wait();
                }
            //}
        }


    }
}
