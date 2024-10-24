using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace RestfulLanding.Database {
    //public class AppDbContext : DbContext {
    //    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    //    public DbSet<User> Users { get; set; }
    //    //public DbSet<Objective> Objectives { get; set; }
    //}

    public class AppIdentityDbContext : IdentityDbContext<User> {
        public AppIdentityDbContext (DbContextOptions<AppIdentityDbContext> options) : base(options) { }
    }
}
