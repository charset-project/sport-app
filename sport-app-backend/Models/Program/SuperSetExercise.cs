using sport_app_backend.Models.Actions;

namespace sport_app_backend.Models.Program;

public class SuperSetExercise
{

    public int Id { get; set; }
    public int ExerciseInDayId { get; set; }
    public ExerciseInDay? ExerciseInDay { get; set; }

    /// exercisenumber1
    public int Set1 { get; set; }
    public int Rep1 { get; set; }
    public Exercise? Exercise1 { get; set; }
    public int ExerciseId1 { get; set; }

    /// exercisenumber2
    public int Set2 { get; set; }
    public int Rep2 { get; set; }
    public Exercise? Exercise2 { get; set; }
    public int ExerciseId2 { get; set; }
    
    
    
}
