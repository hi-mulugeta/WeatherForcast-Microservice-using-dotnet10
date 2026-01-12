using Microsoft.EntityFrameworkCore;
using CloudWeather.Precipetation.DataAccess;

namespace CloudWeather.Precipetation.DataAccess
{
  
    
    public class PricipDbContext : DbContext
    {
        public PricipDbContext()
        {
        }
        public PricipDbContext(DbContextOptions<PricipDbContext> options) : base(options)
        {
        }

        public DbSet<Precipitation> Precipitations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            SnakeCaseIdentityTableNames(modelBuilder);
        }
        private static void SnakeCaseIdentityTableNames(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Precipitation>(entity =>
            {
                entity.ToTable("precipitation");
            });
        }
    }
}   