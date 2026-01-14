using CloudWeather.Report.Config;
using CloudWeather.Report.DataAccess;
using Microsoft.Extensions.Options;
using CloudWeather.Report.Models;
using Microsoft.Extensions.Logging; 

namespace CloudWeather.Report.BusinnessLogic
{
    /// <summary>
    /// Aggregates data from multiple external sources to build a weather report.
    /// </summary>
    public interface IWeatherReportAggregator
    {
        /// <summary>
        /// Builds and reurns a Weekly Weather Report.
        /// Persists WeeklyWeatherReport data    
        /// </summary>
        /// <param name="Zip">The zip code for which to build the report.</param>
        /// <param name="Days">The number of days to include in the report.</param>
        /// <returns></returns>
        /// 
        Task<WeatherReport> BuildWeeklyWeatherReportAsync(string Zip, int Days);

    }
    
    public class WeatherReportAggregator: IWeatherReportAggregator
    {
        private readonly IHttpClientFactory _http;
        private readonly ILogger<WeatherReportAggregator> _logger;
        private readonly WeatherDataConfig _weatherDataConfig;
        private readonly WeatherReportDbContext _db;
        public WeatherReportAggregator(IHttpClientFactory http,ILogger<WeatherReportAggregator> logger,
                                      IOptions<WeatherDataConfig> weatherDataConfig,
                                      WeatherReportDbContext db)
        {
            _http=http;
            _logger=logger;
            _weatherDataConfig=weatherDataConfig.Value;
            _db=db;
        }

        public async Task<WeatherReport> BuildWeeklyWeatherReportAsync(string Zip, int Days)
        {
            var httpClient=_http.CreateClient();
            var pricipData=await FetchPrecipitationDataAsync(httpClient,Zip,Days);
            var totalSnow=GetTotalSnow(pricipData);
            var totalRain=GetTotalRain(pricipData);
            _logger.LogInformation(
                $"Total Snowfall for Zip:{Zip} over {Days} days is {totalSnow} inches. " +
                $"Total Rainfall for Zip:{Zip} over {Days} days is {totalRain} inches.");
            var tempData=await FetchTempratureDataAsync(httpClient,Zip,Days);
            var averageHighTemp=tempData.Average(t=>t.TempHighF);
            var averageLowTemp=tempData.Average(t=>t.TempLowF);
            _logger.LogInformation(
                $"Average High Temprature for Zip:{Zip} over {Days} days is {averageHighTemp} F. " +
                $"Average Low Temprature for Zip:{Zip} over {Days} days is {averageLowTemp} F.");   

            var weatherReport=new WeatherReport
            {
                ZipCode=Zip,
                SnowFallTotalInches=totalSnow,
                RainFallTotalInches=totalRain,
                AverageTempHighF= Math.Round(averageHighTemp,1),
                AverageTempLowF=Math.Round(averageLowTemp,1),
                CreatedOn=DateTime.UtcNow
            };
            // TODO: use cache to store and retrieve weather report data instead of the round trip
            _db.Add(weatherReport);
            await _db.SaveChangesAsync();
            return weatherReport;
        }

        public static decimal GetTotalSnow(IEnumerable<PrecipitationModel> pricipData)
        {
            var totalSnow=pricipData.Where(p=>p.WeatherType=="Snow")
                                   .Sum(p=>(decimal)p.AmountInches);
            return Math.Round(totalSnow,1);                  
            
        }
        public static decimal GetTotalRain(IEnumerable<PrecipitationModel> pricipData)
        {
            var totalRain=pricipData.Where(p=>p.WeatherType=="Rain")
                                   .Sum(p=>(decimal)p.AmountInches);
            return Math.Round(totalRain,1);                  
            
        }

        private async Task<List<TempratureModel>> FetchTempratureDataAsync(HttpClient httpClient, string zip, int days)
        {
          var endpoint=BuildTempratureServiceEndPoint(zip,days);
          var tempratureRecords= await httpClient.GetAsync(endpoint);
          var tempratureData= await tempratureRecords.Content.ReadFromJsonAsync<List<TempratureModel>>();
          return tempratureData??new List<TempratureModel>();
        }
        private async Task<List<PrecipitationModel>> FetchPrecipitationDataAsync(HttpClient httpClient, string zip, int days)
        {
          var endpoint=BuildPrecipitationServiceEndPoint(zip,days);
          var precipitationRecords= await httpClient.GetAsync(endpoint);
          var precipitationData= await precipitationRecords.Content.ReadFromJsonAsync<List<PrecipitationModel>>();
          return precipitationData??new List<PrecipitationModel>();
        }

        private string BuildPrecipitationServiceEndPoint(string zip, int days)
        {
              var precipServoceProtocol=_weatherDataConfig.PrecipDataProtocol;
              var precipServoceHost=_weatherDataConfig.PrecipDataHost;
              var precipServocePort=_weatherDataConfig.PrecipDataPort;
              return $"{precipServoceProtocol}://{precipServoceHost}:{precipServocePort}/observation/{zip}?days={days}";
        }
        private string BuildTempratureServiceEndPoint(string zip, int days)
        {
           var tempServoceProtocol=_weatherDataConfig.TempratureDataProtocol;
           var tempServoceHost=_weatherDataConfig.TempratureDataHost;
           var tempServocePort=_weatherDataConfig.TempratureDataPort;
           return $"{tempServoceProtocol}://{tempServoceHost}:{tempServocePort}/observation/{zip}?days={days}";
        }


     

    }
}