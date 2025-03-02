namespace sport_app_backend.Models;

public class WaterInTake
{
    public int Id { get; set; }
    public int AthleteId { get; set; }
    public DateTime Date { get; set; }
    public int DailyCupOfWater { get; set; } 
    public int NumberOfReminderInDay { get; set; }
}
