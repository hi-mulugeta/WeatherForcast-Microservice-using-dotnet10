using CloudWeather.Temprature.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TempratureDbContext>(options =>
{
    options.EnableSensitiveDataLogging();
    options.EnableDetailedErrors();
    options.UseNpgsql(builder.Configuration.GetConnectionString("AppDb"));
}, ServiceLifetime.Transient);

var app = builder.Build();

app.MapGet("/observation/{zipCode}", async(string zipCode,[FromQuery] int? days,TempratureDbContext db) =>
{
    if(days==null || days<1 || days>30) {
        return Results.BadRequest("days query parameter is required between 1 and 30");  
    }
   var startDate=DateTime.UtcNow - TimeSpan.FromDays(days.Value); 
   var result=await db.Tempratures.Where(p=>p.ZipCode==zipCode && p.CreatedAt>startDate)
                              .OrderByDescending(p=>p.CreatedAt)
                              .ToListAsync();
    return Results.Ok(result);
}); 

app.MapPost("/observation", async(Temprature temp,TempratureDbContext db) =>
{
    temp.CreatedAt=temp.CreatedAt.ToUniversalTime();
    await db.Tempratures.AddAsync(temp);
    await db.SaveChangesAsync();
});
app.Run();
