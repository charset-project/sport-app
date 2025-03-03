using sport_app_backend.Models.Account;
using sport_app_backend.Models.Payments;

namespace sport_app_backend.Models.Program;

public class WorkoutProgram
{
    public int Id { get; set; }
    public int CoachId { get; set; }
    public Coach? Coach {get; set;}
    public int AthleteId { get; set; }
    public Athlete? Athlete {get; set;}
    public string Title { get; set; }="";
    public int PaymentId { get; set; }
    public Payment? Payment { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public WorkoutProgramStatus Status { get; set; }
    public int Duration { get; set; }
    public string Description { get; set; } = "";
    public int NumberOfRepeats { get; set; }    
    public ICollection<ProgramInDay>? ProgramInDays { get; set; }    
}
