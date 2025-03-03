using sport_app_backend.Models.Account;

namespace sport_app_backend.Models;

public class WaterInDay
{   public int Id {get; set;}
    public int UserId {get; set;}
    public Athlete? Athlete {get; set;}      
    public int NumberOfCupsDrinked {get; set;}
    public DateTime Date {get; set;}
    
}
