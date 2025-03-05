using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sport_app_backend.Models.Program;

public class WorkoutProgramInDay
{   [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int WorkoutProgramId { get; set; }
    public WorkoutProgram? WorkoutProgram { get; set; }

    public int ForWhichDay { get; set; }

    public ICollection<ExerciseInDay> AllExerciseInDays { get; set; }= new List<ExerciseInDay>();


    
}
