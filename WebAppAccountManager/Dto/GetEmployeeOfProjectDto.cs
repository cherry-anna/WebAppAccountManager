using AccountManager.Domain.Models;
using System.Collections.Generic;

namespace WebAppAccountManager.Dto
{
    public class GetEmployeeOfProjectDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Position { get; set; }
        public decimal Rate { get; set; }
    }
}
