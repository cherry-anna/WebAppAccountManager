using System;
using System.Collections.Generic;
using System.Text;
using AccountManager.Domain.Interfaces;

namespace AccountManager.Domain.Models
{
    public class Employee : IEntity<int>
    {
        public int Id { get; set; }
        public User User { get; set; }

    }
}
