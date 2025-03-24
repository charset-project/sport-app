using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using sport_app_backend.Models.Account;
using sport_app_backend.Models.Program;
using sport_app_backend.Models.TrainingPlan;

namespace sport_app_backend.Models.Payments;

public class Payment
{   [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }  
    public int AthleteId { get; set; }
    public Athlete? Athlete { get; set; }
    public int CoachId { get; set; }
    public Coach? Coach { get; set; }
    public double Amount {get; set;}
    public required string TitleOfPlan { get; set; }
    public required string DescriptionOfPlan { get; set; }
    public required int DurationByDay { get; set; }
    public TypeOfCoachingPlan TypeOfCoachingPlan { get; set; } 

    public string TransitionId { get; set; }="";

    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.inprogress;
    public DateTime PaymentDate { get; set; }=DateTime.Now;
    public WorkoutProgram? WorkoutProgram { get; set; }



    

}
