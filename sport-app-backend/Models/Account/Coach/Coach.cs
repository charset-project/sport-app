using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using sport_app_backend.Models.Payments;
using sport_app_backend.Models.Program;

namespace sport_app_backend.Models.Account;

public class Coach :IdentityUser<int>
{   [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public override int Id {get; set;}
    
    [Required]
    [StringLength(15)]
    public required string FirstName { get; set; }
    [Required]
    [StringLength(50)]
    public required string LastName { get; set; }

    [Required]
    public DateTime BirthDate { get; set; }
  
    [StringLength(11)]
    [Required]
    public required string PhoneNumber { get; set; }
    [EmailAddress]
    [StringLength(50)]
    public string Email { get; set; } = "";
    public byte[] ImageProfile { get; set; } = Array.Empty<byte>();
    [StringLength(500)]
    public string Bio { get; set; } = "";
    [DataType(DataType.DateTime)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}")]
    public DateTime CreateDate { get; set; }
    [DataType(DataType.DateTime)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}")]
    public DateTime LastLogin { get; set; }
    public Gender Gender { get; set; }
    public ICollection<CoachingDomain>? Domain { get; set; }
    
    public int StartCoachingYear { get; set; }
    
    public ICollection<Coachplan>? Coachplans { get; set; } 
    public ICollection<Payment>? Payments { get; set; }
    public ICollection<WorkoutProgram>? WorkoutPrograms { get; set; }
    
   


    
}
