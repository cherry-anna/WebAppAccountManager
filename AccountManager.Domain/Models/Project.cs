using System;
using System.Collections.Generic;
using System.Text;
using AccountManager.Domain.Interfaces;

namespace AccountManager.Domain.Models
{
    public class Project : IEntity<int>
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
