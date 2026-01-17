using CloudWeather.DataLoader.Models;
using Microsoft.Extensions.Configuration;

using System;
using System.Formats.Tar;
using System.Net.Http.Json;
IConfiguration config= new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

var ServiceConfig=config.GetSection("Services");

var tempServiceConfig=ServiceConfig.GetSection("Temperature");
var tempServiceHost=tempServiceConfig["Host"];
var tempServicePort=tempServiceConfig["Port"];

var precipServiceConfig=ServiceConfig.GetSection("Precipitation");
var precipServiceHost=precipServiceConfig["Host"];
var precipServicePort=precipServiceConfig["Port"];

var zipCodes=new List<string> { "10001", "90210", "33101", "60601", "73301" };
var httpTempratureClient=new HttpClient
{
    BaseAddress=new Uri($"http://{tempServiceHost}:{tempServicePort}/")
};
var httpPrecipitationClient=new HttpClient
{
    BaseAddress=new Uri($"http://{precipServiceHost}:{precipServicePort}/")
};


foreach(var zip in zipCodes)
{
    Console.WriteLine($"Processing zip code: {zip}");
    var from=DateTime.UtcNow.AddDays(-2);
    var thru=DateTime.Now;

    for(var day=from.Date;day.Date<=thru.Date;day=day.AddDays(1))
    {
      var temps=PostTemp(zip,day,httpTempratureClient);
      PostPrecip(temps[0],zip,day,httpPrecipitationClient);
    }
}

List<int> PostTemp(string zip,DateTime day,HttpClient client)
{
    var rand=new Random();
    var t1=rand.Next(0,100);
    var t2=rand.Next(0,100);
    var hiloTemps=new List<int>{t1,t2};
    hiloTemps.Sort();
    var temperatureObservation=new TemperatureModel
    {
        TempHighF=hiloTemps[1],
        TempLowF=hiloTemps[0],
        ZipCode=zip,
        CreatedAt=day
    };
    var response=client.PostAsJsonAsync("observation",temperatureObservation).Result;
    if(response.IsSuccessStatusCode)
    {
        Console.WriteLine($"Posted temperature for {zip} on {day.ToShortDateString()}: High {temperatureObservation.TempHighF}, Low {temperatureObservation.TempLowF}");
       // return hiloTemps;
    }
    else
    {
        Console.WriteLine($"Failed to post temperature for {zip} on {day.ToShortDateString()}: {response.StatusCode}");
        //return new List<int>{0,0};  
    }
    return hiloTemps;

}

void PostPrecip(int lowTemp, string zip, DateTime day, HttpClient client)
{
    var rand=new Random();
   var isPrecip=rand.Next(2)<1;
   PrecipitationModel precipitation;
    if (isPrecip)
    {
        var precipInch=rand.Next(1,16);
        if(lowTemp<=32)
        {
            precipitation=new PrecipitationModel
            {
                AmountInches=precipInch,
                WeatherType="Snow",
                ZipCode=zip,
                CreatedOn=day
            };
        }
        else
        {
            precipitation=new PrecipitationModel
            {
                AmountInches=precipInch,
                WeatherType="Rain",
                ZipCode=zip,
                CreatedOn=day   
            };
        }

    }
    else
    {
        precipitation=new PrecipitationModel
        {
            AmountInches=0,
            WeatherType="None",
            ZipCode=zip,
            CreatedOn=day   
        };
    }
    var PrecipResponse=client.PostAsJsonAsync("observation",precipitation).Result;
    if(PrecipResponse.IsSuccessStatusCode)
    {
        Console.WriteLine($"Posted precipitation for {zip} on {day.ToShortDateString()}: {precipitation.WeatherType} {precipitation.AmountInches} inches");
    }
    else
    {
        Console.WriteLine($"Failed to post precipitation for {zip} on {day.ToShortDateString()}: {PrecipResponse.StatusCode}");
    }
}



