using sport_app_backend.Dtos;

namespace sport_app_backend.Interface
{
    public interface ICoachRepository
    {
        Task<bool> SubmitCoachQuestions(string phoneNumber, CoachQuestionDto coachQuestionDto);
    }
}
