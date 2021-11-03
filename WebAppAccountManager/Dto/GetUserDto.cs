using AccountManager.Domain.Models;
using System.Collections.Generic;

namespace WebAppAccountManager.Dto
{
    public class GetUserDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public ICollection<GetEmployeeDto> Employees { get; set; }
        
    }
}
