namespace sport_app_backend.Models.Program;

public class ProgramInDay
{
    public int Id { get; set; }
    public int WorkoutProgramId { get; set; }
    public WorkoutProgram? WorkoutProgram { get; set; }

    public int ForWhichDay { get; set; }

    public ICollection<ExerciseInDay> AllExerciseInDays { get; set; }= new List<ExerciseInDay>();


    
}
