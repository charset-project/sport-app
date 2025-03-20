using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using sport_app_backend.Models.TrainingPlan;

namespace sport_app_backend.Models.Account;

public class CoachPlan
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int CoachId { get; set; }
    public Coach? Coach {get; set;} 
    
    [MaxLength(50)]
    public required string Title { get; set; }
  
    [MaxLength(500)]
    public required string Description { get; set; }
    public required double Price { get; set; }
    public required int DurationByDay { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
    public TypeOfCoachingPlan TypeOfCoachingPlan { get; set; }

}
