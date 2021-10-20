using System;
using System.Collections.Generic;
using System.Text;
using AccountManager.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace AccountManager.Domain.Models
{
    public class Employee : IdentityUser<int>, IEntity<int>
    {
       // public int Id { get; set; }
       // public User User { get; set; }

        public ICollection<Project> Projects { get; set; } 

        public Employee()
        {
            Projects = new List<Project>();
        }
    }
}
