using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace RestfulLanding.Database {
    //public class AppDbContext : DbContext {
    //    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    //    public DbSet<User> Users { get; set; }
    //    //public DbSet<Objective> Objectives { get; set; }
    //}

    public class AppIdentityDbContext : IdentityDbContext<UserModel> {
        public AppIdentityDbContext (DbContextOptions<AppIdentityDbContext> options) : base(options) { }
        public DbSet<Objective> Objectives { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Objective>().HasOne(t => t.user).WithMany().HasForeignKey(t => t.userId);
        }
    }
}
