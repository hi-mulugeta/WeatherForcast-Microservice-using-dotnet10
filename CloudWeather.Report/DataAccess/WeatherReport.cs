namespace CloudWeather.Report.DataAccess
{
    public class WeatherReport
    {
        
        public Guid Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public decimal AverageTempHighF { get; set; }
        public decimal AverageTempLowF { get; set; }    
        public decimal RainFallTotalInches { get; set; }
        public decimal SnowFallTotalInches { get; set; }
        public string ZipCode { get; set; }
    }
}       