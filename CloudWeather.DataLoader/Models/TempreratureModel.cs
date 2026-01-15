namespace CloudWeather.DataLoader.Models
{
    internal class TemperatureModel
    {
        public DateTime CreatedAt { get; set; }
        public decimal TempHighF { get; set; }
        public decimal TempLowF { get; set; }    
        public string ZipCode { get; set; }
    }
}