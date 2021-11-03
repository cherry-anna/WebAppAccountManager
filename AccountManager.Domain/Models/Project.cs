using System.Collections.Generic;

namespace AccountManager.Domain.Models
{
    public class Project 
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<Employee> Employees { get; set; }

        public Project()
        {
            Employees = new List<Employee>();
        }
    }

}
