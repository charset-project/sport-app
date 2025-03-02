using System.ComponentModel.DataAnnotations;

namespace sport_app_backend.Models.Account;

public class WeightEntry
{
    [Key]
    public int Id { get; set; }
    public int AthleteId { get; set; }
    [Required]
    [Range(1, 300)]
    public int Weight { get; set; }
    [Required]
    public DateTime EntryDate { get; set; }

}
