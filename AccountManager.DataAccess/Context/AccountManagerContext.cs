using Microsoft.EntityFrameworkCore;
using AccountManager.Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using AccountManager.Domain.Interfaces;

namespace AccountManager.DataAccess.Context
{
    public class AccountManagerContext : IdentityDbContext<User, IdentityRole<int>, int>, IUnitOfWork
    {
        //public DbSet<User> Users { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Project> Projects { get; set; }

        public AccountManagerContext(DbContextOptions<AccountManagerContext> options) : base(options)
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Report>(r =>
            {
                r.HasKey(x => x.Id);
                r.HasOne(x => x.Employee);
                    //.WithMany(u => u.ToDoItems)
                    //.HasForeignKey(x => x.UserId);
                r.Property(x => x.Description)
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<User>(u =>
            {
                u.HasKey(x => x.Id);
            });

            modelBuilder.Entity<IdentityRole<int>>(r =>
            {
                r.HasKey(x => x.Id);
                //r.HasData(new IdentityRole<int>("Admin") { Id = 1}, new IdentityRole<int>("Manager") { Id = 2 }, new IdentityRole<int>("Employee") { Id = 3 });
            });
        }
    }
}
