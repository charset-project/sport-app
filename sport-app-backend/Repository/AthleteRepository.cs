using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using sport_app_backend.Data;
using sport_app_backend.Dtos;
using sport_app_backend.Interface;
using sport_app_backend.Models;
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

        public async Task<ApiResponse> AddWaterIntake(string phoneNumber, WaterInTakeDto waterInTakeDto)
        {
            var user = await _userManager.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
            if (user is null) return new ApiResponse() { Message = "User not found", Action = false };
            var athlete = user.Athlete;
            if (athlete is null) return new ApiResponse() { Message = "User is not an athlete", Action = false };// Ensure the user is an athlete
            var waterIntake = new WaterInTake
            {
                AthleteId = athlete.Id,
                Athlete = athlete,
                DailyCupOfWater = waterInTakeDto.DailyCupOfWater,
                Reminder = waterInTakeDto.Reminder
            };
            athlete.WaterInTake = waterIntake;
            //edit last water intake if it exists
            var lastWaterIntake = await _context.WaterInTakes
                .Where(w => w.AthleteId == athlete.Id)
                .FirstOrDefaultAsync();
            if (lastWaterIntake != null)
            {
                lastWaterIntake.DailyCupOfWater = waterInTakeDto.DailyCupOfWater;
                lastWaterIntake.Reminder = waterInTakeDto.Reminder;
                _context.WaterInTakes.Update(lastWaterIntake);
                athlete.WaterInTake = lastWaterIntake; // Update the athlete's WaterInTake reference
            }
            else
            {
                athlete.WaterInTake = waterIntake;
                _context.WaterInTakes.Add(waterIntake);
            }
            await _context.SaveChangesAsync();
            return new ApiResponse(){
                Message = "WaterIntake added successfully",
                Action = true};
        }

        public async Task<ApiResponse> SubmitAthleteQuestions(string phoneNumber, AthleteQuestionDto AthleteQuestionDto)
        {
            var user = await _userManager.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
            if (user is null) return new ApiResponse() { Message = "User not found", Action = false };
            var athlete = user.Athlete;
            if (athlete is null) return new ApiResponse() { Message = "User is not an athlete", Action = false };// Ensure the user is an athlete
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
            return new ApiResponse()
            {
                Message = "Athlete questions submitted successfully",
                Action = true
            };

        }

        public async Task<ApiResponse> UpdateWaterInDay(string phoneNumber)
        {
            var user = await _userManager.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
            if (user is null) return new ApiResponse() { Message = "User not found", Action = false };
            var athlete = user.Athlete;
            if (athlete is null) return new ApiResponse(){Message="User is not athlete",Action = false};
            var WaterInDay = await _context.WaterInDays
                .Where(w => w.AthleteId == athlete.Id && w.Date.Date == DateTime.Now.Date)
                .FirstOrDefaultAsync();
            if (WaterInDay is null)
            {
                var waterInDay = new WaterInDay
                {
                    AthleteId = athlete.Id,
                    Date = DateTime.Now.Date,
                    Athlete = athlete,
                    NumberOfCupsDrinked = 1 // Initialize with 1 cup since it's the first entry for today
                };
                athlete.WaterInDays.Add(waterInDay); // Add the new WaterInDay to the athlete's collection
                await _context.WaterInDays.AddAsync(waterInDay);
                await _context.SaveChangesAsync();
                return new ApiResponse(){Message="WaterInDay added successfully",Action = true};
            }
            else
            {
                WaterInDay.NumberOfCupsDrinked += 1;
                _context.WaterInDays.Update(WaterInDay);
                await _context.SaveChangesAsync();
                return new ApiResponse(){Message="WaterInDay updated successfully",Action = true};
            }
        }
    }
}