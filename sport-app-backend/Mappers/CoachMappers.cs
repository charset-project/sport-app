using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sport_app_backend.Dtos;

namespace sport_app_backend.Mappers
{
    public static class CoachMappers
    {
        public static Dtos.CoachProfileResponse ToCoachProfileResponseDto(this Models.Account.User user)
        {
            if (user.Coach == null) return null;
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
                Domain = user.Coach.Domain.Select(x => x.ToString()).ToList() ?? new List<string>(), // Ensure it's not null
                StartCoachingYear = user.Coach.StartCoachingYear
            };



        }

        public static CoachForSearch ToCoachForSearch(this Models.Account.User user)
        {
            if (user.Coach == null) return new CoachForSearch();
            return new CoachForSearch
            {
                FirstName = user.FirstName ?? string.Empty,
                LastName = user.LastName ?? string.Empty,
                ImageProfile = user.ImageProfile ?? Array.Empty<byte>(),
                Bio = user.Bio ?? string.Empty
            };
            
        }


        
    }
}