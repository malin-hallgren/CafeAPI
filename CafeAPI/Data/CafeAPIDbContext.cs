using CafeAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CafeAPI.Data
{
    public class CafeAPIDbContext : DbContext
    {
        public CafeAPIDbContext(DbContextOptions<CafeAPIDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Product>().Property(p => p.Price).HasColumnType("decimal(18,2)");
        }
    }
}
