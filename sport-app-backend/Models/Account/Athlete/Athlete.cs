using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;


namespace sport_app_backend.Models.Account;

public class Athlete 
{   [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Required]
    [StringLength(15)]
    public required string FirstName { get; set; }
    [Required]
    [StringLength(50)]
    public required string LastName { get; set; }

    
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
    

   
    

}
