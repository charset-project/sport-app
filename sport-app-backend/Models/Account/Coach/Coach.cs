using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using sport_app_backend.Models.Payments;
using sport_app_backend.Models.Program;

namespace sport_app_backend.Models.Account;

public class Coach 
{   
    [Key]
    public  int Id {get; set;}
    public User User {get; set;}
    public int UserId {get; set;}
    [EmailAddress]
    [StringLength(50)]
    public string Email { get; set; } = "";
    public ICollection<CoachingDomain>? Domain { get; set; }
    
    public int StartCoachingYear { get; set; }
    
    public ICollection<Coachplan>? Coachplans { get; set; } 
    public ICollection<Payment>? Payments { get; set; }
    public ICollection<WorkoutProgram>? WorkoutPrograms { get; set; }
    
   


    
}
