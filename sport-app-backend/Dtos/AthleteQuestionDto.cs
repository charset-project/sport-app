using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sport_app_backend.Models.Account;
using sport_app_backend.Models.Question.A_Question;

namespace sport_app_backend.Dtos
{
    public class AthleteQuestionDto
    {
        public Gender Gender { get; set; }
        public int Height { get; set; }
        public int CurrentWeight { get; set; }
        public int TargetWeight { get; set; }
        public int CurrentBodyForm { get; set; }
        public int TargetBodyForm { get; set; }
        public ExerciseGoal? ExerciseGoal { get; set; }
        public ICollection<ExerciseMotivation>? ExerciseMotivation { get; set; }
        public ICollection<InjuryArea>? InjuryArea { get; set; }
        public ICollection<CommonIssues>? CommonIssues { get; set; }
        public FitnessLevel? FitnessLevel { get; set; }


        
    }
}