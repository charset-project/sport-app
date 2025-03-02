using System.ComponentModel.DataAnnotations;
using sport_app_backend.Models.TrainingPlan;

namespace sport_app_backend.Models.Account;

public class Coachplans
{

    public int TariffId { get; set; }
    public int CoachId { get; set; } 
    
    [MaxLength(50)]
    public required string Title { get; set; }
  
    [MaxLength(500)]
    public required string Description { get; set; }
    public required int Price { get; set; }
    public required int DurationByDay { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
    public TypeOfCoachingPlan TypeOfCoachingPlan { get; set; } 
    
}
