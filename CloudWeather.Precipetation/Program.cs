

using CloudWeather.Precipetation.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<PricipDbContext>(options =>
{
    options.EnableSensitiveDataLogging();
    options.EnableDetailedErrors();
    options.UseNpgsql(builder.Configuration.GetConnectionString("AppDb"));
}, ServiceLifetime.Transient);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapGet("/observation/{zipCode}", async(string zipCode,[FromQuery] int? days,PricipDbContext db) =>
{
    if(days==null || days<1 || days>30) {
        return Results.BadRequest("days query parameter is required between 1 and 30");  
    }
   var startDate=DateTime.UtcNow - TimeSpan.FromDays(days.Value); 
   var result=await db.Precipitations.Where(p=>p.ZipCode==zipCode && p.CreatedAt>startDate)
                              .OrderByDescending(p=>p.CreatedAt)
                              .ToListAsync();
    return Results.Ok(result);
}); 
app.Run();

