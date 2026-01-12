namespace CloudWeather.Precipetation.DataAccess
{
    public class Precipitation
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal AmountInches { get; set; }
        public string WeatherType { get; set; } // e.g., Rain, Snow, Sleet
        public string ZipCode { get; set; }
    }
}