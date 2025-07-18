
using sport_app_backend.Dtos;
using sport_app_backend.Models;
using sport_app_backend.Models.Account;

namespace sport_app_backend.Mappers
{
    public static class AthleteMappers
    {
        public static AthleteProfileResponse? ToAthleteProfileResponseDto(this User user)
        {
            if (user.Athlete == null) return null;
            var waterInTake = user.Athlete.WaterInTake ?? new WaterInTake { DailyCupOfWater = 0, Reminder = 0 };

                return new AthleteProfileResponse
                {
                    
                    FirstName = user.FirstName ?? string.Empty,
                    LastName = user.LastName ?? string.Empty,
                    UserName = user.UserName,
                    BirthDate = user.BirthDate.ToString("yyyy-MM-dd"),
                    PhoneNumber = user.PhoneNumber,
                    Id = user.Id,
                    Height = user.Athlete.Height,
                    CurrentWeight = user.Athlete.CurrentWeight,
                    WeightGoal = user.Athlete.WeightGoal,
                    Gender = user.Gender.ToString(),
                    ImageProfile = user.ImageProfile ?? "",
                    Bio = user.Bio ?? "",
                    TimeBeforeWorkout = user.Athlete.TimeBeforeWorkout,
                    RestTime = user.Athlete.RestTime,
                    DailyCupOfWater =waterInTake.DailyCupOfWater,
                    Reminder = waterInTake.Reminder
                };
        }
        
       
    }
}