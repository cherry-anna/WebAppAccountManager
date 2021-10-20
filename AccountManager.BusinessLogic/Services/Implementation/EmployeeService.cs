using AccountManager.BusinessLogic.Services.Interfaces;
using AccountManager.DataAccess.Repositories.Interfaces;
using AccountManager.Domain.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManager.BusinessLogic.Services.Implementation
{
    public class EmployeeService : IEmployeeService
    {
        private readonly UserManager<User> _userManager;
        //private readonly IUserRepository _userRepository;
        private readonly IEmployeeRepository _employeeRepository;
        public EmployeeService(UserManager<User> userManager, IEmployeeRepository employeeRepository)
        {
            _userManager = userManager;
            _employeeRepository = employeeRepository;
        }

        public async Task<IEnumerable<Employee>> GetEmployeesAsync()
        {
            return await _employeeRepository.GetAllAsync();
        } 
        public async Task<Employee> CreateEmployeeAsync(string name, string password)
        {
            var user = new User { UserName = name};
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                throw new Exception();
            }
            Employee employee = new Employee {User=user};
            var insertedItem = await _employeeRepository.InsertAsync(employee);
            await _employeeRepository.UnitOfWork.SaveChangesAsync();

            return insertedItem;
        }
        //public async Task<Employee> ChangeNameEmployeeAsync(int employeeId, string newName)
        //{
        //    throw new NotImplementedException();
        //}

        //public async Task<Employee> ChangePasswordEmployeeAsync(int employeeId, string newPassword)
        //{
        //    throw new NotImplementedException();
        //}

       
        public async Task DeleteEmployeeAsync(int employeeId)
        {
            Employee employee = await _employeeRepository.GetByIdAsync(employeeId);
            await _employeeRepository.DeleteAsync(employee);
            await _employeeRepository.UnitOfWork.SaveChangesAsync();
        }

    }
}
