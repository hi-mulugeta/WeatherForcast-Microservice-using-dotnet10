using Microsoft.EntityFrameworkCore;

namespace CloudWeather.Report.DataAccess
{
    public class WeatherReportDbContext: DbContext
    {
        public WeatherReportDbContext(DbContextOptions<WeatherReportDbContext> options) : base(options)
        {
        }

        public DbSet<WeatherReport> WeatherReports { get; set; }  

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            SnakeCaseIdentityTableNames(modelBuilder);
        }
        private static void SnakeCaseIdentityTableNames(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WeatherReport>(entity =>
            {
                entity.ToTable("weather_report");
            });
        }
    }
}