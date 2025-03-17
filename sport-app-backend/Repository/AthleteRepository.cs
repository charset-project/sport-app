using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using sport_app_backend.Data;
using sport_app_backend.Dtos;
using sport_app_backend.Interface;
using sport_app_backend.Models.Account;
using sport_app_backend.Models.Question.A_Question;

namespace sport_app_backend.Repository
{
    public class AthleteRepository : IAthleteRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<User> _userManager;
        public AthleteRepository(ApplicationDbContext context)
        {
            _context = context;
            _userManager = context.Users;
        }

        public async Task<bool> SubmitAthleteQuestions(string phoneNumber, AthleteQuestionDto AthleteQuestionDto)
        {
            var user = await _userManager.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
            if (user is null) return false;
            var athlete = user.Athlete;
            if (athlete is null) return false;
            user.Gender = AthleteQuestionDto.Gender;
            athlete.Height = AthleteQuestionDto.Height;
            athlete.CurrentWeight = AthleteQuestionDto.CurrentWeight;
            athlete.WeightGoal = AthleteQuestionDto.TargetWeight;
            athlete.CurrentBodyForm = AthleteQuestionDto.CurrentBodyForm;
            athlete.TargetBodyForm = AthleteQuestionDto.TargetBodyForm;
            athlete.InjuryArea = AthleteQuestionDto.InjuryArea ?? [];
            athlete.FitnessLevel = AthleteQuestionDto.FitnessLevel;

            var athleteQuestion = new AthleteQuestion
            {
                AthleteId = athlete.Id,
                Athlete = athlete,
                ExerciseGoal = AthleteQuestionDto.ExerciseGoal,
                ExerciseMotivation = AthleteQuestionDto.ExerciseMotivation,
                CommonIssues = AthleteQuestionDto.CommonIssues,

            };
            athlete.AthleteQuestion = athleteQuestion;
            _context.AthleteQuestions.Add(athleteQuestion);

            await _context.SaveChangesAsync();
            return true;

        }

    }
}