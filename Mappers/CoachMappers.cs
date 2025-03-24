using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using sport_app_backend.Dtos;
using sport_app_backend.Models.Account;

namespace sport_app_backend.Mappers
{
    public static class CoachMappers
    {
        public static CoachProfileResponse ToCoachProfileResponseDto(this User user)
        {   
            
            return new Dtos.CoachProfileResponse
            {
                FirstName = user.FirstName ?? string.Empty,
                LastName = user.LastName ?? string.Empty,
                BirthDate = user.BirthDate.ToString("yyyy-MM-dd"),
                PhoneNumber = user.PhoneNumber,
                Id = user.Id,
                Gender = user.Gender.ToString(),
                ImageProfile = user.ImageProfile ?? Array.Empty<byte>(),
                Bio = user.Bio ?? string.Empty,
                Domain = user.Coach?.Domain?.Select(x => x.ToString()).ToList() ?? [], // Ensure it's not null
                StartCoachingYear = user.Coach?.StartCoachingYear ?? default
            };



        }

        public static CoachForSearch ToCoachForSearch(this User user)
        {
            if (user.Coach == null) return new CoachForSearch();
            return new CoachForSearch
            {
                Id = user.Coach.Id,
                FirstName = user.FirstName ?? string.Empty,
                LastName = user.LastName ?? string.Empty,
                ImageProfile = user.ImageProfile ?? Array.Empty<byte>(),
                Bio = user.Bio ?? string.Empty
            };

        }

        public static CoachingPlanResponse ToCoachingPlanResponse(this CoachPlan coachPlan)
        {
            return new CoachingPlanResponse
            {
                Id = coachPlan.Id,
                Title = coachPlan.Title,
                Description = coachPlan.Description,
                Price = coachPlan.Price,
                DurationByDay = coachPlan.DurationByDay,
                IsActive = coachPlan.IsActive,
                TypeOfCoachingPlan = coachPlan.TypeOfCoachingPlan.ToString()
            };
        }
        public static CoachProfileForAthleteDto ToCoachProfileForAthleteDto(this Coach coach)
        {
            return new CoachProfileForAthleteDto
            {
                FirstName = coach.User?.FirstName ?? string.Empty,
                LastName = coach.User?.LastName ?? string.Empty,
                Id = coach.Id,
                ImageProfile = coach.User?.ImageProfile ?? Array.Empty<byte>(),
                Bio = coach.User?.Bio ?? string.Empty,
                Domain = coach.Domain?.Select(x => x.ToString()).ToList() ?? new List<string>(), // Ensure it's not null
                StartCoachingYear = coach.StartCoachingYear,
                Coachplans = coach.Coachplans?.Select(x => x._toCoachingPlanResponse()).ToList() ?? new List<CoachingPlanResponse>()
            };
        }




        private static CoachingPlanResponse _toCoachingPlanResponse(this CoachPlan coachPlan)
        {
            return new CoachingPlanResponse
            {
                Id = coachPlan.Id,
                Title = coachPlan.Title,
                Description = coachPlan.Description,
                Price = coachPlan.Price,
                DurationByDay = coachPlan.DurationByDay,
                IsActive = coachPlan.IsActive,
                TypeOfCoachingPlan = coachPlan.TypeOfCoachingPlan.ToString()
            };
        }


    }

}