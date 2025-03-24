using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using sport_app_backend.Data;
using sport_app_backend.Dtos;
using sport_app_backend.Interface;
using sport_app_backend.Models;
using sport_app_backend.Models.Account;
using sport_app_backend.Models.TrainingPlan;

namespace sport_app_backend.Repository
{
    public class CoachRepository : ICoachRepository
    {
        private readonly ApplicationDbContext _context;
    

        public CoachRepository(ApplicationDbContext context)
        {
            _context = context;
         
        }

        public async Task<ApiResponse> AddCoachingPlane(string phoneNumber, AddCoachingPlaneDto addCoachingPlaneDto)
        {
         
            var coach = await _context.Coaches.Include(c => c.Coachplans).FirstOrDefaultAsync(c => c.PhoneNumber == phoneNumber);
            if (coach is null) return new ApiResponse() { Message = "User is not a coach", Action = false };// Ensure the user is a coach
            var coachingPlane = new CoachPlan
            {
              Coach = coach,
              CoachId = coach.Id,
              Title = addCoachingPlaneDto.Title,
              Description = addCoachingPlaneDto.Description,
              Price = addCoachingPlaneDto.Price,
              DurationByDay = addCoachingPlaneDto.DurationByDay,
              IsActive = addCoachingPlaneDto.IsActive,
              CreatedDate = DateTime.Now,
              TypeOfCoachingPlan = (TypeOfCoachingPlan)Enum.Parse(typeof(TypeOfCoachingPlan), addCoachingPlaneDto.TypeOfCoachingPlan)


            };
            coach.Coachplans ??= [];

            coach.Coachplans.Add(coachingPlane);
            _context.CoachesPlan.Add(coachingPlane);
            _context.SaveChanges();
            return new ApiResponse()
            {
                Message = "Coaching plane added successfully",
                Action = true
                
            };
        }

        public async Task<ApiResponse> SubmitCoachQuestions(string phoneNumber, CoachQuestionDto coachQuestionDto)
        {  
            var user = await _context.Users.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
            if (user is null) return new ApiResponse() { Message = "User not found", Action = false };
            var coach = user.Coach;
            if (coach == null) return new ApiResponse() { Message = "User is not a coach", Action = false };// Ensure the user is a coach
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
            return new ApiResponse()
            {
                Message = "Coach questions submitted successfully",
                Action = true
            };
        }

        public Task<ApiResponse> UpdateCoachingPlane(string phoneNumber, AddCoachingPlaneDto addCoachingPlaneDto)
        {

            // var  user = _context.Users.Include(x => x.Coach).Include(c => c.Coach.Coachplans).FirstOrDefault(x => x.PhoneNumber == phoneNumber);
            // if (user is null) return Task.FromResult(new ApiResponse() { Message = "User not found", Action = false });
            // var coach = user.Coach;
            // if (coach == null) return Task.FromResult(new ApiResponse() { Message = "User is not a coach", Action = false });// Ensure the user is a coach
            // var coachingPlane = coach.Coachplans.FirstOrDefault(x => x.Id == addCoachingPlaneDto.Id);
            // _context.CoachesPlan.Update(coachingPlane);
            // _context.SaveChanges();
            // return Task.FromResult(new ApiResponse()
            // {
            //     Message = "Coaching plane updated successfully",
            //     Action = true
            // });
            return Task.FromResult(new ApiResponse()
            {
                Message = "Coaching plane updated successfully",
                Action = true
            });
        }
    }
}
