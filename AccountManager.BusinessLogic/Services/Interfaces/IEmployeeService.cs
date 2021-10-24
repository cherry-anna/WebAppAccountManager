using System.Collections.Generic;
using System.Threading.Tasks;
using AccountManager.Domain.Models;

namespace AccountManager.BusinessLogic.Services.Interfaces
{
    public interface IEmployeeService
    {
        Task<IEnumerable<Employee>> GetEmployeesAsync();
        
        Task<Employee> CreateEmployeeAsync(string name, string password);
        Task<Employee> ChangeNameEmployeeAsync(int employeeId, string newName);
        Task ChangePasswordEmployeeAsync(int employeeId, string newPassword);
        Task DeleteEmployeeAsync(int employeeId);
    }
}
