using AccountManager.BusinessLogic.Services.Interfaces;
using AccountManager.DataAccess.Context;
using AccountManager.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

//namespace AccountManager.BusinessLogic.Services.Implementation
//{
    //public class EmployeeService : IEmployeeService
    //{
    //    private readonly AccountManagerContext _context;

    //    private readonly IHttpContextAccessor _httpContextAccessor;
    //    public EmployeeService(AccountManagerContext context, IHttpContextAccessor httpContextAccessor)
    //    {
    //        _context=context;
    //        _httpContextAccessor = httpContextAccessor;
    //    }

        //public async Task<IEnumerable<Employee>> GetEmployeesAsync()
        //{
        //    return await _context.Employees.Include(e => e.Project).Include(e => e.User).AsNoTracking().ToListAsync<Employee>();
        //} 
        //public async Task<User> CreateUserAsync(string name, string password)
        //{
        //    var hasher = new PasswordHasher<User>();
        //    User user = new User
        //    {
        //        UserName = name,
        //        NormalizedUserName = name.ToUpper(),
        //        PasswordHash = hasher.HashPassword(null, password),
        //        SecurityStamp = string.Empty
        //    };

        //    var insertedItem = await _context.Users.AddAsync(user);
        //    await _context.SaveChangesAsync();
            
        //    return insertedItem.Entity;
        //}
        //public async Task ChangeNameEmployeeAsync(int employeeId, string newName)
        //{
        //    User employee = _context.Employees.FirstOrDefault(e=>e.Id == employeeId);
        //    if (employee==null)
        //    {
        //        throw new Exception("User not found.");
        //    }
        //    employee.UserName = newName;
        //    await _context.SaveChangesAsync();
        //}

        //public async Task ChangePasswordEmployeeAsync(string oldPassword, string newPassword)
        //{
        //    HttpContext context = _httpContextAccessor.HttpContext;
        //    int employeeId = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier).Value);

        //    User employee = _context.Employees.FirstOrDefault(u => u.Id == employeeId);
        //    if (employee == null)
        //    {
        //        throw new Exception("User not found.");
        //    }

        //    var hasher = new PasswordHasher<User>();

        //    string passwordHash = hasher.HashPassword(null, oldPassword);
        //    if (passwordHash != oldPassword)
        //    {
        //        throw new Exception("Password invalid.");
        //    }
        //    employee.PasswordHash= hasher.HashPassword(null, newPassword);
        //    await _context.SaveChangesAsync();
        //}

        //public async Task SetPasswordEmployeeAsync(int employeeId, string newPassword)
        //{
        //    User employee = _context.Employees.FirstOrDefault(u => u.Id == employeeId);
        //    if (employee == null)
        //    {
        //        throw new Exception("User not found.");
        //    }

        //    var hasher = new PasswordHasher<User>();
        //    employee.PasswordHash = hasher.HashPassword(null, newPassword);
        //    await _context.SaveChangesAsync();
        //}

        //public async Task DeleteEmployeeAsync(int employeeId)
        //{
        //    User employee = _context.Employees.FirstOrDefault(u => u.Id == employeeId);
        //    _context.Employees.Remove(employee);
            //await _context.SaveChangesAsync();
//        }

//    }
//}
