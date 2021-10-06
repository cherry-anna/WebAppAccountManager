using System.Collections.Generic;
using AccountManager.Domain.Interfaces;

namespace AccountManager.Domain.Models
{
    public class User : IEntity<int>
    {
        public int Id { get; set; }
        public string Name { get; set; }


    }
}
