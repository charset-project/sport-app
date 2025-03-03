namespace sport_app_backend.Models.Program;

public class ExerciseInDay
{   public int Id { get; set; }
    public int ProgramInDayId { get; set; }
    public ProgramInDay? ProgramInDay { get; set; }
    
    public int ExerciseNumber { get; set; }
    public bool IsSuperSet { get; set; }

    public SuperSetExercise? SuperSetExercise { get; set; }

    public SingelExercise? SingelExercise { get; set; }


    


    
}
