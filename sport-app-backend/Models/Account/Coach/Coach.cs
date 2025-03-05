using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sport_app_backend.Models.Account;

public class Coach 
{   [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id {get; set;}
    
    [Required]
    [StringLength(15)]
    public required string FirstName { get; set; }
    [Required]
    [StringLength(50)]
    public required string LastName { get; set; }

    [Range(1300, 1402)]
    public int BirthDate { get; set; }
  
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
    public List<CoachingDomain>? Domain { get; set; }
    
    public int StartCoachingYear { get; set; }
    
    public List<Coachplan>? Coachplans { get; set; }       
   


    
}
