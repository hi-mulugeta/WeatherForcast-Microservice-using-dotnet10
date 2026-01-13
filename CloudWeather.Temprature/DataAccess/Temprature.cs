namespace CloudWeather.Temprature.DataAccess
{
    public class Temprature
    {
       public Guid Id { get; set; }
       public DateTime CreatedAt { get; set; }
       public decimal TempHighF { get; set; }
       public decimal TempLowF { get; set; }
       public string ZipCode { get; set; }  
    }
}