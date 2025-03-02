using System.ComponentModel.DataAnnotations;

namespace sport_app_backend.Models.Account;

public class Athlete : User
{
    public int Id { get; set; }
    [Required]
    [Range(1, 300)]
    public int Height { get; set; }
    [Required]
    [Range(1, 300)]
    public int CurrentWeight { get; set; }
    [Required]
    [Range(1, 300)]
    public int WeightGoal { get; set; }

    public List<String> Injury { get; set; } = [];
    //level & Current Body Form can be added

}
