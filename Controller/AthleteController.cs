
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sport_app_backend.Data;
using sport_app_backend.Dtos;
using sport_app_backend.Interface;
using sport_app_backend.Mappers;
using sport_app_backend.Models;
using sport_app_backend.Models.Account;
using sport_app_backend.Models.Program;

namespace sport_app_backend.Controller
{

    [Route("api/[controller]")]
    [ApiController]
    public class AthleteController(IAthleteRepository athleteRepository, ApplicationDbContext context)
        : ControllerBase
    {
        [HttpPost("add_athlete_question")]
        [Authorize(Roles = "Athlete")]

        public async Task<IActionResult> AddAthleteQuestion([FromBody] AthleteQuestionDto athleteQuestionDto)
        {
            var phoneNumber = User.FindFirst(ClaimTypes.Name)?.Value;
            if (phoneNumber is null) return BadRequest("PhoneNumber is null");

            var result = await athleteRepository.SubmitAthleteQuestions(phoneNumber, athleteQuestionDto);
            if (!result.Action) return BadRequest(result.Message);
            return Ok(result);
        }

        [HttpPost("Add_FirstQuestions")]
        [Authorize(Roles = "Athlete")]
        public async Task<IActionResult> AddFirstQuestions([FromBody] AthleteFirstQuestionsDto athleteFirstQuestions)
        {
            var phoneNumber = User.FindFirst(ClaimTypes.Name)?.Value;
            if (phoneNumber is null) return BadRequest("PhoneNumber is null");
            var result = await athleteRepository.AthleteFirstQuestions(phoneNumber, athleteFirstQuestions);
            if (!result.Action) return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("add_water_intake")]
        [Authorize(Roles = "Athlete")]
        public async Task<IActionResult> AddWaterIntake([FromBody] WaterInTakeDto waterInTakeDto)
        {
            var phoneNumber = User.FindFirst(ClaimTypes.Name)?.Value;
            if (phoneNumber is null) return BadRequest("PhoneNumber is null");

            var result = await athleteRepository.AddWaterIntake(phoneNumber, waterInTakeDto);
            if (!result.Action) return BadRequest(result);
            return Ok(result);
        }


        [HttpGet("get_water_intake")]
        [Authorize(Roles = "Athlete")]
        public async Task<IActionResult> GetWaterIntake()
        {
            var phoneNumber = User.FindFirst(ClaimTypes.Name)?.Value;
            if (phoneNumber is null) return BadRequest("PhoneNumber is null");
            var athlete = await context.Athletes.Include(a => a.WaterInTake)
                .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
            if (athlete == null || athlete.WaterInTake == null)
            {
                return NotFound("Water intake not found");
            }
            else
            {
                return Ok(new ApiResponse()
                {
                    Action = true,
                    Message = "Water intake found",
                    Result = new WaterInTakeDto()
                    {
                        DailyCupOfWater = athlete.WaterInTake.DailyCupOfWater,
                        Reminder = athlete.WaterInTake.Reminder
                    }
                });
            }
        }



        [HttpPut("update_water_inDay")]
        [Authorize(Roles = "Athlete")]
        public async Task<IActionResult> UpdateWaterInDay()
        {
            var phoneNumber = User.FindFirst(ClaimTypes.Name)?.Value;
            if (phoneNumber is null) return BadRequest("PhoneNumber is null");
            var result = await athleteRepository.UpdateWaterInDay(phoneNumber);
            if (!result.Action) return BadRequest(result);
            return Ok(result);
        }


        [HttpGet("get_water_in_day")]
        [Authorize(Roles = "Athlete")]

        public async Task<IActionResult> GetWaterInDayForLast7Days()
        {
            var phoneNumber = User.FindFirst(ClaimTypes.Name)?.Value;
            if (phoneNumber is null) return BadRequest("PhoneNumber is null");
            var athlete = await context.Users
                .Include(u => u.Athlete)
                .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
            var today = DateTime.Now.Date;
            var daysSinceSaturday = (int)today.DayOfWeek == 0 ? 6 : (int)today.DayOfWeek - 6;
            var lastSaturday = today.AddDays(daysSinceSaturday);
            if (athlete == null || athlete.Athlete == null) return NotFound("Athlete not found");
            var listOfWaterInDay = await context.WaterInDays.Where(w => w.AthleteId == athlete.Athlete.Id && w.Date.Date >= lastSaturday.Date)
                .ToListAsync();

            return Ok(new ApiResponse()
            {
                Action = true,
                Message = "Water inday found",
                Result = listOfWaterInDay.Select(w => new WaterInDayDto()
                {

                    NumberOfCupsDrinked = w.NumberOfCupsDrinked,
                    Date = w.Date.ToString("yyyy-MM-dd") // Format the date as needed
                })
            });

        }

        [HttpGet("get_Athlete_profile")]
        [Authorize(Roles = "Athlete")]
        public async Task<IActionResult> GetAthleteProfile()
        {
            var phoneNumber = User.FindFirst(ClaimTypes.Name)?.Value;
            if (phoneNumber is null) return BadRequest(new ApiResponse() { Action = false, Message = "PhoneNumber is null" });
            var user = await context.Users
                .Include(u => u.Athlete).FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
            if (user is null) return BadRequest(new ApiResponse() { Action = false, Message = "User not found" });

            return Ok(new ApiResponse() { Action = true, Message = "User found", Result = user.ToAthleteProfileResponseDto() });

        }

        [HttpPut("search_Coaches")]
        [Authorize(Roles = "Athlete")]
        public async Task<IActionResult> SearchCoaches([FromBody] CoachNameSearchDto coachNameSearchDto)
        {
            var resualt = await athleteRepository.SearchCoaches(coachNameSearchDto);
            if (!resualt.Action) return BadRequest(resualt);
            return Ok(resualt);


        }
        [HttpGet("Get-Coaches")]
        [Authorize(Roles = "Athlete")]
        public async Task<IActionResult> GetCoaches()
        {
            var resualt = await context.Users.Where(c => c.TypeOfUser == TypeOfUser.COACH).ToListAsync();
            return Ok(new ApiResponse() { Action = true, Message = "Coaches found", Result = resualt.Select(c => c.ToCoachForSearch()).ToList() });
        }

        [HttpGet("get_coach_profile/{coachId}")]//need to make it with id
        [Authorize(Roles = "Athlete")]
        public async Task<IActionResult> GetCoachProfile([FromRoute] int coachId)
        {
            var coach = await context.Coaches.Include(c=>c.User).
                Include(c => c.CoachingServices).FirstOrDefaultAsync(c => c.Id == coachId);
            if (coach == null) return BadRequest(new ApiResponse() { Action = false, Message = "Coach not found" });
            var payments  = await context.Payments.Include(p=>p.Athlete).Include(p=>p.WorkoutProgram).
                Where(p => p.CoachId == coach.Id && p.WorkoutProgram != null && p.WorkoutProgram.Status!=WorkoutProgramStatus.WRITING).ToListAsync();
            var numberOfProgram = payments.Count(p => p.WorkoutProgram != null);
            var numberOfAthlete = payments.Select(x=>x.AthleteId).Distinct().Count();

            return Ok(new ApiResponse() { Action = true, Message = "Coach found", Result = coach.ToCoachProfileForAthleteDto( numberOfProgram,numberOfAthlete) });
        }
        [HttpPut("update_weight")]
        [Authorize(Roles = "Athlete")]
        public async Task<IActionResult> UpdateWeight(int weight)
        {
            var phoneNumber = User.FindFirst(ClaimTypes.Name)?.Value;
            if (phoneNumber is null) return BadRequest("PhoneNumber is null");
            var result = await athleteRepository.UpdateWeight(phoneNumber, weight);
            if (!result.Action) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("get_weight_report")]
        [Authorize(Roles = "Athlete")]
        public async Task<IActionResult> GetWeightReport()
        {
            var phoneNumber = User.FindFirst(ClaimTypes.Name)?.Value;
            if (phoneNumber is null) return BadRequest("PhoneNumber is null");
            var result = await athleteRepository.WeightReport(phoneNumber);
            if (!result.Action) return BadRequest(result);
            return Ok(result);
        }


        [HttpPost("add_Activity")]
        [Authorize(Roles = "Athlete")]
        public async Task<IActionResult> AddActivity([FromBody] AddActivityDto addActivityDto)
        {
            var phoneNumber = User.FindFirst(ClaimTypes.Name)?.Value;
            if (phoneNumber is null) return BadRequest("PhoneNumber is null");
            var result = await athleteRepository.AddActivity(phoneNumber, addActivityDto);
            if (!result.Action) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("get_activity_report")]
        [Authorize(Roles = "Athlete")]
        public async Task<IActionResult> GetActivityReport()
        {
            var phoneNumber = User.FindFirst(ClaimTypes.Name)?.Value;
            if (phoneNumber is null) return BadRequest("PhoneNumber is null");
            var result = await athleteRepository.ActivityReport(phoneNumber);
            if (!result.Action) return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("delete_activity")]
        [Authorize(Roles = "Athlete")]
        public async Task<IActionResult> DeleteActivity(int activityId)
        {
            var phoneNumber = User.FindFirst(ClaimTypes.Name)?.Value;
            if (phoneNumber is null) return BadRequest("PhoneNumber is null");
            var result = await athleteRepository.DeleteActivity(phoneNumber, activityId);
            if (!result.Action) return BadRequest(result);
            return Ok(result);
        }


        [HttpPost("buy_Service/{serviceId:int}")]
        [Authorize(Roles = "Athlete")]
        public async Task<IActionResult> BuyCoachingService([FromRoute] int serviceId)
        {
            var phoneNumber = User.FindFirst(ClaimTypes.Name)?.Value;
            if (phoneNumber is null) return BadRequest("PhoneNumber is null");
            var result = await athleteRepository.BuyCoachingService(phoneNumber, serviceId);
            if (!result.Action) return BadRequest(result);
            return Ok(result);
        }

        [HttpPut("Update_TimeBeforeWorkout")]
        [Authorize(Roles = "Athlete")]
        public async Task<IActionResult> UpdateTimeBeforeWorkout([FromRoute] int timeBeforeWorkoutDto)
        {
            var phoneNumber = User.FindFirst(ClaimTypes.Name)?.Value;
            if (phoneNumber is null) return BadRequest("PhoneNumber is null");
            var athlete = await context.Athletes.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
            if (athlete is null) return BadRequest("User not found");
            athlete.TimeBeforeWorkout = timeBeforeWorkoutDto;
            return Ok(new ApiResponse()
            {

                Action = true,
                Message = "TimeBeforeWorkout updated"
            });

        }
        [HttpPut("Update_RestTime")]
        [Authorize(Roles = "Athlete")]
        public async Task<IActionResult> UpdateRestTime([FromRoute] int restTimeDto)
        {
            var phoneNumber = User.FindFirst(ClaimTypes.Name)?.Value;
            if (phoneNumber is null) return BadRequest("PhoneNumber is null");
            var athlete = await context.Athletes.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
            if (athlete is null) return BadRequest("User not found");
            athlete.RestTime = restTimeDto;
            return Ok(new ApiResponse()
            {

                Action = true,
                Message = "RestTime updated"
            });

        }
        [HttpGet("get_lastQuestion")]
        [Authorize(Roles = "Athlete")]
        public async Task<IActionResult> GetLastQuestion()
        {
            var phoneNumber = User.FindFirst(ClaimTypes.Name)?.Value;
            if (phoneNumber is null) return BadRequest("PhoneNumber is null");
            var result = await athleteRepository.GetLastQuestion(phoneNumber);
            if (!result.Action) return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("complete new challenge")]
        [Authorize(Roles = "Athlete")]
        public async Task<IActionResult> CompleteNewChallenge([FromBody] string challenge)
        {
            var phoneNumber = User.FindFirst(ClaimTypes.Name)?.Value;
            if (phoneNumber is null) return BadRequest("PhoneNumber is null");
            var result = await athleteRepository.CompleteNewChallenge(phoneNumber, challenge);
            if (!result.Action) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("completed_challenge")]
        [Authorize(Roles = "Athlete")]
        public async Task<IActionResult> CompletedChallenge()
        {
            var phoneNumber = User.FindFirst(ClaimTypes.Name)?.Value;
            if (phoneNumber is null) return BadRequest("PhoneNumber is null");
            var result = await athleteRepository.CompletedChallenge(phoneNumber);
            if (!result.Action) return BadRequest(result);
            return Ok(result);

        }

        [HttpGet("get_achievements")]
        [Authorize(Roles = "Athlete")]
        public async Task<IActionResult> GetAchievements()
        {
            var phoneNumber = User.FindFirst(ClaimTypes.Name)?.Value;
            if (phoneNumber is null) return BadRequest("PhoneNumber is null");
            var result = await athleteRepository.GetAchievements(phoneNumber);
            if (!result.Action) return BadRequest(result);
            return Ok(result);
        }
        
        [HttpGet("get_AllPrograms")]
        [Authorize(Roles = "Athlete")]
        public async Task<IActionResult> GetAllPrograms()
        {
            var phoneNumber = User.FindFirst(ClaimTypes.Name)?.Value;
            if (phoneNumber is null) return BadRequest("PhoneNumber is null");
            var result = await athleteRepository.GetAllPrograms(phoneNumber);
            if (!result.Action) return BadRequest(result);
            return Ok(result);
        }
        [HttpGet("get_Program")]
        [Authorize(Roles = "Athlete")]
        public async Task<IActionResult> GetProgram([FromRoute] int PaymentId)
        {
            var phoneNumber = User.FindFirst(ClaimTypes.Name)?.Value;
            if (phoneNumber is null) return BadRequest("PhoneNumber is null");
            var result = await athleteRepository.GetProgram(phoneNumber,PaymentId);
            if (!result.Action) return BadRequest(result);
            return Ok(result);
        }

    }
}