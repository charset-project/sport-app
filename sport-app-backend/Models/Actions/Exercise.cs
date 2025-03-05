using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sport_app_backend.Models.Actions;

public class Exercise
{   [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    public required string Name { get; set; }
    [MaxLength(500)]
    public required string Description { get; set; }
    public int Calories { get; set; }
    public required List<string> Image { get; set; }
    public required List<string> Video { get; set; }
    public required List<ExerciseEnum> ActionsTage { get; set; }
   
}
