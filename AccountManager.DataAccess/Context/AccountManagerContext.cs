using Microsoft.EntityFrameworkCore;
using AccountManager.Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using AccountManager.Domain.Interfaces;

namespace AccountManager.DataAccess.Context
{
    public class AccountManagerContext : IdentityDbContext<User, IdentityRole, string>, IUnitOfWork
    {
        //public DbSet<User> Users { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Project> Projects { get; set; }

        public AccountManagerContext(DbContextOptions<AccountManagerContext> options) : base(options)
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Report>(b =>
            {
                b.HasKey(x => x.Id);
                b.HasOne(x => x.Employee);
                    //.WithMany(u => u.ToDoItems)
                    //.HasForeignKey(x => x.UserId);
                b.Property(x => x.Description)
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<User>(b =>
            {
                b.HasKey(x => x.Id);
            });
        }
    }
}
