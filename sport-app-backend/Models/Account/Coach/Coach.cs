namespace sport_app_backend.Models.Account;

public class Coach : User
{
    public int Id {get; set;}
    public List<CoachingDomain>? Domain { get; set; }
    
    public int StartCoachingYear { get; set; }
    
    public List<Coachplan>? Coachplans { get; set; }       
   


    
}
