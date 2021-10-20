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
        private readonly UserManager<Employee> _userManager;
        //private readonly IUserRepository _userRepository;
        private readonly IEmployeeRepository _employeeRepository;
        public EmployeeService(UserManager<Employee> userManager, IEmployeeRepository employeeRepository)
        {
            _userManager = userManager;
            _employeeRepository = employeeRepository;
        }

        public async Task<IEnumerable<Employee>> GetEmployeesAsync()
        {
            return await _employeeRepository.GetAllWithProjectsAsync();
        } 
        public async Task<Employee> CreateEmployeeAsync(string name, string password)
        {
            Employee employee = new Employee { UserName = name};
            var result = await _userManager.CreateAsync(employee, password);
            if (!result.Succeeded)
            {
                throw new Exception(result.ToString());
            }
            
            //var insertedItem = await _employeeRepository.InsertAsync(employee);
            await _employeeRepository.UnitOfWork.SaveChangesAsync();

            return employee;
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
