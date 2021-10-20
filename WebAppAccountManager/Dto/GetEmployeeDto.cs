using AccountManager.Domain.Models;
using System.Collections.Generic;

namespace WebAppAccountManager.Dto
{
    public class GetEmployeeDto
    {
        public int Id { get; set; }
        public User userName { get; set; }
        public ICollection<Project> Projects { get; set; }

    }
}
