using Microsoft.EntityFrameworkCore;
using AccountManager.Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace AccountManager.DataAccess.Context
{
    public class AccountManagerContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public DbSet<User> Users { get; set; }
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

            modelBuilder.Entity<Project>()
             .HasMany(e => e.Employees)
             .WithOne(p => p.Project);

            modelBuilder.Entity<Report>(r =>
            {
                r.HasKey(x => x.Id);
                //r.HasOne(x => x.Employee)
                  //  .WithMany(u => u.)
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
                r.HasData(new IdentityRole<int> {Id=1, Name = "Admin", NormalizedName = "Admin".ToUpper() },
                    new IdentityRole<int> { Id = 2, Name = "Manager", NormalizedName = "Manager".ToUpper() },
                    new IdentityRole<int> { Id = 3, Name = "Employee", NormalizedName = "Employee".ToUpper() }
                    );
                //r.HasData(new IdentityRole<int>("Admin") { Id = 1}, new IdentityRole<int>("Manager") { Id = 2 }, new IdentityRole<int>("Employee") { Id = 3 });
            });

            var hasher = new PasswordHasher<User>();
            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 1,
                UserName = "Admin",
                NormalizedUserName = "admin".ToUpper(),
                Email = "admin@gmail.com",
                NormalizedEmail = "admin@gmail.com".ToUpper(),
                EmailConfirmed = true,
                PasswordHash = hasher.HashPassword(null, "Admin-1"),
                SecurityStamp = string.Empty
            });

            modelBuilder.Entity<IdentityUserRole<int>>().HasData(new IdentityUserRole<int>
            {
                RoleId = 1,
                UserId = 1
            });
        }
    }
}
