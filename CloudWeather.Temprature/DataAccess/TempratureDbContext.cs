using Microsoft.EntityFrameworkCore;

namespace CloudWeather.Temprature.DataAccess
{
    public class TempratureDbContext : DbContext
    {
        public TempratureDbContext(DbContextOptions<TempratureDbContext> options) : base(options)
        {
        }

        public DbSet<Temprature> Tempratures { get; set; }  
         protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            SnakeCaseIdentityTableNames(modelBuilder);
        }
        private static void SnakeCaseIdentityTableNames(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Temprature>(entity =>
            {
                entity.ToTable("temprature");
            });
        }
    }
}