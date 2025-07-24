using Microsoft.EntityFrameworkCore;

namespace DemoAppApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Login> Logins { get; set; }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<Notice> Notices { get; set; }
    }
}