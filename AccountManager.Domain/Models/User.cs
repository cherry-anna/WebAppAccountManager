using System.Collections.Generic;
using AccountManager.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace AccountManager.Domain.Models
{
    public class User : IdentityUser<int>, IEntity<int>
    {
        //public int Id { get; set; }
        //public string Name { get; set; }


    }
}
