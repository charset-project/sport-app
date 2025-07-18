using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using sport_app_backend.Models.Account;
using sport_app_backend.Models.Payments;

namespace sport_app_backend.Models.Program;

public class WorkoutProgram
{   [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public required int CoachId { get; set; }
    public required Coach Coach {get; set;}
    public required int AthleteId { get; set; }
    public required Athlete Athlete {get; set;}
    [MaxLength(50)]
    public string Title { get; set; }="";
    public required int PaymentId { get; set; }
    public required Payment Payment { get; set; }
    public DateTime StartDate { get; set; }
    public int ProgramDuration { get; set; } = 3;
    [MaxLength(20)] 
    public string ProgramLevel { get; set; } = "Beginner";
    public List<ProgramPriority> ProgramPriorities { get; set; } = [];
    public List<GeneralWarmUp>? GeneralWarmUp { get; set; } = [];
    public DedicatedWarmUp? DedicatedWarmUp { get; set; } 
    public DateTime EndDate { get; set; }
    public WorkoutProgramStatus Status { get; set; } = WorkoutProgramStatus.NOTSTARTED;
    public int Duration { get; set; }
    [MaxLength(120)]
    public string Description { get; set; } = "";
    public List<ProgramInDay> ProgramInDays { get; set; } = [];
    public List<TrainingSession> TrainingSessions { get; set; } = [];
    [DataType(DataType.Date)]
    [Column(TypeName = "date")]
    public DateTime? LastExerciseDate { get; set; }     
    public int TotalSessionCount { get; set; } = 0;
    public int CompletedSessionCount { get; set; } = 0;
}