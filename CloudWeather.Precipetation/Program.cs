

using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);


var app = builder.Build();
app.MapGet("/", () => "Hello World!");
app.MapGet("/observation/{zipCode}", (string zipCode,[FromQuery] int? days) =>
{
    // Placeholder for precipitation observation logic
    return $"Let there be an eternal light {zipCode}";
}); 
app.Run();
