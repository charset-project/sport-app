using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using sport_app_backend.Models.Payments;
using sport_app_backend.Models.Program;


namespace sport_app_backend.Models.Account;

public class Athlete
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public  int Id {get; set;}
    public User User {get; set;}
    public int UserId {get; set;}
    [Required]
    [Range(1, 300)]
    public int Height { get; set; }
    [Required]
    [Range(1, 300)]
    public int CurrentWeight { get; set; }
    [Required]
    [Range(1, 300)]
    public int WeightGoal { get; set; }
    public ICollection<WeightEntry> WeightEntries { get; set; } = [];
    public WaterInTake? WaterInTake { get; set; }
    public ICollection<String> Injury { get; set; } = [];
    public ICollection<WaterInDay> WaterInDays { get; set; } = [];
    //level & Current Body Form can be added  
    public LevelOfAthlete LevelOfAthlete { get; set; }
    public BodyForm CurrentBodyForm { get; set; }
    public BodyForm TargetBodyForm { get; set; }
    public ICollection<Payment> Payments { get; set; } = [];
    public ICollection<WorkoutProgram> WorkoutPrograms { get; set; } = [];
}
