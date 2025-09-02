using Microsoft.EntityFrameworkCore;
using Chat.Models;

namespace Chat.Contexts
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserSignalR> UserSignalRs { get; set; }
        public DbSet<GroupSignalR> GroupSignalRs { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình khóa ngoại cho OwnerId
            modelBuilder.Entity<Group>()
                .HasOne(g => g.User)
                .WithMany()   
                .HasForeignKey(g => g.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
               .HasMany(s => s.Groups)
               .WithMany(c => c.Users)
               .UsingEntity(j => j.ToTable("UserGroups"));

            modelBuilder.Entity<User>()
                .HasMany(s => s.Roles)
                .WithMany(c => c.Users)
                .UsingEntity(j => j.ToTable("UserRoles"));

            modelBuilder.Entity<Role>()
                .HasMany(s => s.Permissions)
                .WithMany(c => c.Roles)
                .UsingEntity(j => j.ToTable("RolePermissions"));
        }
    }
}