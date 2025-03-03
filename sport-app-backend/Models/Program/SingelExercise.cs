using sport_app_backend.Models.Actions;

namespace sport_app_backend.Models.Program;

public class SingelExercise
{
    public int Id { get; set; }
    public int ExerciseInDayId { get; set; }
    public ExerciseInDay? ExerciseInDay { get; set; }
    
    public int Set { get; set; }
    public int Rep { get; set; }
    public Exercise? Exercise { get; set; }
    public int ExerciseId { get; set; }

}
