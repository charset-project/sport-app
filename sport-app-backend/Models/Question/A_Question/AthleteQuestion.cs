using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using sport_app_backend.Models.Account;

namespace sport_app_backend.Models.Question.A_Question
{
    public class AthleteQuestion
    {
        [Key]
        public int Id { get; set; }
        public int AthleteId { get; set; }
        public Athlete? Athlete { get; set; }
        public ExerciseGoal? ExerciseGoal { get; set; }
        public ICollection<ExerciseMotivation>? ExerciseMotivation { get; set; }
        public ICollection<InjuryArea>? InjuryArea { get; set; }
        public ICollection<CommonIssues>? CommonIssues { get; set; }
        public FitnessLevel? FitnessLevel { get; set; }
        
    }
}