using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<User> _userManager;

        public CoachRepository(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<bool> SubmitCoachQuestions(string phoneNumber, CoachQuestionDto coachQuestionDto)
        {
            var user = await _userManager.FindByNameAsync(phoneNumber);
            if (user is null) return false;
            var isCoach = await _userManager.IsInRoleAsync(user, "Coach");
            if (!isCoach) return false;
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
            _context.CoachQuestions.Add(coachQuestion);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
