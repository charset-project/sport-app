using sport_app_backend.Dtos;
using sport_app_backend.Models;

namespace sport_app_backend.Interface
{
    public interface ICoachRepository
    {
        Task<ApiResponse> SubmitCoachQuestions(string phoneNumber, CoachQuestionDto coachQuestionDto);
    }
}
