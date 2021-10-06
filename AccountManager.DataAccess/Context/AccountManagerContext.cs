using Microsoft.EntityFrameworkCore;
using AccountManager.DataAccess.Repositories.Interfaces;
using AccountManager.Domain.Models;

namespace AccountManager.DataAccess.Context
{
    public class AccountManagerContext : DbContext, IUnitOfWork
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Project> Projects { get; set; }

        public AccountManagerContext(DbContextOptions<AccountManagerContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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
