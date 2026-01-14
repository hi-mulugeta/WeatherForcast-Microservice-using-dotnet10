namespace CloudWeather.Report.Config
{
    public class WeatherDataConfig
    {
        public string PrecipDataProtocol { get; set; }=string.Empty;
        public string PrecipDataHost { get; set; }=string.Empty;
        public string PrecipDataPort { get; set; }=string.Empty;
        public string TempratureDataProtocol { get; set; }=string.Empty;
        public string TempratureDataHost { get; set; }=string.Empty;    
        public string TempratureDataPort { get; set; }=string.Empty;
    }
}