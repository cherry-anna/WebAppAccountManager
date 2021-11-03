using AccountManager.Domain.Models;
using System.Collections.Generic;

namespace WebAppAccountManager.Dto
{
    public class GetEmployeeDto
    {
        public string ProjectName { get; set; }
        public decimal Rate { get; set; }
        public string Position { get; set; }

    }
}
