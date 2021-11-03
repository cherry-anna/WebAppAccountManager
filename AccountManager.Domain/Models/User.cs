using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace AccountManager.Domain.Models
{
    public class User : IdentityUser<int>
    {
        public ICollection<Employee> Employees { get; set; } 

        public User()
        {
            Employees = new List<Employee>();
        }
    }
}
