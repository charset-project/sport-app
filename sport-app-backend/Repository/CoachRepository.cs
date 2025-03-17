using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using sport_app_backend.Data;
using sport_app_backend.Dtos;
using sport_app_backend.Interface;
using sport_app_backend.Models;
using sport_app_backend.Models.Account;

namespace sport_app_backend.Repository
{
    public class CoachRepository : ICoachRepository
    {
        private readonly ApplicationDbContext _context;
    

        public CoachRepository(ApplicationDbContext context)
        {
            _context = context;
         
        }

        public async Task<bool> SubmitCoachQuestions(string phoneNumber, CoachQuestionDto coachQuestionDto)
        {  
            var user = await _context.Users.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
            if (user is null) return false;
            var coach = user.Coach;
            if (coach == null) return false; // Ensure the user is a coach
            var coachQuestion = new CoachQuestion
            {
                UserId = user.Id,
                User = user,
                Disciplines = coachQuestionDto.Disciplines,
                Motivations = coachQuestionDto.Motivations,
                WorkOnlineWithAthletes = coachQuestionDto.WorkOnlineWithAthletes,
                PresentsPracticeProgram = coachQuestionDto.PresentsPracticeProgram,
                TrackAthlete = coachQuestionDto.TrackAthlete,
                ManagingRevenue = coachQuestionDto.ManagingRevenue,
                DifficultTrackAthletes = coachQuestionDto.DifficultTrackAthletes,
                HardCommunicationWithAthletes = coachQuestionDto.HardCommunicationWithAthletes
            };
            coach.CoachQuestion = coachQuestion;
            _context.CoachQuestions.Add(coachQuestion);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
