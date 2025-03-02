using System.ComponentModel.DataAnnotations;

namespace sport_app_backend.Models.Actions;

public class Action
{   
    public int Id { get; set; }
    [Required]
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required List<string> Image { get; set; }
    public required List<string> Video { get; set; }
    public required List<ActionsEnum> ActionsTage { get; set; }
   
}
